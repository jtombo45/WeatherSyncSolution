using System;
using System.Threading.Tasks;
using WeatherSync.Models;
using WeatherSync.Services;
using WeatherSync.Repositories;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Wrap;
using Serilog;

namespace WeatherSync.BusinessLogic
{
    public class WeatherProcessor
    {
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly IWeatherRepository _weatherRepository;
        private readonly IRedisService _redisService;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly AsyncPolicyWrap _policyWrap;  // using AsyncPolicyWrap

        public WeatherProcessor(IWeatherApiClient weatherApiClient, IWeatherRepository weatherRepository, IRedisService redisService)
        {
            _weatherApiClient = weatherApiClient;
            _weatherRepository = weatherRepository;
            _redisService = redisService;

            // Define an async retry policy (retry up to 3 times with exponential backoff)
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning($"Retry {retryCount} after {timeSpan.Seconds} seconds due to: {exception.Message}");
                    });

            // Define an async circuit breaker policy (open circuit if 5 failures occur within 60 seconds)
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                    onBreak: (exception, timespan) =>
                    {
                        Log.Error($"Circuit broken due to: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Log.Information("Circuit reset.");
                    });

            // Combine policies with AsyncPolicyWrap (use `.WrapAsync()`)
            _policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
        }

        public async Task GetWeatherAsync(CityModel city)
        {
            Log.Information($"Fetching weather data for {city.Name}...");

            // Check rate limit before making API call
            if (await _redisService.IsRateLimitExceededAsync())
            {
                Log.Warning("Rate limit exceeded. Try again tomorrow.");
                return;
            }

            try
            {
                // Execute API call with retry and circuit breaker (Async)
                var weatherData = await _policyWrap.ExecuteAsync(() => _weatherApiClient.GetWeatherAsync(city));

                // Log weather data
                Log.Information($"City: {weatherData.Name}: Temperature: {weatherData.Main.Temp}°F: Description: {weatherData.Weather[0].Description}");

                // Save to database
                await _weatherRepository.SaveWeatherDataAsync(weatherData);

                // Increment API request count
                await _redisService.IncrementRequestCountAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to fetch weather data: {ex.Message}");
            }
        }
    }
}
