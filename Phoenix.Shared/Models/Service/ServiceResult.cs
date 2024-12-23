using System.Net;

namespace Phoenix.Shared.Models.Service
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public required T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public HttpStatusCode ErrorCode { get; set; }

        public static ServiceResult<T?> SuccessResult(T data) => new() { ErrorCode = HttpStatusCode.OK, Success = true, Data = data };
        public static ServiceResult<T?> SuccessResult(T data, HttpStatusCode httpStatusCode) => new() { ErrorCode = httpStatusCode, Success = true, Data = data };
        public static ServiceResult<T?> FailureResult(string errorMessage, HttpStatusCode errorCode = HttpStatusCode.BadRequest) => new() { Data = default, Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }
}
