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
                byte[] aesKey = new byte[16];
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
                context.Response.Headers.Add("Access-Control-Expose-Headers", "EncryptedKey");
                context.Response.Headers.Add("EncryptedKey", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(encryptedAesKey))));
                using (FileStream fs = new FileStream(outputPath, FileMode.Open))
                {
                    await fs.CopyToAsync(context.Response.Body);
                }
                // Remove the file
                File.Delete(outputPath);
            });

            app.MapPost("/rsa/decrypt", () =>
            {

            });

        }
    }
}