namespace LibraryManagement.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, TokenProvider>();
            // services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}
