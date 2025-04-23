using System;
using System.Data;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WeatherSync.Repositories;
using WeatherSync.Models;

namespace WeatherSync.Tests.Repositories
{
    public class WeatherRepositoryTests
    {
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly Mock<IDbCommand> _dbCommandMock;
        private readonly Mock<ILogger<WeatherRepository>> _loggerMock;
        private readonly WeatherRepository _repository;

        public WeatherRepositoryTests()
        {
            _dbConnectionMock = new Mock<IDbConnection>();
            _dbCommandMock = new Mock<IDbCommand>();
            _loggerMock = new Mock<ILogger<WeatherRepository>>();

            // Ensure `CreateCommand()` returns a valid mock
            _dbConnectionMock.Setup(c => c.CreateCommand()).Returns(_dbCommandMock.Object);
            _dbCommandMock.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

            // Fix: Ensure `CreateParameter()` returns a new instance for each call
            _dbCommandMock.Setup(cmd => cmd.CreateParameter()).Returns(() =>
            {
                var paramMock = new Mock<IDbDataParameter>();
                paramMock.SetupAllProperties(); // Ensures the mock behaves like a real parameter
                return paramMock.Object;
            });

            _repository = new WeatherRepository(_dbConnectionMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SaveWeatherDataAsync_ShouldCallStoredProcedure()
        {
            // Arrange
            var weatherData = new CurrentWeatherResponseModel
            {
                Name = "New York",
                Coord = new CoordinateModel { Lon = -74.006, Lat = 40.7128 },
                Main = new MainModel { Temp = 75, FeelsLike = 72, TempMin = 70, TempMax = 80, Pressure = 1015, Humidity = 60 },
                Weather = new List<WeatherModel> { new WeatherModel { Main = "Clear", Description = "Clear Sky" } }
            };

            // Ensure `Parameters.Add` is properly handled
            var parameterCollectionMock = new Mock<IDataParameterCollection>();
            _dbCommandMock.Setup(cmd => cmd.Parameters).Returns(parameterCollectionMock.Object);

            // Act
            await _repository.SaveWeatherDataAsync(weatherData);

            // Assert
            _dbCommandMock.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once); // Ensure stored procedure was executed
        }
    }
}
