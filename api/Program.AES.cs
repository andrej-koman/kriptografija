using System.Security.Cryptography;

namespace API
{
    internal partial class Program
    {
        public static void AES(WebApplication app)
        {
            app.MapPost("/aes/encrypt", async context =>
            {
                // Preverimo če je ključ podan
                if (!int.TryParse(context.Request.Query["keyLength"], out int keyLength))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Key length must be provided.");
                    return;
                };

                // Preverimo ustreznost ključa
                if (keyLength < 128 || keyLength > 256 || keyLength % 8 != 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Key length must be a multiple of 8 and between 128 and 256 bits.");
                    return;
                }


                // Generiramo naključni ključ in IV
                byte[] key = new byte[keyLength / 8];
                byte[] iv = new byte[16];
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(key);
                    rngCsp.GetBytes(iv);
                }

                // Preberemo datoteko
                IFormFile? file = context.Request.Form.Files["file"];

                // Preverimo, če je datoteka pravilno prebrana
                if (file == null || file.Length == 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("File must be provided.");
                    return;
                }

                // Preberemo ime datoteke in velikost
                string fileName = file.FileName;
                long fileSize = file.Length;


                string outputFileName = Path.GetFileNameWithoutExtension(fileName) + "_encrypted" + Path.GetExtension(fileName);
                string outputPath = Path.Combine(Path.GetDirectoryName(fileName), outputFileName);

                int bufferSize = 1024 * 1024; // 1 MB

                // Zašifriramo datoteko
                using (Aes aes = Aes.Create())
                using (Stream input = file.OpenReadStream())
                using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(output, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await input.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await cryptoStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                // Vrnemo datoteko, ključ in IV
                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Expose-Headers", "Key, IV");
                context.Response.Headers.Add("Key", Convert.ToBase64String(key));
                context.Response.Headers.Add("IV", Convert.ToBase64String(iv));
                await context.Response.SendFileAsync(outputPath);
                // Remove the file
                File.Delete(outputPath);
            });

            app.MapPost("/aes/decrypt", async context =>
            {
                // Preberemo ključ in IV
                string? keyString = context.Request.Form["key"];
                string? ivString = context.Request.Form["iv"];
                if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Key and IV must be provided in the headers.");
                    return;
                }

                byte[] key = Convert.FromBase64String(keyString);
                byte[] iv = Convert.FromBase64String(ivString);

                // Preberemo datoteko
                IFormFile? file = context.Request.Form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("File must be provided.");
                    return;
                }


                // Preberemo ime datoteke in velikost
                string fileName = file.FileName;
                long fileSize = file.Length;

                string outputFileName = Path.GetFileNameWithoutExtension(fileName) + "_decrypted" + Path.GetExtension(fileName);
                string outputPath = Path.Combine(Path.GetDirectoryName(fileName), outputFileName);

                int bufferSize = 1024 * 1024; // 1 MB

                // Dešifriramo datoteko
                using (Aes aes = Aes.Create())
                using (Stream input = file.OpenReadStream())
                using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(input, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await cryptoStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.AccessControlAllowOrigin = "*";
                await context.Response.SendFileAsync(outputPath);
                // Remove the file
                File.Delete(outputPath);
            });
        }
    }
}
