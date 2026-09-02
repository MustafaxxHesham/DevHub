namespace DevHub.Domain.Result;
public class SimpleResult<T> where T : struct
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string Error { get; set; }
    private SimpleResult(T? value)
    {
        Error = string.Empty;
        Value = value;
        IsSuccess = true;
    }
    private SimpleResult(string error)
    {
        Error = error;
        Value = null;
        IsSuccess = false;
    }
    public static SimpleResult<T> Success(T value) => new SimpleResult<T>(value);
    public static SimpleResult<T> Failure(string error) => new SimpleResult<T>(error);
}