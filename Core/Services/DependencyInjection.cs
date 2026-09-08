namespace LibraryManagement.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IBorrowingService, BorrowingService>();

            services.AddAutoMapper(
                cfg => { },
                typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
