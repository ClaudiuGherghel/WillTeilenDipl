using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using WebApi.Controllers;

namespace WebApi.Middleware
{


    /* Bsp. Output
        {
          "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
          "title": "Validation Error",
          "status": 400,
          "detail": "Category mit gleichem Namen existiert bereits.",
          "instance": "/api/categories/Post",
          "traceId": "00-4fd4b...-00",
          "timestamp": "2025-07-22T18:45:00Z"
        }
    */

    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails();

            switch (exception)
            {
                case ValidationException validationEx:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Title = "Validation error";
                    problem.Detail = validationEx.ValidationResult?.ErrorMessage ?? "Validation failed";
                    problem.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    problem.Instance = context.Request.Path;
                    break;

                case DbUpdateConcurrencyException:
                    problem.Status = StatusCodes.Status409Conflict;
                    problem.Title = "Concurrency conflict";
                    problem.Detail = "Die Ressource wurde zwischenzeitlich geändert oder gelöscht.";
                    problem.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                    problem.Instance = context.Request.Path;
                    break;

                case DbUpdateException:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Title = "Database error";
                    problem.Detail = "Datenbankfehler beim Speichern oder Löschen.";
                    problem.Instance = context.Request.Path;
                    break;

                case UnauthorizedAccessException:
                    problem.Status = StatusCodes.Status403Forbidden;
                    problem.Title = "Zugriff verweigert";
                    problem.Detail = "Sie haben keine Berechtigung für diese Aktion.";
                    problem.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    break;

                case NotImplementedException:
                    problem.Status = StatusCodes.Status501NotImplemented;
                    problem.Title = "Nicht implementiert";
                    problem.Detail = "Diese Funktion ist noch nicht implementiert.";
                    break;

                case ImageUploadException imageUploadEx:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Bild-Upload-Fehler";
                    problem.Detail = imageUploadEx.Message;
                    break;

                default:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Internal server error";
                    problem.Detail = "Ein unbekannter Fehler ist aufgetreten: " + exception.Message;
                    problem.Instance = context.Request.Path;
                    break;
            }

            // Zusätzliche Felder ergänzen:
            problem.Instance = context.Request.Path;
            problem.Extensions["traceId"] = context.TraceIdentifier;
            problem.Extensions["timestamp"] = DateTime.UtcNow;

            context.Response.StatusCode = problem.Status ?? 500;

            var json = JsonSerializer.Serialize(problem);

            return context.Response.WriteAsync(json);
        }
    }
}
