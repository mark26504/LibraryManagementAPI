using LibraryManagement.Persistence.Identity.Managers;
using LibraryManagement.Persistence.Identity.Store;
using LibraryManagement.Services.Abstraction.Contracts.Identity;

namespace LibraryManagement.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Add ApplicationDbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // Data Protection Configuration
            services.AddDataProtection();

            // Identity Configuration
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // JWT Options
            services.Configure<JwtOptions>(
                configuration.GetSection("Jwt"));

            //// Business Services
            services.AddBusinessServices();

            // Persistence Infrastructure
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityManager, IdentityManager>();
            services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
            return services;
        }
    }
}
