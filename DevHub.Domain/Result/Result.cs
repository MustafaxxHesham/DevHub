namespace DevHub.Domain.Result;
public class Result<T> where T : class 
{
    public bool IsSuccess { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }
    private Result(T value)
    {
        Error = string.Empty;
        Value = value;
        IsSuccess = true;
    }
    private Result(string error)
    {
        Error = error;
        Value = null;
        IsSuccess = false;
    }
    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(string error) => new Result<T>(error);
}
