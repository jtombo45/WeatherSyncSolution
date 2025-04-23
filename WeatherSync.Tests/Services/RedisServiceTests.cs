using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using WeatherSync.Services;

namespace WeatherSync.Tests.Services
{
    public class RedisServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisService _redisService;

        public RedisServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();

            _configurationMock.Setup(c => c["Redis:ConnectionString"]).Returns("localhost:6379"); // ✅ Ensure a non-null connection string
            _configurationMock.Setup(c => c["Redis:Keys:WeatherApi"]).Returns("weatherapi:request_count");
            _configurationMock.Setup(c => c["RateLimit:MaxRequestsPerDay"]).Returns("1000"); // ✅ Ensure a valid number

            _redisMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                      .Returns(_databaseMock.Object);

            // Inject mock Redis connection into RedisService
            _redisService = new RedisService(_configurationMock.Object, _redisMock.Object);
        }

        [Fact]
        public async Task IsRateLimitExceededAsync_ShouldReturnTrue_WhenLimitReached()
        {
            // Arrange: Simulate hitting the request limit
            _databaseMock.Setup(db => db.StringGetAsync("weatherapi:request_count", CommandFlags.None))
                         .ReturnsAsync(1000);

            // Act
            var result = await _redisService.IsRateLimitExceededAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsRateLimitExceededAsync_ShouldReturnFalse_WhenBelowLimit()
        {
            // Arrange: Simulate requests below limit
            _databaseMock.Setup(db => db.StringGetAsync("weatherapi:request_count", CommandFlags.None))
                         .ReturnsAsync(999);

            // Act
            var result = await _redisService.IsRateLimitExceededAsync();

            // Assert
            result.Should().BeFalse();
        }
    }
}
