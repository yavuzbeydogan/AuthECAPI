using System.Runtime.CompilerServices;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(this WebApplication app,
            IConfiguration config)
        {
            // Configure the HTTP request pipeline.
            app.UseCors(options =>
                options.WithOrigins(
                    "http://10.211.55.4:4200",
                    "http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader());
            return app;
        }
    }
}
