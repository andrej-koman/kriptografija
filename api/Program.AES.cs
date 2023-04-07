using System.Security.Cryptography;

namespace API
{
    internal partial class Program
    {
        public static void AES(WebApplication app)
        {
            app.MapPost("/aes/encrypt" , async context =>
            {
                
                context.Request.Headers.AccessControlAllowOrigin = "*";
                if (!int.TryParse(context.Request.Query["keyLength"], out int keyLength))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Key length must be provided.");
                    return;
                };

                if (keyLength < 128 || keyLength > 256 || keyLength % 8 != 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Key length must be a multiple of 8 and between 128 and 256 bits.");
                    return;
                }

                byte[] key = new byte[keyLength / 8];
                byte[] iv = new byte[16];
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(key);
                    rngCsp.GetBytes(iv);
                }

                IFormFile? file = context.Request.Form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("File must be provided.");
                    return;
                }

                string fileName = file.FileName;
                long fileSize = file.Length;

                string outputFileName = Path.GetFileNameWithoutExtension(fileName) + "_encrypted" + Path.GetExtension(fileName);
                string outputPath = Path.Combine(Path.GetDirectoryName(fileName), outputFileName);

                int bufferSize = 1024 * 1024; // 1 MB

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

                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Expose-Headers", "Key, IV");
                context.Response.Headers.Add("Key", Convert.ToBase64String(key));
                context.Response.Headers.Add("IV", Convert.ToBase64String(iv));
                await context.Response.SendFileAsync(outputPath);
            });



            app.MapPost("/aes/decrypt", () =>
            {

            });
        }
    }
}
