namespace WeatherSync.Services
{
    public interface IRedisService
    {
        /// <summary>
        /// Checks if the rate limit has been exceeded for the API.
        /// </summary>
        /// <returns>True if the request limit is reached; otherwise, false.</returns>
        Task<bool> IsRateLimitExceededAsync();

        /// <summary>
        /// Increments the API request count in Redis.
        /// </summary>
        Task IncrementRequestCountAsync();

        /// <summary>
        /// Gets the current API request count stored in Redis.
        /// </summary>
        /// <returns>The current count of API requests made.</returns>
        Task<int> GetRequestCountAsync();

        /// <summary>
        /// Resets the API request count manually if needed (optional).
        /// </summary>
        Task ResetRequestCountAsync();

        /// <summary>
        /// Sets the API request count to a specific value.
        /// </summary>
        Task SetRequestCountAsync(int count);
    }
}