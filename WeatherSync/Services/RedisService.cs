using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Serilog;
using WeatherSync.Services;
using WeatherSync.Models;

namespace WeatherSync.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly string _weatherApiKey;
        private readonly int _maxRequestsPerDay;

        public RedisService(IConfiguration configuration, IConnectionMultiplexer? redisMultiplexer = null)
        {
            var redisConnectionString = configuration["Redis:ConnectionString"];

            // Allow using a mock Redis connection in tests
            var redis = redisMultiplexer ?? ConnectionMultiplexer.Connect(redisConnectionString);
            _database = redis.GetDatabase();

            _weatherApiKey = configuration["Redis:Keys:WeatherApi"];
            _maxRequestsPerDay = int.Parse(configuration["RateLimit:MaxRequestsPerDay"]);
        }

        // Check if rate limit has been exceeded
        public async Task<bool> IsRateLimitExceededAsync()
        {
            var requestCount = await _database.StringGetAsync(_weatherApiKey);
            int count = requestCount.HasValue ? (int)requestCount : 0;

            Log.Information($"Rate limit check: {count} / {_maxRequestsPerDay} request used.");

            return count >= _maxRequestsPerDay;
        }

        // Increment the API request count
        public async Task IncrementRequestCountAsync()
        {
            await _database.StringIncrementAsync(_weatherApiKey);
            Log.Information("Incremented API request count.");

            // Ensure the count resets after 24 hours
            if (!await _database.KeyExistsAsync(_weatherApiKey))
            {
                await _database.KeyExpireAsync(_weatherApiKey, TimeSpan.FromDays(1));
                Log.Information("Set Redis key expiration to 24 hours.");
            }

        }

        // Set the API request count
        public async Task SetRequestCountAsync(int count)
        {
            await _database.StringSetAsync(_weatherApiKey, count);
        }

        // Get the current API request count
        public async Task<int> GetRequestCountAsync()
        {
            var requestCount = await _database.StringGetAsync(_weatherApiKey);
            return requestCount.HasValue ? (int)requestCount : 0;
        }

        // Reset the API request count
        public async Task ResetRequestCountAsync()
        {
            await _database.StringSetAsync(_weatherApiKey, 0, TimeSpan.FromDays(1));
        }
    }
}
