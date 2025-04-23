using Newtonsoft.Json;

namespace WeatherSync.Models
{
    public class CloudsModel
    {
        [JsonProperty("all")]
        public int All { get; set; }
    }
}