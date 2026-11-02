<<<<<<< HEAD:WINDOWFORM/Services/BaseApiClient.cs
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
=======
﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using HotelManager.UI;

namespace HotelManager
{
    public static class BaseApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static BaseApiClient()
        {
            // Trỏ tới cổng API Backend đang chạy thực tế (7203)
            _httpClient.BaseAddress = new Uri("https://localhost:7203/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static HttpClient GetClient()
        {
            // Tự động lấy Token từ FrmMain nhét vào Header trước khi gọi API
            if (FrmMain.CurrentSession != null && !string.IsNullOrEmpty(FrmMain.CurrentSession.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", FrmMain.CurrentSession.Token);
            }
            return _httpClient;
        }
    }
}
>>>>>>> master:Services/BaseApiClient.cs
