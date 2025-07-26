using Core.Contracts;
using Persistence;
using System.Text.Json.Serialization;
using WebApi.Middleware;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Enums als Strings statt Zahlen serialisieren/deserialisieren
                    // ? Eingabe z.?B. "role": "Admin" statt "role": 1
                    // ? Ausgabe z.?B. "role": "Admin" statt "role": 1
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //Statt Zahlen Enum-Werte
                });

            //Ignore circular references
            builder.Services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Swagger hinzufügen (Standard mit Swashbuckle)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            var app = builder.Build();

            // Global Exception Middleware als erstes in der Pipeline (vor UseRouting etc.)
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.MapControllers();

            app.Run();
        }
    }
}
