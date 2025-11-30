using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    /// <summary>
    /// Weather data model from OpenWeatherMap API
    /// </summary>
    public class WeatherData
    {
        public string Description { get; set; } = "";
        public double Temperature { get; set; } // Celsius
        public double FeelsLike { get; set; } // Celsius
        public double WindSpeed { get; set; } // meter/second
        public double WindSpeedKnots => WindSpeed * 1.94384; // Convert to knots
        public int Humidity { get; set; } // percentage
        public int Visibility { get; set; } // meters
        public double Pressure { get; set; } // hPa
        public string Icon { get; set; } = "";
        public DateTime UpdatedAt { get; set; }

        // Wave height estimation (simplified - based on wind speed)
        public double EstimatedWaveHeight => WindSpeed * 0.3; // Very rough estimate

        /// <summary>
        /// Check if weather is safe for docking operations
        /// </summary>
        public bool IsSafeForDocking()
        {
            return WindSpeedKnots < ApiConfiguration.MaxSafeWindSpeed &&
                   Visibility >= ApiConfiguration.MinSafeVisibility &&
                   EstimatedWaveHeight < ApiConfiguration.MaxSafeWaveHeight;
        }

        /// <summary>
        /// Get weather status emoji
        /// </summary>
        public string GetStatusEmoji()
        {
            if (Description.Contains("clear")) return "☀️";
            if (Description.Contains("cloud")) return "☁️";
            if (Description.Contains("rain")) return "🌧️";
            if (Description.Contains("storm")) return "⛈️";
            if (Description.Contains("snow")) return "❄️";
            if (Description.Contains("fog") || Description.Contains("mist")) return "🌫️";
            return "🌤️";
        }

        /// <summary>
        /// Get safety status message
        /// </summary>
        public string GetSafetyStatus()
        {
            if (IsSafeForDocking())
                return "✅ Safe for docking";

            var reasons = "";
            if (WindSpeedKnots >= ApiConfiguration.MaxSafeWindSpeed)
                reasons += $"⚠️ High wind speed ({WindSpeedKnots:F1} knots)\n";
            if (Visibility < ApiConfiguration.MinSafeVisibility)
                reasons += $"⚠️ Low visibility ({Visibility}m)\n";
            if (EstimatedWaveHeight >= ApiConfiguration.MaxSafeWaveHeight)
                reasons += $"⚠️ High waves (~{EstimatedWaveHeight:F1}m)\n";

            return "❌ UNSAFE - " + reasons.Trim();
        }
    }

    /// <summary>
    /// Service for fetching weather data from OpenWeatherMap API
    /// Used for harbor safety assessment
    /// </summary>
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Get current weather at harbor location
        /// </summary>
        public async Task<WeatherData?> GetHarborWeatherAsync()
        {
            try
            {
                // Check if API key is configured
                if (string.IsNullOrEmpty(ApiConfiguration.OpenWeatherApiKey) ||
                    ApiConfiguration.OpenWeatherApiKey.Contains("YOUR_"))
                {
                    throw new Exception("OpenWeather API key not configured. Please update ApiConfiguration.cs");
                }

                // Build API URL with coordinates
                var url = $"{BaseUrl}?" +
                          $"lat={ApiConfiguration.HarborLatitude}&" +
                          $"lon={ApiConfiguration.HarborLongitude}&" +
                          $"appid={ApiConfiguration.OpenWeatherApiKey}&" +
                          $"units=metric"; // Celsius

                // Fetch weather data
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Weather API error: {response.StatusCode} - {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<OpenWeatherResponse>(json);

                if (apiResponse == null)
                    throw new Exception("Failed to parse weather data");

                // Map to our WeatherData model
                var weatherData = new WeatherData
                {
                    Description = apiResponse.Weather[0].Description,
                    Temperature = apiResponse.Main.Temp,
                    FeelsLike = apiResponse.Main.FeelsLike,
                    WindSpeed = apiResponse.Wind.Speed,
                    Humidity = apiResponse.Main.Humidity,
                    Visibility = apiResponse.Visibility,
                    Pressure = apiResponse.Main.Pressure,
                    Icon = apiResponse.Weather[0].Icon,
                    UpdatedAt = DateTime.Now
                };

                return weatherData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch weather data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if current weather is safe for docking
        /// </summary>
        public async Task<bool> IsCurrentWeatherSafeAsync()
        {
            try
            {
                var weather = await GetHarborWeatherAsync();
                return weather?.IsSafeForDocking() ?? false;
            }
            catch
            {
                // If we can't fetch weather, assume it's safe (don't block operations)
                return true;
            }
        }

        /// <summary>
        /// Get weather summary string for display
        /// </summary>
        public async Task<string> GetWeatherSummaryAsync()
        {
            try
            {
                var weather = await GetHarborWeatherAsync();
                if (weather == null)
                    return "Weather data unavailable";

                return $"{weather.GetStatusEmoji()} {weather.Description} | " +
                       $"{weather.Temperature:F1}°C | " +
                       $"Wind: {weather.WindSpeedKnots:F1} knots | " +
                       $"Visibility: {weather.Visibility}m";
            }
            catch (Exception ex)
            {
                return $"⚠️ Weather service error: {ex.Message}";
            }
        }
    }

    #region OpenWeatherMap API Response Models
    // Internal models for deserializing OpenWeatherMap JSON response

    internal class OpenWeatherResponse
    {
        [JsonProperty("weather")]
        public WeatherInfo[] Weather { get; set; } = Array.Empty<WeatherInfo>();

        [JsonProperty("main")]
        public MainInfo Main { get; set; } = new MainInfo();

        [JsonProperty("wind")]
        public WindInfo Wind { get; set; } = new WindInfo();

        [JsonProperty("visibility")]
        public int Visibility { get; set; }
    }

    internal class WeatherInfo
    {
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";
    }

    internal class MainInfo
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }
    }

    internal class WindInfo
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }
    #endregion
}
