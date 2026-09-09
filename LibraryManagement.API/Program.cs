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

            var app = builder.Build();

            // 1. Global Exception Handling
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            // 2. Serilog Request Logging
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

            // 3. HTTPS Redirection
            app.UseHttpsRedirection();

            // 4. Static Files 
            app.UseStaticFiles();

            // 5. CORS
            app.UseCors("AngularClient");

            // 6. Rate Limiting
            // app.UseRateLimiter();

            // 7. Authentication
            app.UseAuthentication();

            // 8. Authorization
            app.UseAuthorization();

            // 9. Mapped Controllers
            app.MapControllers();

            // 10. Health Checks 
            // app.MapHealthChecks("/health");

            app.Run();
        }
    }
}