using System.Net;

namespace WebApi.Helpers.Exceptions;

public class HttpException : Exception {
    public HttpStatusCode StatusCode { get; }
    public override string Message { get; }
    public Exception? Error { get; }

    public HttpException(HttpStatusCode statusCode, string message, Exception? error = null) : base(message) {
        StatusCode = statusCode;
        Message = message;
        Error = error;
    }
}