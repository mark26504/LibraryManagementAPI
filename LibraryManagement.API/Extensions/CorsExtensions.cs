namespace LibraryManagement.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddAngularCorsPolicy(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            if (environment.IsDevelopment() &&
                allowedOrigins.Length == 0)
            {
                allowedOrigins = ["http://localhost:4200"];
            }

            services.AddCors(options =>
            {
                options.AddPolicy("AngularClient", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("Retry-After");
                });
            });

            return services;
        }
    }
}