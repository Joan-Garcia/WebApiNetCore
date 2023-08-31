using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers.Exceptions;

namespace WebApi.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware {

    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        try {
            await next(context);
            
        } catch (Exception e) {
            ProblemDetails errorDetails = GetProblemDetails(context, e);

            await context.Response.WriteAsJsonAsync(errorDetails);
        }
    }

    private ProblemDetails GetProblemDetails(HttpContext context, Exception e) {
        string? idPersonal = context.User.Claims.First(c => c.Type == "idPersonal")?.Value;

        string errorId = Guid.NewGuid().ToString();

        switch (e) {
            case HttpException httpException:
                _logger.LogError(
                    "Request Error [idPersonal: {@IdPersonal}, errorId: {@ErrorId}] : {@Method} {@Path} => {@ErrorMessage}",
                    idPersonal,
                    errorId,
                    context.Request.Method,
                    context.Request.Path.Value,
                    e.Message
                );
                context.Response.StatusCode = (int)httpException.StatusCode;

                return new() {
                    Status = (int)httpException.StatusCode,
                    Title = "Http Error",
                    Detail = httpException.Message,
                    Instance = context.Request.Path,
                    Extensions = {
                        { "errorId", errorId }
                    }
                };
            case MysqlException mySqlException:
                _logger.LogError(
                    "Database Error [idPersonal: {@IdPersonal}, errorId: {@ErrorId}] : {@Method} {@Path} => {@ErrorMessage} Query: {@Query}",
                    idPersonal,
                    errorId,
                    context.Request.Method,
                    context.Request.Path.Value,
                    e.Message,
                    mySqlException.Query
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return new() {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Database Error",
                    Detail = "Internal Server Error",
                    Instance = context.Request.Path,
                    Extensions = {
                        { "errorId", errorId }
                    }
                };
            default:
                _logger.LogError(
                    "Request Failed [idPersonal: {@IdPersonal}, errorId: {@ErrorId}] : {@Method} {@Path} => {@Error}",
                    idPersonal,
                    errorId,
                    context.Request.Method,
                    context.Request.Path.Value,
                    e
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return new() {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Internal Server Error",
                    Detail = e.Message,
                    Instance = context.Request.Path,
                    Extensions = {
                        { "errorId", errorId }
                    }
                };
        }
    }
}