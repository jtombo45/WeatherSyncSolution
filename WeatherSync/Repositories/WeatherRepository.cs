using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WeatherSync.Models;

namespace WeatherSync.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<WeatherRepository> _logger;

        public WeatherRepository(IDbConnection dbConnection, ILogger<WeatherRepository> logger)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveWeatherDataAsync(CurrentWeatherResponseModel weatherData)
        {
            if (weatherData == null)
                throw new ArgumentNullException(nameof(weatherData), "Weather data cannot be null");

            try
            {
                using var conn = _dbConnection;
                conn.Open();

                using var cmd = conn.CreateCommand();

                // ✅ Check if connection is SQL Server or SQLite
                if (conn is SqlConnection)
                {
                    // ✅ Use Stored Procedure for SQL Server
                    cmd.CommandText = "prcInsertWeatherData";
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    // ✅ Use raw SQL query for SQLite (for unit testing)
                    cmd.CommandText = "INSERT INTO WeatherData (CityName, Longitude, Latitude, Main, Description, Temperature, FeelsLike, MinTemperature, MaxTemperature, Pressure, Humidity, RecordedAt) " +
                                      "VALUES (@CityName, @Longitude, @Latitude, @Main, @Description, @Temperature, @FeelsLike, @MinTemperature, @MaxTemperature, @Pressure, @Humidity, @RecordedAt)";
                    cmd.CommandType = CommandType.Text;
                }

                // ✅ Add parameters
                AddParameter(cmd, "@CityName", weatherData.Name);
                AddParameter(cmd, "@Longitude", weatherData.Coord.Lon);
                AddParameter(cmd, "@Latitude", weatherData.Coord.Lat);
                AddParameter(cmd, "@Main", weatherData.Weather[0].Main);
                AddParameter(cmd, "@Description", weatherData.Weather[0].Description);
                AddParameter(cmd, "@Temperature", weatherData.Main.Temp);
                AddParameter(cmd, "@FeelsLike", weatherData.Main.FeelsLike);
                AddParameter(cmd, "@MinTemperature", weatherData.Main.TempMin);
                AddParameter(cmd, "@MaxTemperature", weatherData.Main.TempMax);
                AddParameter(cmd, "@Pressure", weatherData.Main.Pressure);
                AddParameter(cmd, "@Humidity", weatherData.Main.Humidity);
                AddParameter(cmd, "@RecordedAt", DateTime.UtcNow);

                cmd.ExecuteNonQuery(); // ✅ Works for both SQL Server & SQLite

                _logger.LogInformation("Weather data for {City} saved successfully.", weatherData.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert weather data for {City}", weatherData?.Name);
                throw;
            }
        }

        private void AddParameter(IDbCommand cmd, string paramName, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }
    }
}
