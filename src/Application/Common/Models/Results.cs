namespace ConnectFlow.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors, IEnumerable<string> messages)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        Messages = messages.ToArray();
    }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }
    public string[] Messages { get; init; }

    public static Result Success(params string[] messages)
    {
        messages = messages ?? Array.Empty<string>();

        return new Result(true, Array.Empty<string>(), messages);
    }

    public static Result Failure(params string[] errors)
    {
        errors = errors ?? Array.Empty<string>();

        return new Result(false, errors, Array.Empty<string>());
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    internal Result(bool succeeded, IEnumerable<string> errors, IEnumerable<string> messages, T? data) : base(succeeded, errors, messages)
    {
        Data = data;
    }

    public static Result<T> Success(T? data, params string[] messages)
    {
        messages = messages ?? Array.Empty<string>();
        return new Result<T>(true, Array.Empty<string>(), messages, data);
    }

    public static Result<T> Failure(T? data, params string[] errors)
    {
        errors = errors ?? Array.Empty<string>();

        return new Result<T>(false, errors, Array.Empty<string>(), data);
    }
}