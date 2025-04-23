using System.Threading.Tasks;
using WeatherSync.Models;

namespace WeatherSync.Repositories
{
    public interface IWeatherRepository
    {
        Task SaveWeatherDataAsync(CurrentWeatherResponseModel weatherData);
    }
}
