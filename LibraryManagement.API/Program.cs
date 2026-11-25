using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace LibraryManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddSerilogLogging();

            builder.Services
                .AddControllers()
                .AddApplicationPart(
                    typeof(AuthenticationController).Assembly)
                    .AddFrozenValidationProblemDetails(); ;

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Library Management API",
                    Version = "v1",
                    Description = """
                        JWT Bearer access token in the Authorization header.
                        The refresh token travels only in a Secure HttpOnly cookie named 'refreshToken'
                        scoped to /api/v1/auth. The refresh and revoke endpoints expect an empty body ({})
                        and read the cookie automatically. Swagger cannot send HttpOnly cookies,
                        so verify refresh/revoke through the Angular client or Postman with a cookie jar.
                        """
                });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("bearer", document)] = []
                    });
            });

            // Persistence + Business Services
            builder.Services.AddPersistenceServices(
                builder.Configuration);

            // Configurable Data Protection key storage
            var keysPath = builder.Configuration["DataProtection:KeysPath"];

            if (!string.IsNullOrWhiteSpace(keysPath))
            {
                var fullPath = Path.GetFullPath(
                    keysPath,
                    builder.Environment.ContentRootPath);

                Directory.CreateDirectory(fullPath);

                builder.Services.AddDataProtection()
                    .SetApplicationName("LibraryManagement.API")
                    .PersistKeysToFileSystem(new DirectoryInfo(fullPath));
            }

            // SMTP Security - Development-only local SMTP
            builder.Services.AddOptions<EmailOptions>()
                .Validate(
                    options => builder.Environment.IsDevelopment() ||
                               !options.UseLocalSmtp,
                    "Local SMTP is only allowed in Development.")
                .ValidateOnStart();

            // Angular CORS
            builder.Services.AddAngularCorsPolicy(
                builder.Configuration,
                builder.Environment);

            // JWT Authentication + Authorization
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.AddApiRateLimiting();

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                options.ForwardLimit = 1;

                var trustedNetworks = builder.Configuration
                    .GetSection("ReverseProxy:KnownNetworks")
                    .Get<string[]>() ?? [];

                foreach (var network in trustedNetworks)
                {
                    options.KnownIPNetworks.Add(
                        System.Net.IPNetwork.Parse(network));
                }
            });

            var app = builder.Build();

            if (builder.Configuration.GetValue<bool>("ReverseProxy:Enabled"))
            {
                app.UseForwardedHeaders();
            }

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | TraceId: {TraceId}";

                options.EnrichDiagnosticContext += (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set(
                        "TraceId",
                        Activity.Current?.Id ?? httpContext.TraceIdentifier);
                };
            });

            if (app.Environment.IsDevelopment() ||
                builder.Configuration.GetValue<bool>("Swagger:Enabled"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("AngularClient");

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Liveness Health Check
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            // Readiness Health Check
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.Run();
        }
    }
}