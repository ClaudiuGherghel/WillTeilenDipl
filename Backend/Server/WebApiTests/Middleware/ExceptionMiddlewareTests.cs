using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens.Experimental;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Middleware;

namespace WebApiTests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<string> InvokeMiddlewareAsync(HttpContext context, Exception exception)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(_ => throw exception, mockLogger.Object);

            // Act
            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
        [Fact]
        public async Task Handles_ValidationException_Returns_BadRequest()
        {
            // Arrange
            var context = CreateHttpContext();
            var validationResult = new ValidationResult("Name fehlt", ["Name"]);
            var validationException = new ValidationException(validationResult, null, new { Name = "" });

            // Act
            var result = await InvokeMiddlewareAsync(context, validationException);

            // Assert
            Assert.Equal("application/problem+json", context.Response.ContentType);
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Contains("Validation error", result);
            Assert.Contains("Name fehlt", result);
        }


        [Fact]
        public async Task Handles_DbUpdateConcurrencyException_Returns_Conflict()
        {
            var context = CreateHttpContext();
            var result = await InvokeMiddlewareAsync(context, new DbUpdateConcurrencyException());

            Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
            Assert.Contains("Concurrency conflict", result);
        }

        [Fact]
        public async Task Handles_DbUpdateException_Returns_BadRequest()
        {
            var context = CreateHttpContext();
            var result = await InvokeMiddlewareAsync(context, new DbUpdateException());

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Contains("Database error", result);
        }

        [Fact]
        public async Task Handles_UnauthorizedAccessException_Returns_Forbidden()
        {
            var context = CreateHttpContext();
            var result = await InvokeMiddlewareAsync(context, new UnauthorizedAccessException());

            Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
            Assert.Contains("Zugriff verweigert", result);
        }

        [Fact]
        public async Task Handles_ImageUploadException_Returns_InternalServerError()
        {
            var context = CreateHttpContext();
            var result = await InvokeMiddlewareAsync(context, new ImageUploadException("Fehler beim Upload"));

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Contains("Bild-Upload-Fehler", result);
            Assert.Contains("Fehler beim Upload", result);
        }

        [Fact]
        public async Task Handles_GenericException_Returns_InternalServerError()
        {
            var context = CreateHttpContext();
            var result = await InvokeMiddlewareAsync(context, new Exception("Etwas ist kaputt"));

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Contains("Internal server error", result);
            Assert.Contains("Etwas ist kaputt", result);
        }
    }
}
