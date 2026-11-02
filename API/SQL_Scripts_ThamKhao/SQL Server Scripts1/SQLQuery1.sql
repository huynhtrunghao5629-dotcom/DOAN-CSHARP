
USE master;
GO

IF DB_ID(N'QuanLyKhachSan') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyKhachSan
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE QuanLyKhachSan;
END
GO


CREATE DATABASE QuanLyKhachSan;
GO

USE QuanLyKhachSan;
GO
CREATE TABLE TaiKhoan
(
    TaiKhoanID INT IDENTITY(1,1) PRIMARY KEY,

    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,

    MatKhau NVARCHAR(255) NOT NULL,

    VaiTro NVARCHAR(20) NOT NULL,

    CONSTRAINT CK_TaiKhoan_VaiTro
        CHECK (VaiTro IN (N'Admin', N'NhanVien'))
);
GO
CREATE TABLE NhanVien
(
    NhanVienID INT IDENTITY(1,1) PRIMARY KEY,

    HoTen NVARCHAR(100) NOT NULL,

    SoDienThoai VARCHAR(15),

    Email VARCHAR(100),

    ChucVu NVARCHAR(50),

    TaiKhoanID INT UNIQUE,

    CONSTRAINT FK_NhanVien_TaiKhoan
        FOREIGN KEY (TaiKhoanID)
        REFERENCES TaiKhoan(TaiKhoanID)
);
GO
CREATE TABLE KhachHang
(
    KhachHangID INT IDENTITY(1,1) PRIMARY KEY,

    HoTen NVARCHAR(100) NOT NULL,

    CCCD VARCHAR(20) UNIQUE,

    SoDienThoai VARCHAR(15),

    Email VARCHAR(100),

    DiaChi NVARCHAR(200)
);
GO
CREATE TABLE LoaiPhong
(
    LoaiPhongID INT IDENTITY(1,1) PRIMARY KEY,

    TenLoai NVARCHAR(50) NOT NULL UNIQUE,

    SoNguoiToiDa INT NOT NULL,

    GiaPhong DECIMAL(18,2) NOT NULL,

    MoTa NVARCHAR(500),

    CONSTRAINT CK_LoaiPhong_SoNguoi
        CHECK (SoNguoiToiDa > 0),

    CONSTRAINT CK_LoaiPhong_Gia
        CHECK (GiaPhong >= 0)
);
GO
CREATE TABLE Phong
(
    PhongID INT IDENTITY(1,1) PRIMARY KEY,

    SoPhong VARCHAR(10) NOT NULL UNIQUE,

    LoaiPhongID INT NOT NULL,

    TrangThai NVARCHAR(30) NOT NULL,

    CONSTRAINT FK_Phong_LoaiPhong
        FOREIGN KEY (LoaiPhongID)
        REFERENCES LoaiPhong(LoaiPhongID),

    CONSTRAINT CK_Phong_TrangThai
        CHECK
        (
            TrangThai IN
            (
                N'Trong',
                N'DaDat',
                N'DangSuDung',
                N'DangBaoTri'
            )
        )
);
GO
CREATE TABLE DatPhong
(
    DatPhongID INT IDENTITY(1,1) PRIMARY KEY,

    KhachHangID INT NOT NULL,

    NhanVienID INT NULL,

    NgayDat DATETIME2 NOT NULL DEFAULT GETDATE(),

    NgayNhan DATE NOT NULL,

    NgayTra DATE NOT NULL,

    TrangThai NVARCHAR(30) NOT NULL,

    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_DatPhong_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID),

    CONSTRAINT FK_DatPhong_NhanVien
        FOREIGN KEY (NhanVienID)
        REFERENCES NhanVien(NhanVienID),

    CONSTRAINT CK_DatPhong_Ngay
        CHECK (NgayTra > NgayNhan),

    CONSTRAINT CK_DatPhong_TrangThai
        CHECK
        (
            TrangThai IN
            (
                N'ChoXacNhan',
                N'DaDat',
                N'DaNhanPhong',
                N'DaTraPhong',
                N'DaHuy'
            )
        ),

    CONSTRAINT CK_DatPhong_TongTien
        CHECK (TongTien >= 0)
);
GO
CREATE TABLE ChiTietDatPhong
(
    DatPhongID INT NOT NULL,

    PhongID INT NOT NULL,

    GiaPhong DECIMAL(18,2) NOT NULL,

    CONSTRAINT PK_ChiTietDatPhong
        PRIMARY KEY (DatPhongID, PhongID),

    CONSTRAINT FK_ChiTietDatPhong_DatPhong
        FOREIGN KEY (DatPhongID)
        REFERENCES DatPhong(DatPhongID),

    CONSTRAINT FK_ChiTietDatPhong_Phong
        FOREIGN KEY (PhongID)
        REFERENCES Phong(PhongID),

    CONSTRAINT CK_ChiTietDatPhong_Gia
        CHECK (GiaPhong >= 0)
);
GO
CREATE TABLE DichVu
(
    DichVuID INT IDENTITY(1,1) PRIMARY KEY,

    TenDichVu NVARCHAR(100) NOT NULL UNIQUE,

    DonGia DECIMAL(18,2) NOT NULL,

    DonViTinh NVARCHAR(30),

    MoTa NVARCHAR(500),

    CONSTRAINT CK_DichVu_DonGia
        CHECK (DonGia >= 0)
);
GO
CREATE TABLE SuDungDichVu
(
    SuDungDichVuID INT IDENTITY(1,1) PRIMARY KEY,

    DatPhongID INT NOT NULL,

    DichVuID INT NOT NULL,

    SoLuong INT NOT NULL,

    DonGia DECIMAL(18,2) NOT NULL,

    NgaySuDung DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_SuDungDichVu_DatPhong
        FOREIGN KEY (DatPhongID)
        REFERENCES DatPhong(DatPhongID),

    CONSTRAINT FK_SuDungDichVu_DichVu
        FOREIGN KEY (DichVuID)
        REFERENCES DichVu(DichVuID),

    CONSTRAINT CK_SuDungDichVu_SoLuong
        CHECK (SoLuong > 0),

    CONSTRAINT CK_SuDungDichVu_DonGia
        CHECK (DonGia >= 0)
);
GO
CREATE TABLE ThanhToan
(
    ThanhToanID INT IDENTITY(1,1) PRIMARY KEY,

    DatPhongID INT NOT NULL,

    NgayThanhToan DATETIME2 NOT NULL DEFAULT GETDATE(),

    SoTien DECIMAL(18,2) NOT NULL,

    PhuongThuc NVARCHAR(30) NOT NULL,

    TrangThai NVARCHAR(30) NOT NULL,

    CONSTRAINT FK_ThanhToan_DatPhong
        FOREIGN KEY (DatPhongID)
        REFERENCES DatPhong(DatPhongID),

    CONSTRAINT CK_ThanhToan_SoTien
        CHECK (SoTien > 0),

    CONSTRAINT CK_ThanhToan_PhuongThuc
        CHECK
        (
            PhuongThuc IN
            (
                N'TienMat',
                N'ChuyenKhoan',
                N'The'
            )
        ),

    CONSTRAINT CK_ThanhToan_TrangThai
        CHECK
        (
            TrangThai IN
            (
                N'ChoThanhToan',
                N'DaThanhToan',
                N'ThatBai'
            )
        )
);
GO

INSERT INTO TaiKhoan
(
    TenDangNhap,
    MatKhau,
    VaiTro
)
VALUES
(
    N'admin',
    N'123456',
    N'Admin'
),
(
    N'nhanvien01',
    N'123456',
    N'NhanVien'
);
GO

INSERT INTO NhanVien
(
    HoTen,
    SoDienThoai,
    Email,
    ChucVu,
    TaiKhoanID
)
VALUES
(
    N'Nguyen Van Admin',
    '0900000001',
    'admin@khachsan.com',
    N'Quan ly',
    1
),
(
    N'Tran Van B',
    '0900000002',
    'b@khachsan.com',
    N'Nhan vien le tan',
    2
);
GO

INSERT INTO KhachHang
(
    HoTen,
    CCCD,
    SoDienThoai,
    Email,
    DiaChi
)
VALUES
(
    N'Nguyen Van A',
    '079123456789',
    '0911111111',
    'a@gmail.com',
    N'TP.HCM'
),
(
    N'Tran Thi B',
    '079987654321',
    '0922222222',
    'b@gmail.com',
    N'Da Nang'
),
(
    N'Le Van C',
    '079555555555',
    '0933333333',
    'c@gmail.com',
    N'Hue'
);
GO
INSERT INTO LoaiPhong
(
    TenLoai,
    SoNguoiToiDa,
    GiaPhong,
    MoTa
)
VALUES
(
    N'Phong don',
    1,
    500000,
    N'Phong danh cho 1 nguoi'
),
(
    N'Phong doi',
    2,
    800000,
    N'Phong danh cho 2 nguoi'
),
(
    N'Phong VIP',
    4,
    1500000,
    N'Phong VIP tien nghi cao cap'
);
GO


------------------------------------------------------------
-- 17. THEM PHONG
------------------------------------------------------------

INSERT INTO Phong
(
    SoPhong,
    LoaiPhongID,
    TrangThai
)
VALUES
(
    '101',
    1,
    N'Trong'
),
(
    '102',
    1,
    N'Trong'
),
(
    '201',
    2,
    N'Trong'
),
(
    '202',
    2,
    N'Trong'
),
(
    '301',
    3,
    N'Trong'
);
GO


------------------------------------------------------------
-- 18. THEM DICH VU
------------------------------------------------------------

INSERT INTO DichVu
(
    TenDichVu,
    DonGia,
    DonViTinh,
    MoTa
)
VALUES
(
    N'Nuoc suoi',
    10000,
    N'Chai',
    N'Nuoc suoi dong chai'
),
(
    N'An sang',
    100000,
    N'Suat',
    N'Suat an sang'
),
(
    N'Giat ui',
    50000,
    N'Kg',
    N'Dich vu giat ui'
),
(
    N'Thue xe',
    300000,
    N'Ngay',
    N'Thue xe trong ngay'
);
GO
INSERT INTO DatPhong
(
    KhachHangID,
    NhanVienID,
    NgayNhan,
    NgayTra,
    TrangThai,
    TongTien
)
VALUES
(
    1,
    2,
    '2026-10-10',
    '2026-10-12',
    N'DaDat',
    2600000
);
GO


------------------------------------------------------------
-- 20. THEM CHI TIET DAT PHONG
------------------------------------------------------------

INSERT INTO ChiTietDatPhong
(
    DatPhongID,
    PhongID,
    GiaPhong
)
VALUES
(
    1,
    1,
    500000
),
(
    1,
    3,
    800000
);
GO
INSERT INTO SuDungDichVu
(
    DatPhongID,
    DichVuID,
    SoLuong,
    DonGia
)
VALUES
(
    1,
    1,
    2,
    10000
),
(
    1,
    2,
    1,
    100000
);
GO


------------------------------------------------------------
-- 22. THEM THANH TOAN
------------------------------------------------------------

INSERT INTO ThanhToan
(
    DatPhongID,
    SoTien,
    PhuongThuc,
    TrangThai
)
VALUES
(
    1,
    2600000,
    N'ChuyenKhoan',
    N'DaThanhToan'
);
GO
----------------------

SELECT * FROM TaiKhoan;
GO


------------------------------------------------------------
-- 24. KIEM TRA NHAN VIEN
------------------------------------------------------------

SELECT * FROM NhanVien;
GO


------------------------------------------------------------
-- 25. KIEM TRA KHACH HANG
------------------------------------------------------------

SELECT * FROM KhachHang;
GO


------------------------------------------------------------
-- 26. KIEM TRA LOAI PHONG
------------------------------------------------------------

SELECT * FROM LoaiPhong;
GO


------------------------------------------------------------
-- 27. KIEM TRA PHONG
------------------------------------------------------------

SELECT * FROM Phong;
GO


------------------------------------------------------------
-- 28. KIEM TRA DAT PHONG
------------------------------------------------------------

SELECT * FROM DatPhong;
GO


------------------------------------------------------------
-- 29. KIEM TRA CHI TIET DAT PHONG
------------------------------------------------------------

SELECT * FROM ChiTietDatPhong;
GO


------------------------------------------------------------
-- 30. KIEM TRA DICH VU
------------------------------------------------------------

SELECT * FROM DichVu;
GO


------------------------------------------------------------
-- 31. KIEM TRA SU DUNG DICH VU
------------------------------------------------------------

SELECT * FROM SuDungDichVu;
GO


------------------------------------------------------------
-- 32. KIEM TRA THANH TOAN
------------------------------------------------------------

SELECT * FROM ThanhToan;
GO


/* =========================================================
   PHAN 5: KIEM TRA QUAN HE GIUA CAC BANG
   ========================================================= */


------------------------------------------------------------
-- 33. XEM THONG TIN DON DAT PHONG
------------------------------------------------------------

SELECT
    DP.DatPhongID,
    KH.HoTen AS KhachHang,
    NV.HoTen AS NhanVien,
    DP.NgayDat,
    DP.NgayNhan,
    DP.NgayTra,
    DP.TrangThai,
    DP.TongTien
FROM DatPhong DP
INNER JOIN KhachHang KH
    ON DP.KhachHangID = KH.KhachHangID
LEFT JOIN NhanVien NV
    ON DP.NhanVienID = NV.NhanVienID;
GO


------------------------------------------------------------
-- 34. XEM PHONG TRONG CAC DON DAT
------------------------------------------------------------

SELECT
    DP.DatPhongID,
    KH.HoTen AS KhachHang,
    P.SoPhong,
    LP.TenLoai,
    CT.GiaPhong,
    DP.NgayNhan,
    DP.NgayTra
FROM DatPhong DP
INNER JOIN KhachHang KH
    ON DP.KhachHangID = KH.KhachHangID
INNER JOIN ChiTietDatPhong CT
    ON DP.DatPhongID = CT.DatPhongID
INNER JOIN Phong P
    ON CT.PhongID = P.PhongID
INNER JOIN LoaiPhong LP
    ON P.LoaiPhongID = LP.LoaiPhongID;
GO


------------------------------------------------------------
-- 35. XEM DICH VU KHACH DA SU DUNG
------------------------------------------------------------

SELECT
    DP.DatPhongID,
    KH.HoTen AS KhachHang,
    DV.TenDichVu,
    SDV.SoLuong,
    SDV.DonGia,
    SDV.SoLuong * SDV.DonGia AS ThanhTien,
    SDV.NgaySuDung
FROM SuDungDichVu SDV
INNER JOIN DatPhong DP
    ON SDV.DatPhongID = DP.DatPhongID
INNER JOIN KhachHang KH
    ON DP.KhachHangID = KH.KhachHangID
INNER JOIN DichVu DV
    ON SDV.DichVuID = DV.DichVuID;
GO


/* =========================================================
   36. KIEM TRA TOAN BO THONG TIN DAT PHONG
   ========================================================= */

SELECT
    DP.DatPhongID,

    KH.HoTen AS KhachHang,

    KH.SoDienThoai,

    P.SoPhong,

    LP.TenLoai AS LoaiPhong,

    CT.GiaPhong,

    DP.NgayDat,

    DP.NgayNhan,

    DP.NgayTra,

    DP.TrangThai,

    DP.TongTien

FROM DatPhong DP

INNER JOIN KhachHang KH
    ON DP.KhachHangID = KH.KhachHangID

INNER JOIN ChiTietDatPhong CT
    ON DP.DatPhongID = CT.DatPhongID

INNER JOIN Phong P
    ON CT.PhongID = P.PhongID

INNER JOIN LoaiPhong LP
    ON P.LoaiPhongID = LP.LoaiPhongID

ORDER BY DP.DatPhongID;
GO


/* =========================================================
   KET THUC DATABASE
   ========================================================= */