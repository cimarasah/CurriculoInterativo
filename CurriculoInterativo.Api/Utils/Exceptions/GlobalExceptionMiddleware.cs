using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace CurriculoInterativo.Api.Utils.Exceptions
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
                _logger.LogError(ex, "Erro não tratado na aplicação");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse()
            {
                Message = "Erro interno do servidor",
                Details = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new ErrorResponse
                    {
                        Message = "Acesso não autorizado",
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ErrorResponse
                    {
                        Message = "Dados inválidos fornecidos",
                        Details = exception.Message,
                        Timestamp = DateTime.UtcNow
                    };
                    break;
                case ArgumentNullException:
                case ArgumentException:
                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ErrorResponse
                    {
                        Message = "Requisição inválida",
                        Details = exception.Message,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case NotFoundException:
                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ErrorResponse
                    {
                        Message = "Registro não encontrado",
                        Details = exception.Message,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case TimeoutException:
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response = new ErrorResponse
                    {
                        Message = "Tempo limite da requisição excedido",
                        Timestamp = DateTime.UtcNow
                    };
                    break;


                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new ErrorResponse
                    {
                        Message = "Erro interno do servidor",
                        Details = exception.Message,
                        Timestamp = DateTime.UtcNow
                    };
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
