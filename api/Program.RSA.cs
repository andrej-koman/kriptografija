using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API
{
    internal partial class Program
    {
        public static void RSA(WebApplication app)
        {
            app.MapPost("/rsa/encrypt", async (context) =>
            {
                // Get they public key from the form data
                IFormFile? publicKey = context.Request.Form.Files["publicKey"];
                string publicKeyString;

                // Save the public key to a string
                using (StreamReader reader = new StreamReader(publicKey.OpenReadStream()))
                {
                    publicKeyString = await reader.ReadToEndAsync();
                }

                if (String.IsNullOrEmpty(publicKeyString))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Missing public key");
                    return;
                }

                RSA rsa;
                try
                {
                    rsa = RSACryptoServiceProvider.Create();
                    rsa.ImportFromPem(publicKeyString);
                }
                catch (CryptographicException)
                {
                    // The public key is invalid, return an error response
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid public key");
                    return;
                }
                // Generate AES key and IV
                byte[] aesKey = new byte[32];
                byte[] aesIV = new byte[16];

                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(aesKey);
                    rngCsp.GetBytes(aesIV);
                }

                // Encrypt the file with AES
                IFormFile? file = context.Request.Form.Files["file"];
                string outputFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_encrypted" + Path.GetExtension(file.FileName);
                string outputPath = Path.Combine(Path.GetDirectoryName(file.FileName), outputFileName);

                int bufferSize = 1024 * 1024; // 1 MB

                using (Aes aes = Aes.Create())
                using (Stream input = file.OpenReadStream())
                using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(output, aes.CreateEncryptor(aesKey, aesIV), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await input.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await cryptoStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                // Encrypt AES key and IV with RSA
                byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);

                // Return the encrypted file, public key, and private key
                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Expose-Headers", "EncryptedKey, Iv");
                context.Response.Headers.Add("EncryptedKey", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(encryptedAesKey))));
                context.Response.Headers.Add("Iv", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(aesIV))));
                using (FileStream fs = new FileStream(outputPath, FileMode.Open))
                {
                    await fs.CopyToAsync(context.Response.Body);
                }
                // Remove the file
                File.Delete(outputPath);
            });

            app.MapPost("/rsa/decrypt", async (context) =>
            {
                // Get the encrypted file, public key, and private key from the form data
                IFormFile? encryptedFile = context.Request.Form.Files["file"];

                if (encryptedFile == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Missing encrypted file");
                    return;
                }

                string? encryptedKey = context.Request.Headers["EncryptedKey"];
                string? privateKey = context.Request.Form["privateKey"];
                string? iv = context.Request.Headers["Iv"];

                // Save the public key and private key to strings
                if (String.IsNullOrEmpty(encryptedKey) || String.IsNullOrEmpty(privateKey) || String.IsNullOrEmpty(iv))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Missing encrypted key, private key, or IV");
                    return;
                }

                RSA rsa;
                try
                {
                    rsa = RSACryptoServiceProvider.Create();
                    rsa.ImportFromPem(privateKey);
                }
                catch (CryptographicException)
                {
                    // The public key or private key is invalid, return an error response
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid public key or private key");
                    return;
                }

                // Decrypt the AES key with RSA
                byte[] aesKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.OaepSHA256);

                // Turn the IV into a byte array
                byte[] aesIv = Convert.FromBase64String(iv);

                // Decrypt the file with AES
                string outputFileName = Path.GetFileNameWithoutExtension(encryptedFile.FileName) + "_decrypted" + Path.GetExtension(encryptedFile.FileName);
                string outputPath = Path.Combine(Path.GetTempPath(), outputFileName);
                using (var inputFileStream = encryptedFile.OpenReadStream())
                {
                    using (var outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        using (var cryptoStream = new CryptoStream(inputFileStream, new AesManaged().CreateDecryptor(aesKey, aesIv), CryptoStreamMode.Read))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = await cryptoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await outputFileStream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                // Return the decrypted file
                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                using (FileStream fs = new FileStream(outputPath, FileMode.Open))
                {
                    await fs.CopyToAsync(context.Response.Body);
                }

                // Remove the file
                File.Delete(outputPath);
            });
        }
    }
}