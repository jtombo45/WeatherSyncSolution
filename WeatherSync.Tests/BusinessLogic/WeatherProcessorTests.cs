using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using WeatherSync.BusinessLogic;
using WeatherSync.Models;
using WeatherSync.Repositories;
using WeatherSync.Services;

namespace WeatherSync.Tests.BusinessLogic
{
    public class WeatherProcessorTests
    {
        private readonly Mock<IWeatherApiClient> _weatherApiClientMock;
        private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
        private readonly Mock<IRedisService> _redisServiceMock;
        private readonly WeatherProcessor _weatherProcessor;

        public WeatherProcessorTests()
        {
            _weatherApiClientMock = new Mock<IWeatherApiClient>();
            _weatherRepositoryMock = new Mock<IWeatherRepository>();
            _redisServiceMock = new Mock<IRedisService>();

            // Inject mocks into WeatherProcessor
            _weatherProcessor = new WeatherProcessor(
                _weatherApiClientMock.Object,
                _weatherRepositoryMock.Object,
                _redisServiceMock.Object);
        }

        [Fact]
        public async Task GetWeatherAsync_CallsApiAndSavesToRepository()
        {
            // Arrange
            var city = new CityModel { Name = "Winslow", Lon = -110.6974, Lat = 35.0242 };
            var mockWeatherData = new CurrentWeatherResponseModel
            {
                Name = "Winslow",
                Coord = new CoordinateModel { Lon = -110.6974, Lat = 35.0242 },
                Main = new MainModel
                {
                    Temp = 48.38,
                    FeelsLike = 44.76,
                    TempMin = 48.38,
                    TempMax = 48.38,
                    Pressure = 1019,
                    Humidity = 31
                },
                Weather = new List<WeatherModel>
                {
                    new() { Main = "Clear", Description = "clear sky" }
                }
            };

            _redisServiceMock.Setup(r => r.IsRateLimitExceededAsync()).ReturnsAsync(false);
            _weatherApiClientMock.Setup(api => api.GetWeatherAsync(It.IsAny<CityModel>())).ReturnsAsync(mockWeatherData);
            _weatherRepositoryMock.Setup(repo => repo.SaveWeatherDataAsync(It.IsAny<CurrentWeatherResponseModel>())).Returns(Task.CompletedTask);

            // Act
            await _weatherProcessor.GetWeatherAsync(city);

            // Assert
            _weatherApiClientMock.Verify(api => api.GetWeatherAsync(It.IsAny<CityModel>()), Times.Once);
            _weatherRepositoryMock.Verify(repo => repo.SaveWeatherDataAsync(It.IsAny<CurrentWeatherResponseModel>()), Times.Once);
        }

        [Fact]
        public async Task GetWeatherAsync_RateLimitExceeded_DoesNotCallApi()
        {
            // Arrange
            var city = new CityModel { Name = "Austin", Lat = 30.2672, Lon = -97.7431 };
            _redisServiceMock.Setup(r => r.IsRateLimitExceededAsync()).ReturnsAsync(true);

            // Act
            await _weatherProcessor.GetWeatherAsync(city);

            // Assert
            _weatherApiClientMock.Verify(api => api.GetWeatherAsync(It.IsAny<CityModel>()), Times.Never);
            _weatherRepositoryMock.Verify(repo => repo.SaveWeatherDataAsync(It.IsAny<CurrentWeatherResponseModel>()), Times.Never);
        }

        [Fact]
        public async Task GetWeatherAsync_ApiCallFails_Gracefully()
        {
            // Arrange
            var city = new CityModel { Name = "Los Angeles", Lat = 34.0522, Lon = -118.2437 };
            _redisServiceMock.Setup(r => r.IsRateLimitExceededAsync()).ReturnsAsync(false);

            // Simulate API failure
            _weatherApiClientMock.Setup(api => api.GetWeatherAsync(It.IsAny<CityModel>()))
                                 .ThrowsAsync(new Exception("API call failed"));

            // Act & Assert
            await FluentActions.Invoking(() => _weatherProcessor.GetWeatherAsync(city))
                               .Should().NotThrowAsync(); // Ensure no unhandled exception crashes the test

            // Ensure no DB save attempt is made
            _weatherRepositoryMock.Verify(repo => repo.SaveWeatherDataAsync(It.IsAny<CurrentWeatherResponseModel>()), Times.Never);
        }

        [Fact]
        public async Task GetWeatherAsync_DatabaseSaveFails_HandlesErrorGracefully()
        {
            // Arrange
            var city = new CityModel { Name = "Seattle", Lat = 47.6062, Lon = -122.3321 };
            var mockWeatherData = new CurrentWeatherResponseModel
            {
                Name = "Seattle",
                Coord = new CoordinateModel { Lon = -122.3321, Lat = 47.6062 },
                Main = new MainModel
                {
                    Temp = 55.4,
                    FeelsLike = 53.2,
                    TempMin = 50.1,
                    TempMax = 60.2,
                    Pressure = 1015,
                    Humidity = 80
                },
                Weather = new List<WeatherModel>
                {
                    new() { Main = "Cloudy", Description = "overcast clouds" }
                }
            };

            _redisServiceMock.Setup(r => r.IsRateLimitExceededAsync()).ReturnsAsync(false);
            _weatherApiClientMock.Setup(api => api.GetWeatherAsync(It.IsAny<CityModel>())).ReturnsAsync(mockWeatherData);

            // Simulate database save failure
            _weatherRepositoryMock.Setup(repo => repo.SaveWeatherDataAsync(It.IsAny<CurrentWeatherResponseModel>()))
                                  .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await FluentActions.Invoking(() => _weatherProcessor.GetWeatherAsync(city))
                               .Should().NotThrowAsync(); // Ensure no unhandled exception crashes the test
        }
    }
}
