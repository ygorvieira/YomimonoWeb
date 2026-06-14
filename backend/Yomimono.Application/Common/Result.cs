using System.Net;

namespace Yomimono.Application.Common;

public class Result<T>
{
    public bool Valid { get; private set; }
    public T? Data { get; private set; }
    public List<string> Messages { get; private set; } = [];
    public HttpStatusCode StatusCode { get; private set; }

    private Result() { }

    public static Result<T> Success(T data, string? message = null)
    {
        var result = new Result<T>
        {
            Valid = true,
            Data = data,
            StatusCode = HttpStatusCode.OK
        };
        if (message is not null)
            result.Messages.Add(message);
        return result;
    }

    public static Result<T> Created(T data, string? message = null)
    {
        var result = new Result<T>
        {
            Valid = true,
            Data = data,
            StatusCode = HttpStatusCode.Created
        };
        if (message is not null)
            result.Messages.Add(message);
        return result;
    }

    public static Result<T> Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new Result<T>
        {
            Valid = false,
            StatusCode = statusCode,
            Messages = [message]
        };
    }

    public static Result<T> NotFound(string message = "Registro não encontrado.")
    {
        return new Result<T>
        {
            Valid = false,
            StatusCode = HttpStatusCode.NotFound,
            Messages = [message]
        };
    }
}
