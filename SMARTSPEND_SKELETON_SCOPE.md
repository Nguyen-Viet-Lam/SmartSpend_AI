# SMARTSPEND SKELETON SCOPE

Updated: 2026-03-16

## Mục đích của bản này

Repo hiện tại được dọn theo hướng `khung sạch để test và phát triển tiếp`, không cố hoàn thành toàn bộ tính năng nâng cao ngay.

Mục tiêu:
- Có đủ page flow để demo đồ án.
- Có đủ database khung để backend nối tiếp.
- Giao diện rõ ràng, ít rối, dễ phân chia việc nhóm.

## Khung hiện có

### Public pages
- `Home`
- `About`
- `Guide`

### Auth pages
- `Login`
- `Register`
- `OTP`
- `Forgot Password`
- `Reset Password`

### User pages
- `Dashboard`
- `Wallets`
- `Transactions`
- `Budgets`
- `Profile`

### Admin pages
- `Admin Dashboard`
- `Admin Users`

## Khung dữ liệu hiện có

Các bảng chính đã có trong schema:
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

Ghi chú:
- Schema đang rộng hơn nhu cầu skeleton vì giữ sẵn chỗ cho giai đoạn phát triển sau.
- Không bắt buộc phải hoàn thiện hết các bảng mở rộng ngay trong bản test đầu tiên.

## Những gì nên xem là "đã sẵn sàng để test"

- Luồng `Đăng ký -> OTP -> Đăng nhập`
- Luồng `Tạo ví -> Nhập giao dịch -> Xem dashboard`
- Luồng `Tạo ngân sách -> Theo dõi progress`
- Luồng `Admin -> xem tổng quan -> khóa/mở khóa user`
- Dark mode và responsive cơ bản

## Những gì đã được hạ xuống mức phát triển sau

Các phần dưới đây không còn nằm trong luồng giao diện chính của skeleton:
- `AI Smart Input` trên trang giao dịch
- `Admin Keywords`
- `Admin Audit Logs`

Lý do:
- Đây là các phần nâng cao.
- Giữ lại trong repo ở mức backend/placeholders nếu cần mở rộng sau.
- Không để chúng làm rối bản test khung hiện tại.

## Các placeholder đang được giữ chủ động

- Avatar người dùng mới là placeholder UI.
- Một số vùng dashboard vẫn đóng vai trò trình bày khung mở rộng.
- Một số API nâng cao có thể vẫn còn trong code để tiện phát triển sprint sau.

## Phần phát triển sau

Danh sách ưu tiên hợp lý sau khi chốt được khung:

1. Excel export
2. Weekly summary email
3. AI categorization nâng cao
4. Timeline giao dịch đẹp hơn
5. Admin category management
6. Security alert khi login thiết bị lạ
7. Upload biên lai

## Khi nào nên dừng ở mức này

Nếu mục tiêu hiện tại là:
- có giao diện để thuyết trình,
- có luồng chạy được để demo,
- có khung DB và role rõ ràng,

thì bản hiện tại là đủ để cả nhóm bắt đầu chia backend/frontend tiếp mà không bị quá tải.

## Cách test nhanh

1. Chạy app:
```powershell
dotnet run --launch-profile http
```

2. Mở các trang chính:
- `http://localhost:5176/home/index.html`
- `http://localhost:5176/home/login.html`
- `http://localhost:5176/home/dashboard.html`
- `http://localhost:5176/home/wallets.html`
- `http://localhost:5176/home/transactions.html`
- `http://localhost:5176/home/budgets.html`
- `http://localhost:5176/home/admin-dashboard.html`
- `http://localhost:5176/home/admin-users.html`

3. Nếu đang dùng seed dev:
- `demo@smartspend.local / Demo123!`
- `admin@smartspend.local / Admin123!`
