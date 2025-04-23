using Newtonsoft.Json;
namespace WeatherSync.Models
{
    public class CurrentWeatherResponseModel
    {
        [JsonProperty("coord")]
        public CoordinateModel Coord { get; set; }

        [JsonProperty("weather")]
        public List<WeatherModel> Weather { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("main")]
        public MainModel Main { get; set; }

        [JsonProperty("visibility")]
        public int Visibility { get; set; }

        [JsonProperty("wind")]
        public WindModel Wind { get; set; }

        [JsonProperty("clouds")]
        public CloudsModel Clouds { get; set; }

        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonProperty("sys")]
        public SysModel Sys { get; set; }

        [JsonProperty("timezone")]
        public int Timezone { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cod")]
        public int Cod { get; set; }

    }
}