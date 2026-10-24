using System;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageRooms : Form
    {
        public FrmManageRooms()
        {
            InitializeComponent();
        }

        private void FrmManageRooms_Load(object sender, EventArgs e)
        {
            // Cấu hình ban đầu cho form Quản lý phòng
            this.Text = "Quản lý Thông tin Phòng khách sạn";
        }
    }
}