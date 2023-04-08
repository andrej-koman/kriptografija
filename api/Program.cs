using System.Security.Cryptography;
namespace API
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            // TODO - Add key and IV checking
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(options =>
            {   
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            AES(app);
            RSA(app);

            app.Run();
        }
    }
}

