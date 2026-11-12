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
            var req = new
            {
                TenDangNhap = txtUsername.Text.Trim(),
                MatKhau = txtPassword.Text
            };

            if (string.IsNullOrWhiteSpace(req.TenDangNhap) ||
                string.IsNullOrWhiteSpace(req.MatKhau))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;

            try
            {
                var client = BaseApiClient.GetClient();

                var response = await client.PostAsJsonAsync(
                    "TaiKhoan/dang-nhap", req);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<UserSession>();

                    if (result != null &&
                        !string.IsNullOrWhiteSpace(result.Token))
                    {
                        CurrentUser = result;
                        BaseApiClient.SetToken(result.Token);

                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }

                    MessageBox.Show(
                        "Dữ liệu đăng nhập từ máy chủ không hợp lệ.",
                        "Lỗi đăng nhập",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show(
                        "Tên đăng nhập hoặc mật khẩu không chính xác.",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        $"Không thể đăng nhập. Mã lỗi HTTP: {(int)response.StatusCode}",
                        "Lỗi máy chủ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể kết nối API: {ex.Message}",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}