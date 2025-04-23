using Newtonsoft.Json;

namespace WeatherSync.Models
{
    public class CoordinateModel
    {
        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }
}