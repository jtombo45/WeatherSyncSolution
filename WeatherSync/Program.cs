using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using WeatherSync.BusinessLogic;
using WeatherSync.Services;
using WeatherSync.Repositories;
using WeatherSync.Models;

namespace WeatherSync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Ensure the logs directory exists
            string logDirectory = "logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Initialize Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/weatherapp.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Application Starting...");

                // Build the host
                var host = CreateHostBuilder(args).Build();

                // Resolve necessary services
                using var scope = host.Services.CreateScope(); // Use scope to handle DI properly
                var services = scope.ServiceProvider;

                var weatherProcessor = services.GetRequiredService<WeatherProcessor>();
                var configuration = services.GetRequiredService<IConfiguration>();

                // Get list of cities from configuration
                var cities = configuration.GetSection("Cities").Get<List<CityModel>>();

                if (cities == null || cities.Count == 0)
                {
                    Log.Warning("No cities found in configuration. Exiting.");
                    return;
                }

                // Process weather data for each city
                foreach (var city in cities)
                {
                    await weatherProcessor.GetWeatherAsync(city);
                }

                Log.Information("Weather processing complete.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occurred while processing weather data.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .UseSerilog() // Ensure Serilog is properly integrated
                .ConfigureServices((context, services) =>
                {
                    // Register SQL Connection
                    services.AddSingleton<IDbConnection>(sp =>
                        new SqlConnection(context.Configuration.GetConnectionString("DefaultConnection")));

                    // Register other services
                    services.AddHttpClient<IWeatherApiClient, WeatherApiClient>();
                    services.AddSingleton<IWeatherRepository, WeatherRepository>();
                    services.AddSingleton<IRedisService, RedisService>();
                    services.AddSingleton<WeatherProcessor>();

                    // Register logging with Serilog
                    services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
                });
    }
}
