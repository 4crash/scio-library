namespace Scio.Models
{
    /// <summary>
    /// Enum representing different types of API errors
    /// </summary>
    public enum ApiErrorType
    {
        Success = 0,
        NetworkError = 1,
        HttpError = 2,
        Timeout = 3,
        ValidationError = 4,
        ServerError = 5
    }

    /// <summary>
    /// Generic result class for API operations returning data
    /// </summary>
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public ApiErrorType ErrorType { get; set; }

        // Static factory methods
        public static ApiResult<T> SuccessResult(T data)
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                ErrorType = ApiErrorType.Success
            };
        }

        public static ApiResult<T> FailureResult(string errorMessage, ApiErrorType errorType)
        {
            return new ApiResult<T>
            {
                Success = false,
                Data = default,
                ErrorMessage = errorMessage,
                ErrorType = errorType
            };
        }
    }

    /// <summary>
    /// Result class for API operations without return data (mutations)
    /// </summary>
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public ApiErrorType ErrorType { get; set; }

        // Static factory methods
        public static ApiResult SuccessResult()
        {
            return new ApiResult
            {
                Success = true,
                ErrorType = ApiErrorType.Success
            };
        }

        public static ApiResult FailureResult(string errorMessage, ApiErrorType errorType)
        {
            return new ApiResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorType = errorType
            };
        }
    }
}
