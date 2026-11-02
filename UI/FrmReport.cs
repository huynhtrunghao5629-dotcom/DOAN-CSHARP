using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmReport : Form
    {
        public FrmReport()
        {
            InitializeComponent();
        }

        private async void FrmReport_Load(object sender, EventArgs e)
        {
            InitFilterControls();
            SetupDataGridView();
            await LoadReportDataAsync();
        }

        private void InitFilterControls()
        {
            cboMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                cboMonth.Items.Add($"Tháng {i}");
            }
            cboMonth.SelectedIndex = DateTime.Now.Month - 1;
            nudYear.Value = DateTime.Now.Year;
        }

        private void SetupDataGridView()
        {
            dgvReport.AutoGenerateColumns = true;
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.ReadOnly = true;
        }

        private async Task LoadReportDataAsync()
        {
            int month = cboMonth.SelectedIndex + 1;
            int year = (int)nudYear.Value;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"api/BaoCao/doanh-thu?thang={month}&nam={year}");

                    if (response.IsSuccessStatusCode)
                    {
                        var dailyReport = await response.Content.ReadFromJsonAsync<List<BaoCaoDoanhThuViewModel>>() ?? new List<BaoCaoDoanhThuViewModel>();
                        dgvReport.DataSource = dailyReport;

                        decimal grandTotalRevenue = dailyReport.Sum(r => r.TongDoanhThu);
                        int grandTotalBookings = dailyReport.Sum(r => r.SoLuotDatPhong);

                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        double occupancyRate = Math.Min(100.0, Math.Round((double)grandTotalBookings / (daysInMonth * 10) * 100, 1));

                        lblTotalRevenue.Text = $"{grandTotalRevenue:N0} VNĐ";
                        lblTotalBookings.Text = $"{grandTotalBookings} lượt đặt";
                        lblOccupancyRate.Text = $"{occupancyRate}%";
                    }
                    else
                    {
                        MessageBox.Show($"Không thể tải báo cáo. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi lấy báo cáo: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnViewReport_Click(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
        }
    }
}