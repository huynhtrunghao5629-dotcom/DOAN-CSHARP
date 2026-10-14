using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;

namespace HotelManagement.Desktop.UI
{
    public partial class FrmReport : Form
    {
        private readonly ReportService _reportService;
        private readonly UserSession _currentUser;

        // Các control tổng quan
        private Label lblTotalCustomersVal;
        private Label lblTotalRoomsVal;
        private Label lblTotalBookingsVal;
        private Label lblTotalRevenueVal;
        private Button btnRefresh;

        // Các control lọc theo ngày và bảng chi tiết
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Button btnFilter;
        private DataGridView dataGridView1;

        public FrmReport()
        {
            InitializeComponentCustom();
            _reportService = new ReportService();
        }

        public FrmReport(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        private void InitializeComponentCustom()
        {
            this.SuspendLayout();

            // Tiêu đề
            Label lblTitle = new Label()
            {
                Text = "BÁO CÁO THỐNG KÊ TỔNG HỢP HỆ THỐNG",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(30, 20),
                AutoSize = true
            };

            // --- Phần 1: Thống kê tổng quan ---
            Label lbl1 = new Label() { Text = "Tổng số Khách hàng:", Location = new System.Drawing.Point(30, 65), AutoSize = true };
            Label lbl2 = new Label() { Text = "Tổng số Phòng:", Location = new System.Drawing.Point(30, 100), AutoSize = true };
            Label lbl3 = new Label() { Text = "Tổng số Lượt đặt phòng:", Location = new System.Drawing.Point(30, 135), AutoSize = true };
            Label lbl4 = new Label() { Text = "Tổng doanh thu:", Location = new System.Drawing.Point(30, 170), AutoSize = true };

            lblTotalCustomersVal = new Label() { Text = "0", Location = new System.Drawing.Point(200, 65), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold), AutoSize = true };
            lblTotalRoomsVal = new Label() { Text = "0", Location = new System.Drawing.Point(200, 100), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold), AutoSize = true };
            lblTotalBookingsVal = new Label() { Text = "0", Location = new System.Drawing.Point(200, 135), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold), AutoSize = true };
            lblTotalRevenueVal = new Label() { Text = "0 VNĐ", Location = new System.Drawing.Point(200, 170), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.Green, AutoSize = true };

            btnRefresh = new Button() { Text = "Làm mới số liệu", Location = new System.Drawing.Point(30, 210), Width = 140, Height = 30 };
            btnRefresh.Click += BtnRefresh_Click;

            // --- Phần 2: Lọc theo khoảng thời gian và DataGridView ---
            Label lblFrom = new Label() { Text = "Từ ngày:", Location = new System.Drawing.Point(350, 65), AutoSize = true };
            dateTimePicker1 = new DateTimePicker() { Location = new System.Drawing.Point(420, 62), Format = DateTimePickerFormat.Short, Width = 130 };

            Label lblTo = new Label() { Text = "Đến ngày:", Location = new System.Drawing.Point(570, 65), AutoSize = true };
            dateTimePicker2 = new DateTimePicker() { Location = new System.Drawing.Point(640, 62), Format = DateTimePickerFormat.Short, Width = 130 };

            btnFilter = new Button() { Text = "Lọc dữ liệu", Location = new System.Drawing.Point(790, 60), Width = 90, Height = 28 };
            btnFilter.Click += btnFilter_Click;

            dataGridView1 = new DataGridView()
            {
                Location = new System.Drawing.Point(350, 105),
                Size = new System.Drawing.Size(530, 260),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Cấu hình Form
            this.ClientSize = new System.Drawing.Size(910, 400);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lbl1);
            this.Controls.Add(lbl2);
            this.Controls.Add(lbl3);
            this.Controls.Add(lbl4);
            this.Controls.Add(lblTotalCustomersVal);
            this.Controls.Add(lblTotalRoomsVal);
            this.Controls.Add(lblTotalBookingsVal);
            this.Controls.Add(lblTotalRevenueVal);
            this.Controls.Add(btnRefresh);

            // Thêm các control lọc vào form
            this.Controls.Add(lblFrom);
            this.Controls.Add(lblTo);
            this.Controls.Add(dateTimePicker1);
            this.Controls.Add(dateTimePicker2);
            this.Controls.Add(btnFilter);
            this.Controls.Add(dataGridView1);

            this.Name = "FrmReport";
            this.Text = "Báo cáo Thống kê Tổng hợp";
            this.Load += FrmReport_Load;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void FrmReport_Load(object sender, EventArgs e)
        {
            // Load số liệu tổng quan
            await LoadReportDataAsync();

            // Đặt mặc định thời gian lọc
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private async Task LoadReportDataAsync()
        {
            var report = await _reportService.GetSystemReportAsync();
            lblTotalCustomersVal.Text = report.TotalCustomers.ToString();
            lblTotalRoomsVal.Text = report.TotalRooms.ToString();
            lblTotalBookingsVal.Text = report.TotalBookings.ToString();
            lblTotalRevenueVal.Text = report.TotalRevenue.ToString("N0") + " VNĐ";
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
            MessageBox.Show("Cập nhật số liệu báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date;

                if (fromDate > toDate)
                {
                    MessageBox.Show("Khoảng thời gian không hợp lệ (Từ ngày phải nhỏ hơn hoặc bằng Đến ngày)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var reportData = await _reportService.GetReportDataByDateRangeAsync(fromDate, toDate);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = reportData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lọc dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}