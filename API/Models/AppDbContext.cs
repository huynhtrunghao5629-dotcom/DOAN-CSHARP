using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyKhachSanAPI.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<SuDungDichVu> SuDungDichVus { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDatPhong>(entity =>
        {
            entity.HasKey(e => new { e.DatPhongId, e.PhongId });

            entity.ToTable("ChiTietDatPhong");

            entity.Property(e => e.DatPhongId).HasColumnName("DatPhongID");
            entity.Property(e => e.PhongId).HasColumnName("PhongID");
            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.DatPhong).WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(d => d.DatPhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDatPhong_DatPhong");

            entity.HasOne(d => d.Phong).WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(d => d.PhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDatPhong_Phong");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.DatPhongId).HasName("PK__DatPhong__2A079D7D79E9FB74");

            entity.ToTable("DatPhong");

            entity.Property(e => e.DatPhongId).HasColumnName("DatPhongID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(30);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatPhong_KhachHang");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK_DatPhong_NhanVien");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.DichVuId).HasName("PK__DichVu__8DB8EDA120F12FCA");

            entity.ToTable("DichVu");

            entity.HasIndex(e => e.TenDichVu, "UQ__DichVu__A77D0689838D9BD0").IsUnique();

            entity.Property(e => e.DichVuId).HasColumnName("DichVuID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh).HasMaxLength(30);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenDichVu).HasMaxLength(100);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211B06BD0D7A");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Cccd, "UQ__KhachHan__A955A0AA5A05DF8C").IsUnique();

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.LoaiPhongId).HasName("PK__LoaiPhon__5768124D32072877");

            entity.ToTable("LoaiPhong");

            entity.HasIndex(e => e.TenLoai, "UQ__LoaiPhon__E29B1042098A8EEA").IsUnique();

            entity.Property(e => e.LoaiPhongId).HasColumnName("LoaiPhongID");
            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EAED46BDD5");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.TaiKhoanId, "UQ__NhanVien__9A124B64F2CD45D3").IsUnique();

            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

            entity.HasOne(d => d.TaiKhoan).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.TaiKhoanId)
                .HasConstraintName("FK_NhanVien_TaiKhoan");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.PhongId).HasName("PK__Phong__FC6699471C130B39");

            entity.ToTable("Phong");

            entity.HasIndex(e => e.SoPhong, "UQ__Phong__7C736CA1EAECB6DF").IsUnique();

            entity.Property(e => e.PhongId).HasColumnName("PhongID");
            entity.Property(e => e.LoaiPhongId).HasColumnName("LoaiPhongID");
            entity.Property(e => e.SoPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(30);

            entity.HasOne(d => d.LoaiPhong).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.LoaiPhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_LoaiPhong");
        });

        modelBuilder.Entity<SuDungDichVu>(entity =>
        {
            entity.HasKey(e => e.SuDungDichVuId).HasName("PK__SuDungDi__63A2E3D9B456B5F3");

            entity.ToTable("SuDungDichVu");

            entity.Property(e => e.SuDungDichVuId).HasColumnName("SuDungDichVuID");
            entity.Property(e => e.DatPhongId).HasColumnName("DatPhongID");
            entity.Property(e => e.DichVuId).HasColumnName("DichVuID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgaySuDung).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.DatPhong).WithMany(p => p.SuDungDichVus)
                .HasForeignKey(d => d.DatPhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SuDungDichVu_DatPhong");

            entity.HasOne(d => d.DichVu).WithMany(p => p.SuDungDichVus)
                .HasForeignKey(d => d.DichVuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SuDungDichVu_DichVu");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaiKhoan__9A124B65FCFFD094");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0258B6FC6").IsUnique();

            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.VaiTro).HasMaxLength(20);
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.ThanhToanId).HasName("PK__ThanhToa__24A8D684A583E6A7");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.ThanhToanId).HasColumnName("ThanhToanID");
            entity.Property(e => e.DatPhongId).HasColumnName("DatPhongID");
            entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhuongThuc).HasMaxLength(30);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(30);

            entity.HasOne(d => d.DatPhong).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.DatPhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThanhToan_DatPhong");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
