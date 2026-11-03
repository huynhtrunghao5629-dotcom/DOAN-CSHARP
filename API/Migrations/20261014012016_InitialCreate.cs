using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhachSanAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    DichVuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DichVu__8DB8EDA120F12FCA", x => x.DichVuID);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__880F211B06BD0D7A", x => x.KhachHangID);
                });

            migrationBuilder.CreateTable(
                name: "LoaiPhong",
                columns: table => new
                {
                    LoaiPhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: false),
                    GiaPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoaiPhon__5768124D32072877", x => x.LoaiPhongID);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    TaiKhoanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaiKhoan__9A124B65FCFFD094", x => x.TaiKhoanID);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    PhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    LoaiPhongID = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phong__FC6699471C130B39", x => x.PhongID);
                    table.ForeignKey(
                        name: "FK_Phong_LoaiPhong",
                        column: x => x.LoaiPhongID,
                        principalTable: "LoaiPhong",
                        principalColumn: "LoaiPhongID");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    NhanVienID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaiKhoanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__E27FD7EAED46BDD5", x => x.NhanVienID);
                    table.ForeignKey(
                        name: "FK_NhanVien_TaiKhoan",
                        column: x => x.TaiKhoanID,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateTable(
                name: "DatPhong",
                columns: table => new
                {
                    DatPhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangID = table.Column<int>(type: "int", nullable: false),
                    NhanVienID = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    NgayNhan = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayTra = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DatPhong__2A079D7D79E9FB74", x => x.DatPhongID);
                    table.ForeignKey(
                        name: "FK_DatPhong_KhachHang",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK_DatPhong_NhanVien",
                        column: x => x.NhanVienID,
                        principalTable: "NhanVien",
                        principalColumn: "NhanVienID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDatPhong",
                columns: table => new
                {
                    DatPhongID = table.Column<int>(type: "int", nullable: false),
                    PhongID = table.Column<int>(type: "int", nullable: false),
                    GiaPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDatPhong", x => new { x.DatPhongID, x.PhongID });
                    table.ForeignKey(
                        name: "FK_ChiTietDatPhong_DatPhong",
                        column: x => x.DatPhongID,
                        principalTable: "DatPhong",
                        principalColumn: "DatPhongID");
                    table.ForeignKey(
                        name: "FK_ChiTietDatPhong_Phong",
                        column: x => x.PhongID,
                        principalTable: "Phong",
                        principalColumn: "PhongID");
                });

            migrationBuilder.CreateTable(
                name: "SuDungDichVu",
                columns: table => new
                {
                    SuDungDichVuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatPhongID = table.Column<int>(type: "int", nullable: false),
                    DichVuID = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SuDungDi__63A2E3D9B456B5F3", x => x.SuDungDichVuID);
                    table.ForeignKey(
                        name: "FK_SuDungDichVu_DatPhong",
                        column: x => x.DatPhongID,
                        principalTable: "DatPhong",
                        principalColumn: "DatPhongID");
                    table.ForeignKey(
                        name: "FK_SuDungDichVu_DichVu",
                        column: x => x.DichVuID,
                        principalTable: "DichVu",
                        principalColumn: "DichVuID");
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    ThanhToanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatPhongID = table.Column<int>(type: "int", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ThanhToa__24A8D684A583E6A7", x => x.ThanhToanID);
                    table.ForeignKey(
                        name: "FK_ThanhToan_DatPhong",
                        column: x => x.DatPhongID,
                        principalTable: "DatPhong",
                        principalColumn: "DatPhongID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDatPhong_PhongID",
                table: "ChiTietDatPhong",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_KhachHangID",
                table: "DatPhong",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DatPhong_NhanVienID",
                table: "DatPhong",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "UQ__DichVu__A77D0689838D9BD0",
                table: "DichVu",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__KhachHan__A955A0AA5A05DF8C",
                table: "KhachHang",
                column: "CCCD",
                unique: true,
                filter: "[CCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__LoaiPhon__E29B1042098A8EEA",
                table: "LoaiPhong",
                column: "TenLoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__NhanVien__9A124B64F2CD45D3",
                table: "NhanVien",
                column: "TaiKhoanID",
                unique: true,
                filter: "[TaiKhoanID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_LoaiPhongID",
                table: "Phong",
                column: "LoaiPhongID");

            migrationBuilder.CreateIndex(
                name: "UQ__Phong__7C736CA1EAECB6DF",
                table: "Phong",
                column: "SoPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVu_DatPhongID",
                table: "SuDungDichVu",
                column: "DatPhongID");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVu_DichVuID",
                table: "SuDungDichVu",
                column: "DichVuID");

            migrationBuilder.CreateIndex(
                name: "UQ__TaiKhoan__55F68FC0258B6FC6",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_DatPhongID",
                table: "ThanhToan",
                column: "DatPhongID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDatPhong");

            migrationBuilder.DropTable(
                name: "SuDungDichVu");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "DatPhong");

            migrationBuilder.DropTable(
                name: "LoaiPhong");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
