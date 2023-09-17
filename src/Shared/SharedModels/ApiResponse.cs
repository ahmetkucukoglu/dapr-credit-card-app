namespace SharedModels;

public class ApiResponse
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static ApiResponse CreateFail(string message)
    {
        return new ApiResponse {IsSuccess = false, ErrorMessage = message};
    }
}

public class ApiResponse<T> : ApiResponse
{
    public required T Data { get; init; }

    public static ApiResponse<T> CreateSuccess(T data)
    {
        return new ApiResponse<T> {IsSuccess = true, Data = data};
    }
}