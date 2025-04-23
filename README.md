# WeatherSync

WeatherSync is a console application designed to fetch current weather data and insert it into a database. It will later support weather forecasts as well.

# Project Overview

1. Application Startup
   ```text
   - The program starts execution from Program.cs.
   - Dependency Injection is used to register necessary services.
   - The application reads a list of three cities from appsettings.json.
   ```
2. Fetching Weather Data
   ```text
   - The program loops through each city and calls the WeatherProcess class.
   - The WeatherProcess class:
    - Calls an external API to fetch weather data.
    - Uses models to structure and organize the data.
    - Passes the data to the WeatherRepository for storage.
   ```
3. Storing Data in the Database
   ```text
   - The WeatherRepository prepares the API response for database storage.
   - It interacts with the DbContext and executes a stored procedure to insert data.
   - SQL queries are executed using the SQL Client.
   ```

# Project Structure

```
WeatherSyncSolution/  # Root folder containing the solution file
│
├── README.md  # Project documentation
├── WeatherSync/  # WeatherSync project folder
│   ├── bin/  # Compiled binaries and build artifacts
│   │   └── Debug/
│   │       └── net8.0/
│   ├── BusinessLogic/  # Contains core business logic
│   │   └── WeatherProcessor.cs
│   ├── Data/  # Database-related files
│   │   ├── Scripts/  # SQL scripts for database setup
│   │   │   ├── StoredProcedures/  # Stored procedure files
│   │   │   │   ├── prcInsertWeatherData.sql
│   │   │   │   └── WeatherData.sql
│   │   │   ├── Tables/  # Table creation scripts (if any)
│   │   │   └── WeatherDbContext.cs
│   │   └── WeatherDbContext.cs  # DB Context for database interactions
│   ├── Models/  # Data models for API responses and database interactions
│   │   ├── CityModel.cs
│   │   ├── CloudsModel.cs
│   │   ├── CoordinateModel.cs
│   │   ├── CurrentWeatherResponseModel.cs
│   │   ├── MainModel.cs
│   │   ├── SysModel.cs
│   │   ├── WeatherModel.cs
│   │   └── WindModel.cs
│   ├── Repositories/  # Handles database interactions
│   │   ├── IWeatherRepository.cs
│   │   └── WeatherRepository.cs
│   ├── Services/  # Handles API communication
│   │   ├── IWeatherApiClient.cs
│   │   └── WeatherApiClient.cs
│   ├── obj/  # Temporary build files (auto-generated)
│   ├── appsettings.json  # Configuration file (e.g., cities, database connection)
│   ├── Program.cs  # Main entry point of the application
│   ├── WeatherSync.csproj  # Project file
├── WeatherSyncSolution.sln  # Solution file (in the root folder)
```

# Dependencies

The following NuGet packages are required to run this project:

- **Microsoft.Data.SqlClient** (6.0.1)
  - Provides data access to SQL Server databases, allowing your application to connect to and interact with SQL databases.
- **Microsoft.Extensions.Configuration** (9.0.2)
  - Allows reading configuration settings from various sources such as JSON, environment variables, and command-line arguments.
- **Microsoft.Extensions.Configuration.Binder** (9.0.2)
  - Provides functionality to bind configuration values to strongly-typed objects.
- **Microsoft.Extensions.Configuration.Json** (9.0.2)
  - Adds support for reading configuration data from JSON files, such as `appsettings.json`.
- **Microsoft.Extensions.DependencyInjection** (9.0.2)
  - Provides dependency injection functionality for managing object lifetimes and dependencies in your application.
- **Microsoft.Extensions.Hosting** (9.0.2)
  - Provides APIs to create and manage application hosting, particularly useful for console applications and background services.
- **Microsoft.Extensions.Http** (9.0.2)
  - Adds functionality for making HTTP requests, enabling the app to communicate with external APIs.
- **Newtonsoft.Json** (13.0.3)
  - Provides high-performance JSON serialization and deserialization functionality, useful for handling JSON data.
- **System.Data.SqlClient** (4.9.0)
  - Another library for data access to SQL Server databases, offering alternative functionality to `Microsoft.Data.SqlClient`.
- **Polly** (7.2.3)
  - Provides resilience and transient-fault-handling capabilities, such as retries, circuit breakers, and rate limiting.
- **Polly.Contrib.RateLimit** (1.0.0)
  - Extends Polly with rate-limiting capabilities to control the number of API calls made within a specified period.
- **StackExchange.Redis** (2.6.111)
  - A high-performance Redis client for .NET, used for caching, rate limiting, and distributed data management.

## Installing Dependencies & Running the Application

To install the required dependencies for this project, follow these steps:

1. Ensure you have the [.NET SDK](https://dotnet.microsoft.com/download/dotnet) installed.
2. Open a terminal or command prompt and clone the repo in your desired location.
   ```bash
   git clone https://github.com/jtombo45/WeatherSyncSolution.git
   ```
3. Navigate to the project folder where `WeatherSync.csproj` is located.
4. Modify the `ConnectionStrings` in `appsettings.json` to point to your server and database. Ensure that you're using the appropriate database credentials.
5. Navigate to `Data/Scripts/` folder and verify that the table(s) and stored procedure(s) exist in your database if not create them.
6. Navigate to the project root and run the following commands to install the required NuGet packages:
   ```bash
   dotnet add package Microsoft.Data.SqlClient --version 6.0.1
   dotnet add package Microsoft.Extensions.Configuration --version 9.0.2
   dotnet add package Microsoft.Extensions.Configuration.Binder --version 9.0.2
   dotnet add package Microsoft.Extensions.Configuration.Json --version 9.0.2
   dotnet add package Microsoft.Extensions.DependencyInjection --version 9.0.2
   dotnet add package Microsoft.Extensions.Hosting --version 9.0.2
   dotnet add package Microsoft.Extensions.Http --version 9.0.2
   dotnet add package Newtonsoft.Json --version 13.0.3
   dotnet add package System.Data.SqlClient --version 4.9.0
   dotnet add package Polly
   dotnet add package Polly.Contrib.RateLimit
   dotnet add package StackExchange.Redis
   dotnet add package Serilog
   dotnet add package Serilog.Sinks.Console
   dotnet add package Serilog.Sinks.File
   dotnet add package Serilog.Extensions.Logging
   dotnet add package Serilog.Settings.Configuration
   dotnet add package Serilog.Extensions.Hosting
   dotnet add WeatherSync.Tests package Moq
   dotnet add WeatherSync.Tests package Microsoft.Extensions.DependencyInjection
   dotnet add WeatherSync.Tests package Microsoft.Extensions.Logging.Abstractions
   dotnet add WeatherSync.Tests package Microsoft.NET.Test.Sdk
   dotnet add WeatherSync.Tests package xunit
   dotnet add WeatherSync.Tests package FluentAssertions
   ```
7. Install redis and start the redis server ([instructions](https://redis.io/docs/latest/operate/oss_and_stack/install/install-redis/))
8. Run the following command to restore the dependencies:
   ```bash
   dotnet restore
   ```
9. Run the following command to build the project
   ```bash
   dotnet build
   ```
10. Run the following command to run the application

```bash
dotnet run
```
