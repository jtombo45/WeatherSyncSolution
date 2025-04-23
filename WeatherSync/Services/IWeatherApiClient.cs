using System.Threading.Tasks;
using WeatherSync.Models;

namespace WeatherSync.Services
{
    public interface IWeatherApiClient
    {
        // <summary>    
        // Get the current weather for a city
        // </summary>
        // <returns> CurrentWeatherResponseModel</returns>
        Task<CurrentWeatherResponseModel> GetWeatherAsync(CityModel city);
    }
}