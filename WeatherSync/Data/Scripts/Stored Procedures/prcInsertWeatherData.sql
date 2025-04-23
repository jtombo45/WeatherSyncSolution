SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcInsertWeatherData]
    @CityName NVARCHAR(100),
    @Longitude DECIMAL(9,6),
    @Latitude DECIMAL(9,6),
    @Main NVARCHAR(100),
    @Description NVARCHAR(255),
    @Temperature FLOAT,
    @FeelsLike FLOAT,
    @MinTemperature FLOAT,
    @MaxTemperature FLOAT,
    @Pressure INT,
    @Humidity INT,
    @RecordedAt DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for existing record with the same city and recorded date (ignoring time)
    IF NOT EXISTS (
        SELECT 1
    FROM WeatherData
    WHERE CityName = @CityName
        AND RecordedAt = @RecordedAt
    )
    BEGIN
        -- Insert new record if no duplicate exists
        INSERT INTO WeatherData
            (CityName, Longitude, Latitude, Main, Description, Temperature, FeelsLike, MinTemperature, MaxTemperature, Pressure, Humidity, RecordedAt)
        VALUES
            (@CityName, @Longitude, @Latitude, @Main, @Description, @Temperature, @FeelsLike, @MinTemperature, @MaxTemperature, @Pressure, @Humidity, @RecordedAt);
    END
    ELSE
    BEGIN
        PRINT 'Duplicate entry detected for ' + @CityName + ' on ' + CAST(@RecordedAt AS VARCHAR);
    END
END;
GO
