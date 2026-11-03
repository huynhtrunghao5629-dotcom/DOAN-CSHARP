
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HotelManager.Services
{
    public static class BaseApiClient
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7203/api/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        static BaseApiClient()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public static HttpClient GetClient()
        {
            return _client;
        }

        // Gắn token sau khi đăng nhập thành công
        public static void SetToken(string? token)
        {
            _client.DefaultRequestHeaders.Authorization =
                string.IsNullOrWhiteSpace(token)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", token);
        }

        // Xóa token khi đăng xuất
        public static void ClearToken()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
