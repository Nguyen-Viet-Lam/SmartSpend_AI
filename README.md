# SmartSpend AI

Web quản lý chi tiêu cá nhân bằng ASP.NET Core, SQL Server, JWT, OTP Gmail và SignalR.

## SmartSpend hiện có gì
- `Home / About / Guide` để giới thiệu dự án, công nghệ và cách dùng.
- `Auth` gồm đăng ký, đăng nhập, OTP xác thực email, quên mật khẩu, đặt lại mật khẩu.
- `User area` gồm Dashboard, Ví, Giao dịch, Ngân sách, Hồ sơ.
- `Admin area` gồm Dashboard hệ thống và quản lý user trong bản khung hiện tại.
- `SignalR Budget Alert` để đẩy cảnh báo khi ngân sách vượt 80% hoặc 100%.

## Bản khung hiện tại
Repo đang được giữ theo hướng `skeleton để test và phát triển tiếp`, nghĩa là:
- Ưu tiên page flow, database khung và phân quyền cơ bản.
- Tạm hạ một số phần nâng cao khỏi giao diện chính để repo gọn hơn.
- Phù hợp để chia việc nhóm backend/frontend trong các sprint tiếp theo.

## Phân quyền
- `Guest`: xem Home / About / Guide và dùng các trang xác thực.
- `StandardUser`: quản lý ví, giao dịch, ngân sách và hồ sơ của chính mình.
- `SystemAdmin`: xem thống kê hệ thống, khóa/mở khóa user, quản lý keyword AI, xem audit logs.

## Database chính
Schema hiện tại xoay quanh các bảng:
- `Users`
- `Roles`
- `EmailVerificationOtps`
- `Wallets`
- `Categories`
- `Transactions`
- `Budgets`
- `BudgetAlerts`
- `Keywords`
- `AuditLogs`
- `Transfers`

Migration reset mới của SmartSpend:
- `InitialSmartSpend`

## Project Management Files
- File giao việc chi tiết 14 ngày:
  [SMARTSPEND_TEAM_ASSIGNMENT_14D.md](SMARTSPEND_TEAM_ASSIGNMENT_14D.md)
- File backlog tổng đã cập nhật link sang file giao việc:
  [SMARTSPEND_BACKLOG.md](SMARTSPEND_BACKLOG.md)
- File mô tả phạm vi bản khung và phần phát triển sau:
  [SMARTSPEND_SKELETON_SCOPE.md](SMARTSPEND_SKELETON_SCOPE.md)

## Chạy local
1. Mở SQL Server và đảm bảo kết nối được tới `localhost,1433`.
   Mật khẩu mẫu của repo là `SmartSpend123!`.
2. Kiểm tra `ConnectionStrings:DefaultConnection` trong `appsettings.json`.
   Mặc định project local đang dùng database `SmartSpendDb` với `Encrypt=False;TrustServerCertificate=True;` để tránh lỗi bắt tay TLS với SQL Server local và tránh đụng schema cũ.
3. Nếu muốn ghi đè cấu hình riêng máy bạn mà không commit vào git, tạo `appsettings.Local.json` dựa trên:
   - `appsettings.Local.example.json`
4. Apply migration:
```powershell
dotnet ef database update --project Wed_Project.csproj --startup-project Wed_Project.csproj
```
5. Chạy web:
```powershell
dotnet run --launch-profile http
```
6. Mở các trang chính:
- Home: `http://localhost:5176/home/index.html`
- About: `http://localhost:5176/home/about.html`
- Guide: `http://localhost:5176/home/guide.html`
- Login: `http://localhost:5176/home/login.html`
- Dashboard: `http://localhost:5176/home/dashboard.html`
- Admin: `http://localhost:5176/home/admin-dashboard.html`

### Auto seed trong môi trường Development
`appsettings.Development.json` đang bật:
- `AutoApplyMigrations = true`
- `Enabled = true`
- `SeedDemoData = true`

Điều đó có nghĩa là khi chạy local ở `Development`, app sẽ:
1. Tự apply migration mới nhất.
2. Tự tạo tài khoản admin mẫu.
3. Tự tạo tài khoản demo user mẫu kèm ví, giao dịch, budget, alert.
4. Nếu chưa cấu hình SMTP thật, OTP mail sẽ được log ra terminal để test local.

### Tài khoản mẫu để demo
- `SystemAdmin`
  - Email: `admin@smartspend.local`
  - Username: `admin.smartspend`
  - Password: `Admin123!`
- `StandardUser`
  - Email: `demo@smartspend.local`
  - Username: `demo.smartspend`
  - Password: `Demo123!`

## Page Map
### Public
- `/home/index.html`
- `/home/about.html`
- `/home/guide.html`

### Auth
- `/home/login.html`
- `/home/register.html`
- `/home/otp.html`
- `/home/forgot-password.html`
- `/home/reset-password.html`

### User
- `/home/dashboard.html`
- `/home/wallets.html`
- `/home/transactions.html`
- `/home/budgets.html`
- `/home/profile.html`

### Admin
- `/home/admin-dashboard.html`
- `/home/admin-users.html`

## Ghi chú kỹ thuật
- Route mặc định `/` và `/home` đều redirect về `/home/index.html`.
- Logging đã chuyển về `Console + Debug` để app chạy local không bị lỗi quyền ghi Windows Event Log.
- Dashboard và admin pages là static HTML + JS gọi API backend.
- Protected pages sẽ kiểm tra JWT ở phía client và các API đều được bảo vệ ở phía server.
- Seeder chỉ bật mặc định trong `appsettings.Development.json`, còn `appsettings.json` để trạng thái tắt.

## Troubleshooting
### 1. Lỗi `file+.vscode-resource...` hoặc `DNS_PROBE_FINISHED_NXDOMAIN`
Nguyên nhân:
- Đây không phải URL thật của web project.
- Đây là URL nội bộ của VS Code webview.

Cách xử lý:
1. Không mở link `file+.vscode-resource...` trên browser ngoài.
2. Chạy app bằng `dotnet run`.
3. Mở đúng URL localhost của project.

### 2. App không lên sau khi `dotnet run`
Kiểm tra:
```powershell
netstat -ano | findstr :5176
```
Nếu cổng bị chiếm, đổi port trong `Properties/launchSettings.json`.

### 3. Lỗi SQL Server
Kiểm tra:
```powershell
Test-NetConnection localhost -Port 1433
```
Nếu `TcpTestSucceeded = False`, hãy bật lại SQL Server service / TCP port.

Nếu gặp lỗi kiểu `SQL Server requires encryption but this machine does not support it`, hãy kiểm tra connection string đang có:
- `Encrypt=False`
- `TrustServerCertificate=True`

### 4. OTP không thấy gửi mail
- Nếu `Smtp:Host` đang để trống, app sẽ không gửi Gmail thật mà log nội dung OTP ra terminal để test local.
- Nếu muốn gửi Gmail thật, cấu hình `appsettings.Local.json` theo mẫu `appsettings.Local.example.json`.

### 5. Sau khi đổi schema SmartSpend mà DB cũ lỗi
Chạy lại:
```powershell
dotnet ef database update --project Wed_Project.csproj --startup-project Wed_Project.csproj
```
Nếu bạn đang giữ dữ liệu DB cũ của schema học tập trước đây thì nên backup trước khi update schema mới.
