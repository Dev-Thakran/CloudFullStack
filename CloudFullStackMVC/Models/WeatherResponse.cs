using Newtonsoft.Json;

namespace CloudFullStackMVC.Models
{
    public class WeatherResponse
    {
        [JsonProperty("current")]
        public CurrentWeather? Current { get; set; }
    }

    public class CurrentWeather
    {
        [JsonProperty("temperature_2m")]
        public double Temperature { get; set; }

        [JsonProperty("precipitation")]
        public double Precipitation { get; set; }

        [JsonProperty("weather_code")]
        public int WeatherCode { get; set; }

        [JsonProperty("wind_speed_10m")]
        public double WindSpeed { get; set; }

        // Turns Open-Meteo's number code into words
        public string Description => WeatherCode switch
        {
            0 => "Clear sky",
            1 or 2 => "Partly cloudy",
            3 => "Overcast",
            45 or 48 => "Fog",
            >= 51 and <= 67 => "Rain",
            >= 71 and <= 77 => "Snow",
            >= 80 and <= 82 => "Showers",
            >= 95 => "Thunderstorm",
            _ => "Unknown"
        };
    }
}