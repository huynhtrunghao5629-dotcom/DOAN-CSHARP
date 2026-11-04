using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmManageServices : Form
    {
        private int _currentDatPhongId = 0;

        public FrmManageServices()
        {
            InitializeComponent();
        }

        // Hàm hỗ trợ lấy DataGridView độc lập tên Control trong Designer
        private DataGridView? GetGridServices()
        {
            if (Controls.Find("dgvSuDungDichVu", true).Length > 0)
                return Controls.Find("dgvSuDungDichVu", true)[0] as DataGridView;
            if (Controls.Find("dgvServices", true).Length > 0)
                return Controls.Find("dgvServices", true)[0] as DataGridView;
            return null;
        }

        private async void FrmManageServices_Load(object sender, EventArgs e)
        {
            await LoadDanhSachDichVuAsync();
            await LoadPhongDangOAsync();
        }

        private async System.Threading.Tasks.Task LoadDanhSachDichVuAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var list = await client.GetFromJsonAsync<List<DichVuViewModel>>("DichVu");
                if (Controls.Find("cboServices", true).Length > 0 && Controls.Find("cboServices", true)[0] is ComboBox cbo)
                {
                    cbo.DataSource = list;
                    cbo.DisplayMember = "TenDichVu";
                    cbo.ValueMember = "Id";
                }
            }
            catch { }
        }

        private async System.Threading.Tasks.Task LoadPhongDangOAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var list = await client.GetFromJsonAsync<List<BookingViewModel>>("DatPhong");
                var activeBookings = list?.FindAll(b => b.TrangThai == "DaNhanPhong") ?? new List<BookingViewModel>();

                if (Controls.Find("cboOccupiedRooms", true).Length > 0 && Controls.Find("cboOccupiedRooms", true)[0] is ComboBox cbo)
                {
                    cbo.DataSource = activeBookings;
                    cbo.DisplayMember = "SoPhong";
                    cbo.ValueMember = "Id";
                }
            }
            catch { }
        }

        private async void cboOccupiedRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cbo && cbo.SelectedValue is int datPhongId)
            {
                _currentDatPhongId = datPhongId;
                await LoadDichVuDaDungAsync(datPhongId);
            }
        }

        private void cboServices_SelectedIndexChanged(object sender, EventArgs e) { }

        private async System.Threading.Tasks.Task LoadDichVuDaDungAsync(int datPhongId)
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var list = await client.GetFromJsonAsync<List<ChiTietDichVuViewModel>>($"SuDungDichVu/by-dat-phong/{datPhongId}");
                var grid = GetGridServices();
                if (grid != null)
                {
                    grid.DataSource = list;
                    TieuDeTiengViet.GanCho(grid);
                }
            }
            catch { }
        }

        private async void btnAddService_Click(object sender, EventArgs e)
        {
            int selectedDichVuId = 0;
            int soLuong = 1;

            if (Controls.Find("cboServices", true).Length > 0 && Controls.Find("cboServices", true)[0] is ComboBox cbo)
                if (cbo.SelectedValue is int dvId) selectedDichVuId = dvId;

            if (Controls.Find("nudQuantity", true).Length > 0 && Controls.Find("nudQuantity", true)[0] is NumericUpDown nud)
                soLuong = (int)nud.Value;

            if (_currentDatPhongId <= 0 || selectedDichVuId <= 0) return;

            var dto = new
            {
                DatPhongId = _currentDatPhongId,
                DichVuId = selectedDichVuId,
                SoLuong = soLuong
            };

            var client = BaseApiClient.GetClient();
            var res = await client.PostAsJsonAsync("SuDungDichVu", dto);
            if (res.IsSuccessStatusCode)
            {
                await LoadDichVuDaDungAsync(_currentDatPhongId);
            }
        }

        private async void btnDeleteService_Click(object sender, EventArgs e)
        {
            var grid = GetGridServices();
            if (grid?.CurrentRow?.DataBoundItem is ChiTietDichVuViewModel item)
            {
                var client = BaseApiClient.GetClient();
                var res = await client.DeleteAsync($"SuDungDichVu/{item.Id}");
                if (res.IsSuccessStatusCode)
                {
                    await LoadDichVuDaDungAsync(_currentDatPhongId);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadPhongDangOAsync();
            if (_currentDatPhongId > 0)
                await LoadDichVuDaDungAsync(_currentDatPhongId);
        }
    }
}