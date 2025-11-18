namespace WMS.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = new List<string>();
        if (error != null) Errors.Add(error);
    }

    protected Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.FirstOrDefault();
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T value) : base(true)
    {
        Value = value;
    }

    protected Result(string error) : base(false, error)
    {
        Value = default;
    }

    protected Result(List<string> errors) : base(false, errors)
    {
        Value = default;
    }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(string error) => new(error);
    public new static Result<T> Failure(List<string> errors) => new(errors);
}
