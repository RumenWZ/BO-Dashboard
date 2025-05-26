using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
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
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                    ? new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }
                    : new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "An internal server error occurred."
                    };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

        private class ErrorDetails
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public string? StackTrace { get; set; }
        }
    }
}
