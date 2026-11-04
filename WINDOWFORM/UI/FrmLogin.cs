using System;
using System.Net.Http.Json;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmLogin : Form
    {
        public UserSession? CurrentUser { get; private set; }

        public FrmLogin()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var req = new LoginRequest
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim()
            };

            try
            {
                var client = BaseApiClient.GetClient();
                var response = await client.PostAsJsonAsync("TaiKhoan/login", req);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserSession>();
                    if (result != null)
                    {
                        CurrentUser = result;
                        BaseApiClient.SetToken(result.Token); // Gắn Token vào Header[cite: 17]

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                }

                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API Server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}