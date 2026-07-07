namespace HomeLibrary.Contracts.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public List<string> Errors { get; set; } = new();

    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(params string[] errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors.ToList()
        };
    }
}