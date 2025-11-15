using System.Threading.Tasks;
using Supabase;

namespace HarborMaster.Data
{
    public static class SupabaseManager
    {
        private const string SupabaseUrl = "https://vpltuzpbzqksvuxsuwle.supabase.co";
        private const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZwbHR1enBienFrc3Z1eHN1d2xlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjIwOTIwNzcsImV4cCI6MjA3NzY2ODA3N30.l8efVQYoUb-kxK54e6delGhYv0hjt1iOX7F1nMVy04g";
        private static Client _client;

        public static Client Client
        {
            get
            {
                if (_client == null)
                {
                    InitializeClient().Wait();
                }
                return _client;
            }
        }

        private static async Task InitializeClient()
        {
            if (_client != null) return;

            var options = new SupabaseOptions
            {
            };

            _client = new Client(SupabaseUrl, SupabaseKey, options);

            await _client.InitializeAsync();
        }

        public static async Task<bool> SignIn(string email, string password)
        {
            try
            {
                var response = await Client.Auth.SignIn(email, password);
                return response.User != null;
            }
            catch
            {
                return false;
            }
        }

        public static async Task SignOut()
        {
            await Client.Auth.SignOut();
        }
    }
}