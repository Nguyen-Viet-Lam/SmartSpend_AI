# SMARTSPEND AI - TEAM ASSIGNMENT 14 DAYS

Updated: 2026-03-16

## Status rules
- Allowed status: `To do` / `In progress` / `Done`
- Priority tags: `Must` / `Should` / `Nice`
- Owners:
`A = Backend/API + DB`
`B = AI/Automation + Email/Jobs`
`C = Frontend Integration`
`D = UI/UX + Design System`

## Checkpoint 2026-03-16

| Owner | Status | What is already real in repo | Best next move |
|---|---|---|---|
| A | In progress | SmartSpend DB, migration, wallet/transaction/budget/dashboard/admin APIs, SignalR, demo seed | secret cleanup, Excel export, more authz tests |
| B | In progress | OTP register/reset, smart input parser, seeded keyword dictionary, demo data support | weekly email job, login warning mail, AI tests |
| C | In progress | Home/About/Guide/Auth/User/Admin pages wired to SmartSpend API shell | polish forms, charts, empty states, export integration |
| D | In progress | Shared design system base, sidebar shell, dark/light direction, desktop-first layout | typography polish, chart readability, submission screenshots |

## Immediate sprint recommendation

1. `A`: clean secrets from config and finish Excel export.
2. `B`: add weekly summary email draft and more keyword samples.
3. `C`: polish dashboard, budgets, transactions UI using the seeded demo account.
4. `D`: freeze one consistent visual style and prepare slide-ready screenshots.

## Daily assignment board

| Day | Owner | Priority | Task | Status | Output of day |
|---|---|---|---|---|---|
| 1 | A | Must | Final ERD SmartSpend (`Users`, `Wallets`, `Categories`, `Transactions`, `Budgets`) | To do | ERD + reviewed schema |
| 1 | B | Must | Setup SMTP + OTP mail template + env config (no secret hardcode) | To do | SMTP/OTP ready in dev |
| 1 | C | Must | Scaffold public pages: Home, Guide, Login, Register, OTP | To do | Page skeletons run locally |
| 1 | D | Must | UI direction + design tokens (color/type/spacing) for SmartSpend | To do | Mini design system v1 |
| 2 | A | Must | Create entities + migration + seed `Role`, `Category` default | To do | Migration applied |
| 2 | B | Must | Harden OTP flow (verify/resend/attempt limit/error handling) | To do | Stable OTP behavior |
| 2 | C | Must | Connect Login/Register/OTP pages to Auth API | To do | End-to-end auth UI flow |
| 2 | D | Must | Final auth UI responsive (mobile/tablet/desktop) | To do | Auth pages polish |
| 3 | A | Must | Wallet CRUD API + ownership guard | To do | Wallet endpoints done |
| 3 | B | Must | Build keyword dictionary v1 for auto category | To do | Mapping file/service |
| 3 | C | Must | Wallet page UI + connect Wallet API | To do | Wallet management page |
| 3 | D | Should | Fast-input pattern for transaction form | To do | UX spec for fast input |
| 4 | A | Must | Transaction CRUD API + ownership guard | To do | Transaction endpoints done |
| 4 | B | Must | Category suggestion service API (keyword based) | To do | Suggestion endpoint v1 |
| 4 | C | Must | Transaction quick form + transaction list UI | To do | Input + list running |
| 4 | D | Should | Timeline layout for transaction history | To do | Timeline component style |
| 5 | A | Must | Budget API by category/month | To do | Budget endpoints done |
| 5 | B | Should | Spending forecast v1 (moving average) | To do | Forecast service v1 |
| 5 | C | Must | Budget page with progress bars | To do | Budget page integrated |
| 5 | D | Must | Alert visual states (green/amber/red) | To do | Consistent alert UI |
| 6 | A | Must | Dashboard aggregate API (balance, 7d trend, category ratio) | To do | Dashboard payload API |
| 6 | B | Should | Forecast tuning + fallback logic | To do | More stable forecast |
| 6 | C | Must | Integrate dashboard cards/charts with API | To do | Dashboard data live |
| 6 | D | Must | Dashboard visual polish + spacing + typography | To do | Cohesive dashboard UI |
| 7 | A | Must | SignalR Hub + budget alert push trigger | To do | Real-time alert backend |
| 7 | B | Should | Weekly report email job draft (Hangfire) | To do | Job draft running |
| 7 | C | Must | SignalR client for instant budget alerts | To do | Frontend receives alert |
| 7 | D | Must | Responsive pass phase 1 on main user pages | To do | No major breakpoints issue |
| 8 | A | Should | Admin API: list user + lock/unlock | To do | Admin user APIs |
| 8 | B | Nice | Login anomaly email alert basic rule | To do | Suspicious login mail v1 |
| 8 | C | Should | Admin user page (list + lock/unlock actions) | To do | Admin user screen |
| 8 | D | Should | Admin page UI cleanup | To do | Readable admin layout |
| 9 | A | Should | Export Excel API for transaction history | To do | Download endpoint `.xlsx` |
| 9 | B | Should | Unit tests for categorization + forecast | To do | AI module tests added |
| 9 | C | Should | History filters + export button integration | To do | Export flow usable |
| 9 | D | Should | Chart style polish + legends + labels | To do | Better chart readability |
| 10 | A | Should | Category management API (Admin) | To do | Category CRUD admin |
| 10 | B | Should | Improve category dictionary from real input samples | To do | Mapping v2 |
| 10 | C | Should | Category management UI | To do | Category admin page |
| 10 | D | Should | Dark mode toggle + token mapping | To do | Theme switch works |
| 11 | A | Should | Basic system logs API for admin | To do | Logs endpoint basic |
| 11 | B | Should | Finalize weekly email schedule (Monday morning) | To do | Scheduled email confirmed |
| 11 | C | Should | Admin logs page basic viewer | To do | Logs page viewable |
| 11 | D | Must | Accessibility pass (contrast, font, click targets) | To do | A11y checklist pass |
| 12 | A | Must | Security hardening: role policy + data ownership checks | To do | Authz checklist green |
| 12 | B | Should | Integration test for automation flows | To do | Automation smoke tests |
| 12 | C | Must | End-to-end bugfix user/admin flows | To do | Core flow stable |
| 12 | D | Must | Final responsive pass all key pages | To do | Mobile/tablet final pass |
| 13 | A | Must | README runbook: local + env + migrate | To do | Clear setup docs |
| 13 | B | Should | Seed/demo data script for presentation | To do | Demo seed prepared |
| 13 | C | Must | Demo script flow + record support assets | To do | Demo steps validated |
| 13 | D | Should | Slide visuals + screenshot assets | To do | Report/slide assets ready |
| 14 | A | Must | Final merge + release candidate branch + backup tag | To do | RC build prepared |
| 14 | B | Must | Full smoke test on RC build | To do | Smoke test report |
| 14 | C | Must | Last bugfix patch + regression check | To do | Final frontend stable |
| 14 | D | Must | Final QA signoff UI/UX | To do | UI/UX signoff |

## Current quick snapshot (from current repo)

| Owner | Current status | Notes |
|---|---|---|
| A | In progress | Core SmartSpend backend is already running; remaining work is hardening and delivery items |
| B | In progress | OTP and AI parser exist; remaining work is automation and report mail |
| C | In progress | Main SmartSpend pages already exist; remaining work is polish and feature completion |
| D | In progress | Shared UI direction exists; remaining work is consistency and final presentation quality |

## Suggested daily standup format (5-10 mins)

1. Yesterday done (`Done` rows)
2. Today plan (`In progress` rows)
3. Blockers (dependency or owner support needed)
4. Update table status directly after standup
