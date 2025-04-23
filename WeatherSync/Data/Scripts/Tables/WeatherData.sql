CREATE TABLE WeatherData
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CityName NVARCHAR(100) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL,
    Main NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Temperature FLOAT NOT NULL,
    FeelsLike FLOAT NOT NULL,
    MinTemperature FLOAT NOT NULL,
    MaxTemperature FLOAT NOT NULL,
    Pressure INT NOT NULL,
    Humidity INT NOT NULL,
    RecordedAt DATE NOT NULL
);
