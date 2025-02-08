using System.Net;
using System.Text.Json;

public class ExceptionMiddlewareService
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddlewareService> _logger;

    public ExceptionMiddlewareService(RequestDelegate next, ILogger<ExceptionMiddlewareService> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro capturado no middleware.");

            // Determinar dinamicamente o status HTTP com base no tipo da exceção
            var statusCode = ex switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest, // 400
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404
                _ => (int)HttpStatusCode.InternalServerError // 500 para qualquer outro erro
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                Status = statusCode,
                Message = ex.Message, // Mantém a mensagem original da exceção
                ErrorType = ex.GetType().Name
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
