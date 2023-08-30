namespace WebApi.Helpers.Exceptions;

public class MysqlException : Exception {
    public string Query { get; }
    public Exception Error { get; }

    public MysqlException(string query, Exception error) : base(error.Message) {
        Query = query;
        Error = error;
    }
}