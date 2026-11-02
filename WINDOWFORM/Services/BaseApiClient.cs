using System;
using System.Net.Http;

namespace HotelManager.Services
{
    /// <summary>
    /// HttpClient dùng chung cho phần quản lý khách hàng trên WinForms.
    /// Cổng 7203 trùng cấu hình HTTPS của API trong Properties/launchSettings.json.
    /// </summary>
    public static class BaseApiClient
    {
        private static readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7203/api/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static HttpClient GetClient() => Client;
    }
}
