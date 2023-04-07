using System.Security.Cryptography;

namespace API 
{
    internal partial class Program
    {
        public static void RSA(WebApplication app)
        {
            app.MapPost("/rsa/encrypt", ()=> {

            });

            app.MapPost("/rsa/decrypt", ()=> {

            });
        }
    }
}