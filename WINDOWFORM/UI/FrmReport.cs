using HotelManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmReport : Form
    {
        private sealed class MonthlyRevenue
        {
            public int Thang { get; set; }
            public decimal DoanhThu { get; set; }
            public int SoDon { get; set; }
        }

        public FrmReport() => InitializeComponent();

        private async void FrmReport_Load(object sender, EventArgs e)
        {
            cboMonth.Items.Clear();
            for (int m = 1; m <= 12; m++) cboMonth.Items.Add($"Tháng {m}");
            cboMonth.SelectedIndex = DateTime.Now.Month - 1;
            nudYear.Value = Math.Min(nudYear.Maximum, Math.Max(nudYear.Minimum, DateTime.Now.Year));
            dgvReport.AutoGenerateColumns = true;
            dgvReport.BackgroundColor = System.Drawing.Color.White;
            lblTotalBookingsTitle.Text = "SỐ ĐƠN ĐẶT PHÁT SINH:";
            dgvReport.DataBindingComplete += (_, _) =>
            {
                foreach (DataGridViewColumn column in dgvReport.Columns)
                {
                    column.HeaderText = column.DataPropertyName switch
                    {
                        "Thang" => "Tháng",
                        "DoanhThu" => "Doanh thu đã ghi nhận",
                        "SoDon" => "Số đơn đặt",
                        _ => column.HeaderText
                    };
                    if (column.DataPropertyName == "DoanhThu") column.DefaultCellStyle.Format = "N0";
                }
            };
            dgvReport.ReadOnly = true;
            lblOccupancyRate.Text = "Chưa có dữ liệu";
            await LoadReportDataAsync();
        }

        private async Task LoadReportDataAsync()
        {
            var month = cboMonth.SelectedIndex + 1;
            var year = (int)nudYear.Value;
            try
            {
                btnViewReport.Enabled = false;
                using var response = await BaseApiClient.GetClient()
                    .GetAsync($"ThongKe/doanh-thu-theo-thang?nam={year}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Không lấy được báo cáo: HTTP {(int)response.StatusCode}", "Lỗi API");
                    return;
                }
                var yearly = await response.Content.ReadFromJsonAsync<List<MonthlyRevenue>>() ?? new();
                var current = yearly.Where(x => x.Thang == month).ToList();
                dgvReport.DataSource = current;
                lblTotalRevenue.Text = $"{current.Sum(x => x.DoanhThu):N0} VNĐ";
                lblTotalBookings.Text = $"{current.Sum(x => x.SoDon)} đơn";
                // No reliable sold-room-nights / available-room-nights data yet.
                lblOccupancyRate.Text = "Chưa có dữ liệu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải báo cáo: {ex.Message}", "Lỗi kết nối");
            }
            finally
            {
                btnViewReport.Enabled = true;
            }
        }

        private async void btnViewReport_Click(object sender, EventArgs e) => await LoadReportDataAsync();
    }
}
