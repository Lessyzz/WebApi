namespace WebApi.Models;


public class ResponseException : Exception
{
    public int StatusCode { get; }

    public ResponseException(int statusCode, string? message) : base(message)
    {
        StatusCode = statusCode;
    }

    public ResponseException(int statusCode, string message, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }
}