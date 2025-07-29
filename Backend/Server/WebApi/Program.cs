using Core.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Middleware;
using Microsoft.OpenApi.Models;

namespace WebApi
{
    public class Program
    {
        /*  JWT-Authentifizierung aktiviert
            Token-Validierung vollständig
            Enums als String im JSON
            SoftDelete & Referenzzyklen korrekt ignoriert
            Swagger-UI mit JWT-Token-Unterstützung
        */

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JWT Einstellungen aus appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            if (jwtSettings == null || jwtSettings["Secret"] == null)
                throw new Exception("JwtSettings or Secret not found in appsettings.json");

            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true
                };
            });

            builder.Services.AddAuthorization();

            // DI
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // JSON Optionen (Enums als String + Referenzhandling)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Swagger mit JWT Support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Beispiel: 'Bearer <token>'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            var app = builder.Build();

            // Middleware-Reihenfolge beachten!
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthentication(); // Wichtig!
            app.UseAuthorization();

            app.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.MapControllers();

            app.Run();
        }
    }
}
