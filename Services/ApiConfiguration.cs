using System;

namespace HarborMaster.Services
{
    /// <summary>
    /// Configuration class for external API keys and settings
    /// </summary>
    public static class ApiConfiguration
    {
        // ========================
        // GMAIL SMTP EMAIL SETTINGS
        // ========================
        // Setup Guide:
        // 1. Use your Gmail account
        // 2. Enable 2-Factor Authentication
        // 3. Generate App Password: https://myaccount.google.com/apppasswords
        // 4. Use App Password (not your regular password!)
        // Free tier: 500 emails/day

        public static string SmtpHost { get; set; } = "smtp.gmail.com";
        public static int SmtpPort { get; set; } = 587; // TLS port
        public static string SmtpUsername { get; set; } = "bulalacatering@gmail.com"; // Your Gmail address
        public static string SmtpPassword { get; set; } = "gfovutjkeqqojnly"; // Gmail App Password (16 characters)
        public static string SenderEmail { get; set; } = "bulalacatering@gmail.com"; // Same as SmtpUsername
        public static string SenderName { get; set; } = "Harbor Master System";

        // ========================
        // OPENWEATHERMAP API
        // ========================
        // Get your API key from: https://home.openweathermap.org/api_keys
        // Free tier: 60 calls/minute, 1,000,000 calls/month
        public static string OpenWeatherApiKey { get; set; } = "f0c7723b4e7b038d73d3560bb7d030c3";

        // Default harbor location (example: Jakarta Tanjung Priok)
        public static double HarborLatitude { get; set; } = -6.1057; // Jakarta
        public static double HarborLongitude { get; set; } = 106.8826;
        public static string HarborLocation { get; set; } = "Jakarta";

        // Weather safety thresholds
        public static double MaxSafeWindSpeed { get; set; } = 30.0; // knots
        public static double MinSafeVisibility { get; set; } = 1000.0; // meters
        public static double MaxSafeWaveHeight { get; set; } = 2.5; // meters

        /// <summary>
        /// Validate that all required API keys are configured
        /// </summary>
        public static bool IsConfigured()
        {
            bool hasSmtp = !string.IsNullOrEmpty(SmtpUsername) &&
                          !SmtpUsername.Contains("your-email") &&
                          !string.IsNullOrEmpty(SmtpPassword) &&
                          !SmtpPassword.Contains("your-app-password");
            bool hasOpenWeather = !string.IsNullOrEmpty(OpenWeatherApiKey) &&
                                  !OpenWeatherApiKey.Contains("YOUR_");

            return hasSmtp || hasOpenWeather; // At least one should be configured
        }

        /// <summary>
        /// Check if SMTP is configured
        /// </summary>
        public static bool IsSmtpConfigured()
        {
            return !string.IsNullOrEmpty(SmtpUsername) &&
                   !SmtpUsername.Contains("your-email") &&
                   !string.IsNullOrEmpty(SmtpPassword) &&
                   !SmtpPassword.Contains("your-app-password");
        }

        /// <summary>
        /// Get configuration status message
        /// </summary>
        public static string GetConfigurationStatus()
        {
            var status = "";

            if (IsSmtpConfigured())
                status += "✅ Gmail SMTP configured\n";
            else
                status += "⚠️ Gmail SMTP not configured\n";

            if (string.IsNullOrEmpty(OpenWeatherApiKey) || OpenWeatherApiKey.Contains("YOUR_"))
                status += "⚠️ OpenWeather API Key not configured\n";
            else
                status += "✅ OpenWeather API configured\n";

            return status;
        }
    }
}
