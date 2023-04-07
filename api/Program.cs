namespace API
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
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

            app.Run();
        }
    }
}

