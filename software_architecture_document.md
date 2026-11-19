# SOFTWARE ARCHITECTURE DOCUMENT (SAD)
## HỆ THỐNG QUẢN LÝ KHÁCH SẠN & ĐẶT PHÒNG TRỰC TUYẾN (SAWADE HOTELS & RESORTS)

---

## 1. Introduction

### 1.1. Purpose
Tài liệu này cung cấp mô hình kiến trúc toàn diện cho hệ thống **SAWADE Hotels & Resorts**, phục vụ cho việc thiết kế, phát triển, triển khai và bảo trì hệ thống phần mềm quản lý lưu trú đa nền tảng (Web & Desktop).

### 1.2. Scope (Phạm vi)
Hệ thống bao gồm:
* **Phân hệ Web Khách hàng (`QuanLyKhachSan.Web`):** Xây dựng bằng ASP.NET Core MVC (.NET 8.0), phục vụ khách hàng tra cứu, lọc, xem chi tiết và đặt phòng trực tuyến.
* **Phân hệ Quản trị Nội bộ (`HotelManager`):** Xây dựng bằng Windows Forms (.NET 8.0) theo mô hình MDI, phục vụ Lễ tân và Quản lý vận hành nghiệp vụ tại quầy.
* **Phân hệ Dịch vụ Trung tâm (`QuanLyKhachSanAPI`):** Xây dựng bằng ASP.NET Core Web API (.NET 8.0), đóng vai trò xử lý logic nghiệp vụ, giao dịch cơ sở dữ liệu và cung cấp RESTful API.
* **CSDL tập trung:** Microsoft SQL Server.

---

## 2. Architectural Goals and Constraints

* **Tính nhất quán dữ liệu (ACID Compliance):** Chống đặt trùng lịch (Overbooking) thời gian thực giữa Web và WinForms thông qua Database Transaction (`BEGIN TRANSACTION`, `ROLLBACK`, `COMMIT`).
* **Khả năng bảo trì & Mở rộng:** Tách biệt rõ ràng giữa tầng Giao diện (Presentation Layer), Tầng Nghiệp vụ (Business Logic Layer) và Tầng Dữ liệu (Data Access Layer) theo mô hình Shared Business & Data Layer Architecture.
* **Ràng buộc công nghệ:** Sử dụng đồng nhất hệ sinh thái .NET 8.0 SDK, ngôn ngữ C# và Entity Framework Core (EF Core) làm ORM chuẩn cho các thao tác truy xuất dữ liệu.

---

## 3. Use-Case Model

* **Khách hàng:** Tra cứu & lọc phòng trống (theo ngày, khoảng giá, hạng phòng), xem chi tiết, đặt phòng trực tuyến và quản lý lịch sử đơn cá nhân.
* **Lễ tân:** Theo dõi sơ đồ phòng trực quan, duyệt đơn đặt từ Web, thực hiện Check-in / Check-out, ghi nhận dịch vụ phát sinh và lập hóa đơn thanh toán.
* **Quản lý:** Quản lý danh mục phòng, thiết lập đơn giá, phân quyền tài khoản nhân viên và theo dõi báo cáo doanh thu, công suất phòng.

---

## 4. Component View
* **4.1. Component: Phân hệ Khách hàng (Web MVC):** Quản lý các Controller (`HomeController`, `BookingController`), bộ lọc đa tiêu chí, ViewModels (`PhongViewModel`) và Razor Views.
* **4.2. Component: Phân hệ Dịch vụ Trung tâm (Web API):** Quản lý `PhongController`, `DatPhongController`, xử lý giao dịch chống trùng lịch và kết nối `KhachSanDbContext`.
* **4.3. Component: Phân hệ Quản trị Nội bộ (WinForms MDI):** Quản lý các Form nghiệp vụ (Sơ đồ phòng, Quản lý phòng, Quản lý khách hàng, Dịch vụ phát sinh, Thanh toán & Hóa đơn) qua các lớp Business Services.
* **4.4. Component: Quản lý Dữ liệu & Giao dịch:** Đảm bảo toàn vẹn dữ liệu chuẩn 3NF trên SQL Server cho 8 bảng cốt lõi (`LOAI_PHONG`, `PHONG`, `KHACH_HANG`, `NHAN_VIEN`, `DAT_PHONG`, `DICH_VU`, `SU_DUNG_DICH_VU`, `HOA_DON`).

---