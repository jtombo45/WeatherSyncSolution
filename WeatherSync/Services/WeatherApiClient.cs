using WeatherSync.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WeatherSync.Services
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _units;

        public WeatherApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApiKey"];
            _baseUrl = configuration["WeatherApiBaseUrl"];
            _units = configuration["WeatherApiUnits"];
        }

        public async Task<CurrentWeatherResponseModel> GetWeatherAsync(CityModel city)
        {
            try
            {
                // Create WeatherResponse object
                var weatherResponse = new CurrentWeatherResponseModel();

                // Send request to the weather API
                var url = $"{_baseUrl}?lat={city.Lat}&lon={city.Lon}&units={_units}&appid={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response into a CurrentWeatherResponse object
                var jsonResponse = await response.Content.ReadAsStringAsync();
                weatherResponse = JsonConvert.DeserializeObject<CurrentWeatherResponseModel>(jsonResponse);

                return weatherResponse;
            }
            catch (Exception ex)
            {
                Log.Error($"[ERROR] Failed to get weather data: {ex.Message}");
                return null;
            }
        }
    }
}