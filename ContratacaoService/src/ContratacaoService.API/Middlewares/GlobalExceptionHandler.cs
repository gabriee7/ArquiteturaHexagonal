using ContratacaoService.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Erro capturado: {Message}", exception.Message);
            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Não encontrado"),
                DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
                _ => (StatusCodes.Status500InternalServerError, "Erro Interno no Servidor")
            };
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception is DomainException or NotFoundException
                    ? exception.Message
                    : "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
