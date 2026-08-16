
using LibraryManagement.Persistence;
using LibraryManagement.Persistence.Data;
using LibraryManagement.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(AuthenticationController).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Allow Presistence Service
            builder.Services.AddPersistenceServices(builder.Configuration);

            // Allow AutoMapper
            builder.Services.AddAutoMapper(config => 
            {
                config.AddMaps(typeof(LibraryManagement.Services.AssemblyReference).Assembly);
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
