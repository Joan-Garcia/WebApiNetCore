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
        switch (e) {
            case HttpException httpException:
                _logger.LogError(
                    "Request error: {Method} {Path} => {ErrorMessage}",
                    context.Request.Method,
                    context.Request.Path,
                    e.Message
                );
                context.Response.StatusCode = (int)httpException.StatusCode;

                return new() {
                    Status = (int)httpException.StatusCode,
                    Title = "Http Error",
                    Detail = httpException.Message,
                    Instance = context.Request.Path
                };
            case MysqlException mySqlException:
                _logger.LogError(
                    "Database error: {Method} {Path} => {ErrorMessage} Query: {Query}",
                    context.Request.Method,
                    context.Request.Path,
                    e.Message,
                    mySqlException.Query
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return new() {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Database Error",
                    Detail = "Internal Server Error",
                    Instance = context.Request.Path
                };
            default:
                _logger.LogError(
                    "Request failed: {Method} {Path} => {Error}",
                    context.Request.Method,
                    context.Request.Path,
                    e
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return new() {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Internal Server Error",
                    Detail = e.Message,
                    Instance = context.Request.Path
                };
        }
    }
}