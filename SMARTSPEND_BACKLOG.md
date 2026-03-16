# SMARTSPEND BACKLOG

Updated: 2026-03-16

## Rules
- Status: `To do` / `In progress` / `Done`
- Priority: `Must` / `Should` / `Nice`
- Owners:
`A = Backend/API + DB`
`B = AI/Automation + Email/Jobs`
`C = Frontend Integration`
`D = UI/UX + Design System`

## Project scope
- Product: `SmartSpend` personal finance web app
- Delivery shape: desktop-first web, tablet-safe, not a mobile app
- Roles in v1: `StandardUser`, `SystemAdmin`
- AI in v1: regex + keyword dictionary + rule-based forecast

## Done

| ID | Priority | Area | Task | Owner | Notes |
|---|---|---|---|---|---|
| D-001 | Must | Foundation | Re-scope repo from `AI Study` to `SmartSpend` | Leader + All | Domain, pages, APIs moved to finance use case |
| D-002 | Must | DB | SmartSpend schema + migration reset | A | `Users`, `Wallets`, `Categories`, `Transactions`, `Budgets`, `BudgetAlerts`, `Keywords`, `AuditLogs`, `Transfers` |
| D-003 | Must | Security | Role model reduced to `StandardUser` + `SystemAdmin` | A | Role seed updated |
| D-004 | Must | Auth | Register/Login/JWT flow working | A | `/api/auth/register`, `/api/auth/login`, `/api/auth/me` |
| D-005 | Must | OTP | OTP supports register verify and password reset | B | verify/resend/reset flow available |
| D-006 | Must | User | Profile API + change password | A | Profile kept and adapted for SmartSpend |
| D-007 | Must | Frontend | Public pages: Home, About, Guide | C/D | SmartSpend landing and onboarding pages available |
| D-008 | Must | Frontend | Auth pages: Login, Register, OTP, Forgot Password, Reset Password | C | Connected to current auth API |
| D-009 | Must | Frontend | User pages: Dashboard, Wallets, Transactions, Budgets, Profile | C/D | Shared shell/sidebar/topbar/theme in place |
| D-010 | Must | Frontend | Admin pages: Dashboard, Users, Keywords, Audit Logs | C/D | Basic admin UI available |
| D-011 | Must | Backend | Wallet CRUD + transfer + StandardUser 3-wallet limit | A | Ownership guard included |
| D-012 | Must | Backend | Transaction CRUD + filters + ownership guard | A | Income/Expense flow ready |
| D-013 | Must | Backend | Budget API with progress calculation by month/category | A | Progress derived from transactions |
| D-014 | Must | Backend | Dashboard aggregate API | A | balance, month totals, trend, budget progress, insights, forecasts |
| D-015 | Must | AI | Smart input parser v1 | B | amount/date/category suggestion via regex + keywords |
| D-016 | Must | Real-time | SignalR budget alerts | A/B | warning and danger alerts pushed without reload |
| D-017 | Should | Admin | User management + keyword CRUD + audit log read | A | admin routes protected by role |
| D-018 | Should | Dev Experience | Development auto-seed + demo accounts + sample finance data | A/B | demo/admin login and seeded wallets, transactions, budgets, alerts |
| D-019 | Should | QA | API smoke verified | A/C | demo login, admin login, dashboard, wallets, budgets, alerts, admin users |
| D-020 | Should | QA | Unit tests verified | A/B | `5/5` xUnit tests passed via `dotnet vstest` |
| D-021 | Must | Docs | README runbook updated | Leader | local run, docker note, seed note, demo credentials |

## In progress

| ID | Priority | Area | Task | Owner | Next action |
|---|---|---|---|---|---|
| P-001 | Must | Security | Remove secrets from tracked config before public push | A | move SMTP and sensitive values to env/User Secrets |
| P-002 | Must | UI/UX | Final SmartSpend visual polish | C/D | tighten typography, chart labels, spacing, empty states |
| P-003 | Should | Docs | Add ERD + role matrix to report/slide assets | Leader + A | export simple diagrams for submission |
| P-004 | Should | Demo | Prepare demo script and screenshots from seeded data | C/D | capture dashboard, wallets, budgets, admin flow |
| P-005 | Should | QA | Expand tests around wallet, budget, transaction permissions | A | add happy-path and forbidden-path coverage |

## To do

| ID | Priority | Area | Task | Owner | Definition of done |
|---|---|---|---|---|---|
| T-001 | Must | Backend | Export Excel for transaction history | A + C | user/admin can download `.xlsx` with active filters |
| T-002 | Should | Backend | Receipt image upload for transaction | A + C | local upload path or simple file storage works |
| T-003 | Should | Automation | Weekly summary email job | B | Monday report can be triggered and formatted |
| T-004 | Should | Security | Strange login warning email | B | simple rule-based mail notification exists |
| T-005 | Should | Admin | Broader system stats on admin dashboard | A | more than users/transactions/keywords counts |
| T-006 | Should | Frontend | Transaction timeline view polish | C/D | clearer grouped timeline for history page |
| T-007 | Should | Frontend | Dark/light mode persistence review | C/D | theme remains stable across pages and reload |
| T-008 | Should | Cleanup | Remove any remaining `AI Study` naming or stale assets | All | no confusing old domain wording left |
| T-009 | Must | DevOps | Final docker submission verification | A | fresh docker run reaches app + db correctly |
| T-010 | Must | Delivery | Slide/report/video checklist | All | final presentation package complete |

## Suggested next sprint

1. Finish config secret cleanup before any public push.
2. Complete Excel export because it is useful and presentation-friendly.
3. Polish dashboard/budget UI with the seeded demo data.
4. Add 3-5 more tests for wallet limit, ownership, and budget alert thresholds.
5. Freeze demo script and record screenshots/video.

## Minimum page map

- Public:
`Home`, `About`, `Guide`, `Login`, `Register`, `OTP`, `Forgot Password`, `Reset Password`
- User:
`Dashboard`, `Wallets`, `Transactions`, `Budgets`, `Profile`
- Admin:
`Admin Dashboard`, `User Management`, `Keyword Management`, `Audit Logs`

## Team assignment file

- Daily A/B/C/D board:
`SMARTSPEND_TEAM_ASSIGNMENT_14D.md`
