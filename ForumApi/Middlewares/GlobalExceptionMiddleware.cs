using System.Net;
using System.Text.Json;

namespace ForumApi.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    status = 500,
                    message = ex.Message,
                    path = context.Request.Path
                };

                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
