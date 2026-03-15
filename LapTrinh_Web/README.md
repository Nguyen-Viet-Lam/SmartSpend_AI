# SmartSpend AI (Local Only - No Docker)

## Run local

```powershell
cd C:\Users\ASUS\source\repos\LapTrinhWeb\LapTrinh_Web\LapTrinh_Web
dotnet ef database update
dotnet run --launch-profile http
```

Open: `http://localhost:5046`

## Gmail App Password setup

1. Open Google Account > Security.
2. Enable 2-Step Verification.
3. Open App passwords and create a new password for `Mail`.
4. Paste values in `appsettings.Development.json`.

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "your-gmail@gmail.com",
  "SenderName": "SmartSpend AI",
  "AppPassword": "xxxx xxxx xxxx xxxx"
}
```

## Auth APIs

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/verify-otp`
- `POST /api/auth/resend-otp`
- `POST /api/auth/logout`

## Asynchronous processing

- OTP email is queued (non-blocking) then sent by background worker.
- Components:
  - `IEmailDispatchQueue`
  - `EmailDispatchQueue` (Channel-based queue)
  - `EmailDispatchBackgroundService`

## Static UI structure for future frontend/backend split

- Core scripts:
  - `wwwroot/js/app/core/api-client.js`
  - `wwwroot/js/app/core/theme-manager.js`
- Feature scripts:
  - `wwwroot/js/app/auth/auth-page.js`
  - `wwwroot/js/app/dashboard/dashboard-page.js`
  - `wwwroot/js/app/transactions/transactions-page.js`
  - `wwwroot/js/app/wallet/wallet-page.js`
  - `wwwroot/js/app/admin/admin-page.js`

## Static pages

- `/Auth/Login`
- `/Auth/Register`
- `/Auth/Otp`
- `/Dashboard`
- `/Transactions`
- `/Wallet`
- `/Admin`
