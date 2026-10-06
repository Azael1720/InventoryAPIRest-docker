namespace InventoryAPIRest.Web.Infrastructure.Api
{
    public sealed record ApiError(int Status, string Message, IReadOnlyList<string>? Details = null);
    public class ApiResult
    {
        public bool Success => Error is null;
        public ApiError? Error { get; protected init; }
    }
    public sealed class ApiResult<T> : ApiResult
    {
        public T? Value { get; private init; }

        public static ApiResult<T> Ok(T value) => new() { Value = value };
        public static ApiResult<T> Fail(ApiError error) => new() { Error = error };
    }
}
