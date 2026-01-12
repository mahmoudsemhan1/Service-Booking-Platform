using Application.DTOs.Error;
using FluentValidation; // تأكد من إضافة هذه الـ Namespace
using System.Net;
using System.Text.Json;

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
                _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred on the server.";

            switch (ex)
            {
                // إضافة معالجة أخطاء الـ FluentValidation
                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    // هنا نجمع كل رسائل الخطأ من الحقول المختلفة ونضعها في الـ Message
                    message = string.Join(" | ", validationEx.Errors.Select(e => e.ErrorMessage));
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    message = "You do not have permission to access this resource.";
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case InvalidOperationException:
                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
            }

            // في بيئة التطوير، نريد رؤية الخطأ الحقيقي إذا لم يكن خطأ Validation
            if (_env.IsDevelopment() && ex is not ValidationException)
            {
                message = ex.Message;
            }

            context.Response.StatusCode = statusCode;

            var response = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message,
                Trace = _env.IsDevelopment() ? ex.StackTrace : null
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}