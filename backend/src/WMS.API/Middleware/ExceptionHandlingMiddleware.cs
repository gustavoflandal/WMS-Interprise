using System.Net;
using System.Text.Json;
using WMS.API.Models;

namespace WMS.API.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções
    /// Captura todas as exceções não tratadas e retorna uma resposta padronizada
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case OperationCanceledException:
                    // Requisição cancelada
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Operação cancelada pelo cliente";
                    break;

                case InvalidOperationException:
                    // Operação inválida
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;

                case ArgumentException:
                    // Argumento inválido
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;

                case KeyNotFoundException:
                    // Recurso não encontrado
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Recurso não encontrado";
                    break;

                case UnauthorizedAccessException:
                    // Acesso não autorizado
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Acesso não autorizado";
                    break;

                case TimeoutException:
                    // Timeout
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Message = "A requisição expirou. Por favor, tente novamente.";
                    break;

                default:
                    // Erro genérico não esperado (500 Internal Server Error)
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.";
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsJsonAsync(response, jsonOptions);
        }
    }
}
