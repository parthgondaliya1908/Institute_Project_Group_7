namespace Api.Common.Types;

#region Do not touch

public enum SuccessStatus
{
    Ok,
    Created
}

public class ResultBase
{
    public bool IsSuccess { get; set; }
    public SuccessStatus? SuccessStatus { get; set; }
    public Error? Error { get; set; } = null!;
    public string? Url { get; set; }
}

public class Result<T> : ResultBase
{
    public T Value { get; set; } = default!;

    public Result AsNonGeneric() => new()
    {
        IsSuccess = IsSuccess,
        SuccessStatus = SuccessStatus,
        Error = Error,
        Url = Url,
    };

    public static implicit operator Result<T>(Result result) => new()
    {
        IsSuccess = result.IsSuccess,
        SuccessStatus = result.SuccessStatus,
        Error = result.Error,
        Url = result.Url,
    };
}
#endregion

public class Result : ResultBase
{
    public static Result Success()
    {
        return new Result()
        {
            IsSuccess = true,
            SuccessStatus = Types.SuccessStatus.Ok,
            Url = null,
        };
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            SuccessStatus = Types.SuccessStatus.Ok,
            Value = value,
            Url = null,
        };
    }

    public static Result Created(string? url = null)
    {
        return new Result()
        {
            IsSuccess = true,
            SuccessStatus = Types.SuccessStatus.Created,
            Url = url
        };
    }

    public static Result<T> Created<T>(T value, string? url = null)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            SuccessStatus = Types.SuccessStatus.Created,
            Value = value,
            Url = url
        };
    }    

    public static Result Fail(Error error) => new() { Error = error };
    public static Result Fail(string? message = null) => Fail(new Error(message));
    public static Result UnauthorizedError(string? message = null) => Fail(new UnauthorizedError(message));
    public static Result ConflictError(string? message = null) => Fail(new ConflictError(message));
    public static Result NotFoundError(string? message = null) => Fail(new NotFoundError(message));
    public static Result CannotProcessError(string? message = null) => Fail(new CannotProcessError(message));
    public static Result BadRequestError(string? message = null) => Fail(new BadRequestError(message));
}
