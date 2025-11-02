namespace ProductCatalog.Application.Common.Results;

public class Result<T> : Result
{
    private readonly T? _value;

    protected Result(T value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException();

    public static Result<T> Success(T value) => new(value, true, null);
    public new static Result<T> Failure(string error) => new(default, false, error);
    public static implicit operator Result<T>(T value) => Success(value);
}