namespace EF_CRUD.Common;

/// <summary>
/// Outcome of a service operation that returns no data (for example, delete).
/// Services never throw for expected failures; they return a failed result instead,
/// and the controller decides which HTTP status code that becomes.
/// </summary>
public class ServiceResult
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors =
        new Dictionary<string, string[]>();

    protected ServiceResult(
        ResultStatus status,
        string? message = null,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        Status = status;
        Message = message;
        Errors = errors ?? NoErrors;
    }

    public ResultStatus Status { get; }
    public string? Message { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.Created;

    public static ServiceResult Success() => new(ResultStatus.Ok);
    public static ServiceResult NotFound(string message) => new(ResultStatus.NotFound, message);
    public static ServiceResult Conflict(string message) => new(ResultStatus.Conflict, message);
    public static ServiceResult Unauthorized(string message) => new(ResultStatus.Unauthorized, message);
    public static ServiceResult Forbidden(string message) => new(ResultStatus.Forbidden, message);
    public static ServiceResult Failure(string message) => new(ResultStatus.Failed, message);

    public static ServiceResult Validation(IReadOnlyDictionary<string, string[]> errors) =>
        new(ResultStatus.ValidationFailed, "One or more validation errors occurred.", errors);
}

/// <summary>Outcome of a service operation that returns data of type <typeparamref name="T"/>.</summary>
public sealed class ServiceResult<T> : ServiceResult
{
    private ServiceResult(
        ResultStatus status,
        T? data = default,
        string? message = null,
        IReadOnlyDictionary<string, string[]>? errors = null)
        : base(status, message, errors)
    {
        Data = data;
    }

    public T? Data { get; }

    public static ServiceResult<T> Success(T data) => new(ResultStatus.Ok, data);
    public static ServiceResult<T> Created(T data) => new(ResultStatus.Created, data);

    public new static ServiceResult<T> NotFound(string message) =>
        new(ResultStatus.NotFound, message: message);

    public new static ServiceResult<T> Conflict(string message) =>
        new(ResultStatus.Conflict, message: message);

    public new static ServiceResult<T> Unauthorized(string message) =>
        new(ResultStatus.Unauthorized, message: message);

    public new static ServiceResult<T> Forbidden(string message) =>
        new(ResultStatus.Forbidden, message: message);

    public new static ServiceResult<T> Failure(string message) =>
        new(ResultStatus.Failed, message: message);

    public new static ServiceResult<T> Validation(IReadOnlyDictionary<string, string[]> errors) =>
        new(ResultStatus.ValidationFailed,
            message: "One or more validation errors occurred.",
            errors: errors);
}
