# CLAUDE.md

---

## 1. Ngăn xếp công nghệ & Quy tắc kiến trúc
- **Framework & Giao diện:** C# .NET Framework / .NET Core, Windows Forms (`.cs`, `.Designer.cs`). Tách biệt logic giao diện (UI logic) khỏi tầng truy cập dữ liệu (data access). Không viết câu lệnh SQL thô trực tiếp trong file code-behind của Form; hãy sử dụng các lớp Repository hoặc Service phù hợp.
- **Cơ sở dữ liệu:** Microsoft SQL Server (`QuanLyKhachSan`).
  - Mẫu chuỗi kết nối chuẩn: `Server=.\SQLEXPRESS;Database=QuanLyKhachSan;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;`.
  - Luôn sử dụng tham số hóa câu lệnh SQL (parameterized queries) hoặc Stored Procedures để ngăn chặn lỗi SQL Injection. Xử lý các ngoại lệ kết nối cơ sở dữ liệu một cách mượt mà.
- **Quản lý phiên bản & Làm việc nhóm:**
  - Tuyệt đối không đẩy mã nguồn trực tiếp (push directly) lên nhánh `main` hoặc các nhánh phát hành chung mà chưa qua kiểm tra cục bộ hoặc đánh giá (review).
  - Xử lý các xung đột mã nguồn (merge conflicts) một cách cẩn thận, đặc biệt là trong các file `Program.cs`, `.csproj` và các file `.Designer.cs` do hệ thống tự sinh.

---

## 2. Nguyên tắc kỹ thuật cốt lõi

### 1. Think Before Coding
- **Nêu rõ giả định:** Xác nhận lại cấu trúc bảng (schema), chữ ký Stored Procedure hoặc vòng đời sự kiện WinForms trước khi viết code.
- **Không nhập nhằng ngầm định:** Nếu một yêu cầu (ví dụ: tính toán tiền đặt phòng, logic giảm giá hóa đơn, quy trình nhận/trả phòng) có nhiều cách hiểu khác nhau, hãy trình bày rõ các phương án trước khi thực hiện.

### 2. Simplicity First
- **Không làm phức tạp hóa vấn đề:** Tránh tạo ra các tầng trừu tượng quá mức, thư viện thành phần UI tùy chỉnh phức tạp, hoặc các hàm trợ giúp (helpers) tổng quát chưa cần thiết cho các mô hình nghiệp vụ quản lý khách sạn.
- **Giữ cho WinForms gọn gàng:** Tránh đăng ký sự kiện trùng lặp gây rò rỉ bộ nhớ (memory leaks) từ các sự kiện Form không được hủy đăng ký, hoặc trộn lẫn logic nghiệp vụ cồng kềnh trực tiếp vào các trình xử lý sự kiện nhấn nút (button click handlers).

### 3. Surgical Changes
- **Chỉ chạm vào những gì bắt buộc:** Khi chỉnh sửa các Form hoặc module dữ liệu (ví dụ: quản lý phòng, hóa đơn, theo dõi khách hàng), hãy tránh thay đổi bố cục giao diện hoặc định dạng của các phần lân cận không liên quan.
- **Tuân thủ phong cách hiện có:** Tuân theo quy tắc đặt tên (`camelCase` cho biến cục bộ/private field, `PascalCase` cho class, method, property) và cấu trúc mã nguồn đã được thiết lập sẵn trong dự án. Chỉ dọn dẹp các thư viện `using` thừa hoặc biến do chính các thay đổi của bạn tạo ra.

### 4. Goal-Driven Execution
- **Xác thực trước khi hoàn tất:** Biến các nhiệm vụ thành các tiêu chí kiểm tra cụ thể:
  - *"Sửa lỗi cập nhật trạng thái phòng"* → Viết các bước tái hiện lỗi, kiểm tra đồng bộ trạng thái UI với SQL Server, sau đó xác thực lại.
  - *"Thêm tính năng tính tiền phòng"* → Kiểm tra các trường hợp biên (thuế, ngày check-in/check-out, loại phòng) trước khi commit.