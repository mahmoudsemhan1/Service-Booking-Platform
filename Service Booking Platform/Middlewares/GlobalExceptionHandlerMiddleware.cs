using Application.DTOs.Error;
using System.Net;

namespace ServiceBooking.Api.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred on the server.";
            // Map specific exceptions to HTTP Status Codes
            switch (ex)
            {
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    message = "You do not have permission to access this resource.";
                    break;
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                    // Add more custom exceptions here as needed
            }

            context.Response.StatusCode = statusCode;

            var response = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message,
                // Include StackTrace only in Development environment for debugging
                Trace = _env.IsDevelopment() ? ex.StackTrace : null
            };

            await context.Response.WriteAsync(response.ToString());
        }
    }
}