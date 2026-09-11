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
                    typeof(AuthenticationController).Assembly);

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

            // Angular CORS
            builder.Services.AddAngularCorsPolicy();

            // JWT Authentication + Authorization
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.AddApiRateLimiting();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

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

            if (app.Environment.IsDevelopment())
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

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}