namespace LibraryManagement.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddAngularCorsPolicy(
                this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AngularClient", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("Retry-After"); ;
                });
            });

            return services;
        }

    }
}
    