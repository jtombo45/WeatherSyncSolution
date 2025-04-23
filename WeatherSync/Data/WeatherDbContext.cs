using Microsoft.Extensions.Configuration;

namespace WeatherSync.Data
{
    public class WeatherDbContext
    {
        public string ConnectionString { get; }

        public WeatherDbContext(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
