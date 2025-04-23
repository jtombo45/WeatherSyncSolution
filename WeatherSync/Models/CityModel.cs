using Newtonsoft.Json;
namespace WeatherSync.Models
{
    public class CityModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Lon")]
        public double Lon { get; set; }
        [JsonProperty("Lat")]
        public double Lat { get; set; }
    }
}