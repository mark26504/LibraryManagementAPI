namespace LibraryManagement.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:SecretKey"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException(
                    "JWT SecretKey is missing from configuration.");

            if (Encoding.UTF8.GetByteCount(secretKey) < 32)
                throw new InvalidOperationException(
                    "JWT SecretKey must be at least 32 bytes long.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException(
                    "JWT Issuer is missing from configuration.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException(
                    "JWT Audience is missing from configuration.");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(secretKey)),

                            ValidateIssuer = true,
                            ValidIssuer = issuer,

                            ValidateAudience = true,
                            ValidAudience = audience,

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,

                            NameClaimType = "sub",
                            RoleClaimType = "role"
                        };

                    options.IncludeErrorDetails = false;
                });

            services.AddAuthorization();

            return services;
        }
    }
}