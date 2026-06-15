using System.Net;
using System.Text.Json.Serialization;

namespace Yomimono.Application.Common;

public class Result<T>
{
    [JsonInclude]
    public bool Valid { get; private set; }
    [JsonInclude]
    public T? Data { get; private set; }
    [JsonInclude]
    public List<string> Messages { get; private set; } = [];
    [JsonInclude]
    public HttpStatusCode StatusCode { get; private set; }

    [JsonConstructor]
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
