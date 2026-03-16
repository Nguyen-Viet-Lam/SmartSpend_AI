(function () {
  const appReadyHandlers = {
    dashboard: initDashboardPage,
    wallets: initWalletsPage,
    transactions: initTransactionsPage,
    budgets: initBudgetsPage,
    profile: initProfilePage,
    "admin-dashboard": initAdminDashboardPage,
    "admin-users": initAdminUsersPage,
  };

  function message(targetId, text, tone) {
    const element = document.getElementById(targetId);
    if (!element) {
      return;
    }

    if (!text) {
      element.hidden = true;
      element.textContent = "";
      element.className = "message";
      return;
    }

    element.hidden = false;
    element.textContent = text;
    element.className = `message ${tone || ""}`.trim();
  }

  function budgetTone(status) {
    const value = String(status || "").toLowerCase();
    if (value.includes("danger")) {
      return "danger";
    }

    if (value.includes("warning")) {
      return "warning";
    }

    return "safe";
  }

  function statusBadge(status) {
    const tone = budgetTone(status);
    return `<span class="status-pill status-${tone}">${window.SmartSpendApp.escapeHtml(status || "Safe")}</span>`;
  }

  function renderTrendChart(points) {
    const container = document.getElementById("trendChart");
    if (!container) {
      return;
    }

    if (!Array.isArray(points) || points.length === 0) {
      container.innerHTML = '<div class="empty-state">Chưa có dữ liệu đủ để dựng biểu đồ thu - chi.</div>';
      return;
    }

    const max = Math.max(...points.flatMap((item) => [Number(item.income || 0), Number(item.expense || 0)]), 1);
    container.innerHTML = points
      .map((item) => {
        const incomeHeight = Math.max(18, Math.round((Number(item.income || 0) / max) * 180));
        const expenseHeight = Math.max(18, Math.round((Number(item.expense || 0) / max) * 180));
        return `
          <div class="trend-bar">
            <div style="display:grid; gap:8px; align-items:end; min-height:190px;">
              <span style="height:${incomeHeight}px; background:linear-gradient(180deg, #42d392, #21b77b);"></span>
              <span style="height:${expenseHeight}px; background:linear-gradient(180deg, #ff9b73, #ff5b6e);"></span>
            </div>
            <strong>${window.SmartSpendApp.escapeHtml(item.label)}</strong>
            <div class="muted" style="font-size:0.84rem;">Thu ${window.SmartSpendApp.formatCurrency(item.income)}<br>Chi ${window.SmartSpendApp.formatCurrency(item.expense)}</div>
          </div>`;
      })
      .join("");
  }

  function renderExpenseDonut(items) {
    const donut = document.getElementById("expenseDonut");
    const legend = document.getElementById("expenseLegend");
    if (!donut || !legend) {
      return;
    }

    if (!Array.isArray(items) || items.length === 0) {
      donut.style.background = "conic-gradient(#31465f 0deg 360deg)";
      legend.innerHTML = '<div class="empty-state">Chưa có giao dịch chi tiêu trong tháng.</div>';
      return;
    }

    const total = items.reduce((sum, item) => sum + Number(item.amount || 0), 0) || 1;
    let currentAngle = 0;
    const segments = items.map((item) => {
      const ratio = Number(item.amount || 0) / total;
      const nextAngle = currentAngle + ratio * 360;
      const segment = `${item.color || "#48d1a0"} ${currentAngle}deg ${nextAngle}deg`;
      currentAngle = nextAngle;
      return segment;
    });

    donut.style.background = `conic-gradient(${segments.join(",")})`;
    legend.innerHTML = items
      .map(
        (item) => `
          <div class="legend-item">
            <div class="inline-actions">
              <span class="legend-swatch" style="background:${window.SmartSpendApp.escapeHtml(item.color || "#48d1a0")}"></span>
              <span>${window.SmartSpendApp.escapeHtml(item.categoryName)}</span>
            </div>
            <strong>${window.SmartSpendApp.formatCurrency(item.amount)}</strong>
          </div>`
      )
      .join("");
  }

  function renderBudgetProgress(items, targetId) {
    const container = document.getElementById(targetId);
    if (!container) {
      return;
    }

    if (!Array.isArray(items) || items.length === 0) {
      container.innerHTML = '<div class="empty-state">Chưa thiết lập ngân sách nào trong tháng này.</div>';
      return;
    }

    container.innerHTML = items
      .map((item) => {
        const tone = budgetTone(item.status);
        const percentage = Math.min(Number(item.progressPercentage || 0), 100);
        return `
          <div class="budget-row">
            <div class="panel-header" style="margin:0;">
              <div>
                <strong>${window.SmartSpendApp.escapeHtml(item.categoryName)}</strong>
                <div class="muted">Đã dùng ${window.SmartSpendApp.formatCurrency(item.spentAmount)} / ${window.SmartSpendApp.formatCurrency(item.limitAmount)}</div>
              </div>
              ${statusBadge(item.status)}
            </div>
            <div class="progress-track"><div class="progress-fill ${tone}" style="width:${percentage}%;"></div></div>
          </div>`;
      })
      .join("");
  }

  function renderBulletList(targetId, items, emptyText) {
    const container = document.getElementById(targetId);
    if (!container) {
      return;
    }

    if (!Array.isArray(items) || items.length === 0) {
      container.innerHTML = `<div class="empty-state">${window.SmartSpendApp.escapeHtml(emptyText || "Chưa có dữ liệu.")}</div>`;
      return;
    }

    container.innerHTML = `<ul class="insight-list">${items
      .map((item) => `<li>${window.SmartSpendApp.escapeHtml(item)}</li>`)
      .join("")}</ul>`;
  }

  async function initDashboardPage() {
    try {
      const data = await window.SmartSpendApp.api("/api/dashboard");
      const mappings = {
        totalBalance: data.totalBalance,
        totalIncome: data.totalIncomeThisMonth,
        totalExpense: data.totalExpenseThisMonth,
      };

      Object.entries(mappings).forEach(([id, value]) => {
        const node = document.getElementById(id);
        if (node) {
          node.textContent = window.SmartSpendApp.formatCurrency(value);
        }
      });

      const alertsNode = document.getElementById("dashboardAlertCount");
      if (alertsNode) {
        alertsNode.textContent = String(data.unreadAlerts || 0);
      }

      renderTrendChart(data.monthlyTrend || []);
      renderExpenseDonut(data.expenseBreakdown || []);
      renderBudgetProgress(data.budgetProgress || [], "dashboardBudgets");
      renderBulletList("insightsList", data.insights || [], "Chưa có insight nào.");
      renderBulletList("forecastList", data.forecasts || [], "Chưa có dự báo nào.");
    } catch (error) {
      message("dashboardMessage", error.message, "error");
    }
  }

  async function initWalletsPage() {
    const walletForm = document.getElementById("walletForm");
    const transferForm = document.getElementById("transferForm");
    const walletList = document.getElementById("walletList");
    const walletNameInput = document.getElementById("walletName");
    const walletTypeSelect = document.getElementById("walletType");
    const walletInitialBalanceInput = document.getElementById("walletInitialBalance");
    const walletIsDefaultInput = document.getElementById("walletIsDefault");
    const fromWalletSelect = document.getElementById("fromWalletId");
    const toWalletSelect = document.getElementById("toWalletId");
    const transferAmountInput = document.getElementById("transferAmount");
    const transferDateInput = document.getElementById("transferDate");
    const transferNoteInput = document.getElementById("transferNote");
    let editingWalletId = null;
    let wallets = [];

    function updateTransferOptions() {
      if (!fromWalletSelect || !toWalletSelect) {
        return;
      }

      const options = ['<option value="">Chọn ví</option>']
        .concat(
          wallets.map(
            (item) => `<option value="${item.walletId}">${window.SmartSpendApp.escapeHtml(item.name)} (${window.SmartSpendApp.formatCurrency(item.balance)})</option>`
          )
        )
        .join("");

      fromWalletSelect.innerHTML = options;
      toWalletSelect.innerHTML = options;
    }

    function resetWalletForm() {
      editingWalletId = null;
      walletForm?.reset();
      const submit = walletForm?.querySelector("button[type='submit']");
      if (submit) {
        submit.textContent = "Lưu ví";
      }
      const cancel = document.getElementById("cancelWalletEdit");
      if (cancel) {
        cancel.hidden = true;
      }
    }

    async function loadWallets() {
      wallets = await window.SmartSpendApp.api("/api/wallets");
      updateTransferOptions();

      const totalNode = document.getElementById("walletTotalBalance");
      if (totalNode) {
        const total = wallets.reduce((sum, item) => sum + Number(item.balance || 0), 0);
        totalNode.textContent = window.SmartSpendApp.formatCurrency(total);
      }

      const countNode = document.getElementById("walletCount");
      if (countNode) {
        countNode.textContent = String(wallets.length);
      }

      if (!walletList) {
        return;
      }

      if (wallets.length === 0) {
        walletList.innerHTML = '<div class="empty-state">Chưa có ví nào. Tạo ví đầu tiên để bắt đầu ghi nhận chi tiêu.</div>';
        return;
      }

      walletList.innerHTML = wallets
        .map(
          (wallet) => `
            <article class="wallet-card">
              <div class="panel-header" style="margin-bottom:8px;">
                <div>
                  <div class="wallet-badge">${wallet.isDefault ? "★" : wallet.type.slice(0, 1)}</div>
                  <h3>${window.SmartSpendApp.escapeHtml(wallet.name)}</h3>
                </div>
                ${wallet.isDefault ? '<span class="tag tag-safe">Mặc định</span>' : ''}
              </div>
              <p class="muted">Loại ví: ${window.SmartSpendApp.escapeHtml(wallet.type)}</p>
              <div class="metric-value">${window.SmartSpendApp.formatCurrency(wallet.balance)}</div>
              <div class="muted">Tạo ngày ${window.SmartSpendApp.formatDate(wallet.createdAt)}</div>
              <div class="table-actions">
                <button class="btn btn-ghost" data-edit-wallet="${wallet.walletId}">Sửa</button>
                <button class="btn btn-ghost" data-delete-wallet="${wallet.walletId}">Xóa</button>
              </div>
            </article>`
        )
        .join("");

      walletList.querySelectorAll("[data-edit-wallet]").forEach((button) => {
        button.addEventListener("click", () => {
          const wallet = wallets.find((item) => item.walletId === Number(button.dataset.editWallet));
          if (!wallet || !walletForm) {
            return;
          }

          editingWalletId = wallet.walletId;
          walletNameInput.value = wallet.name;
          walletTypeSelect.value = wallet.type;
          walletInitialBalanceInput.value = wallet.balance;
          walletIsDefaultInput.checked = Boolean(wallet.isDefault);
          walletForm.querySelector("button[type='submit']").textContent = "Cập nhật ví";
          document.getElementById("cancelWalletEdit").hidden = false;
          window.scrollTo({ top: 0, behavior: "smooth" });
        });
      });

      walletList.querySelectorAll("[data-delete-wallet]").forEach((button) => {
        button.addEventListener("click", async () => {
          if (!window.confirm("Bạn có chắc muốn xóa ví này?")) {
            return;
          }

          try {
            await window.SmartSpendApp.api(`/api/wallets/${button.dataset.deleteWallet}`, { method: "DELETE" });
            window.SmartSpendApp.showToast("Đã xóa ví.", "success");
            await loadWallets();
            resetWalletForm();
          } catch (error) {
            message("walletMessage", error.message, "error");
          }
        });
      });
    }

    walletForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("walletMessage", "", "");
      try {
        const payload = {
          name: walletNameInput.value,
          type: walletTypeSelect.value,
          initialBalance: Number(walletInitialBalanceInput.value || 0),
          isDefault: walletIsDefaultInput.checked,
        };

        await window.SmartSpendApp.api(editingWalletId ? `/api/wallets/${editingWalletId}` : "/api/wallets", {
          method: editingWalletId ? "PUT" : "POST",
          body: payload,
        });

        window.SmartSpendApp.showToast(editingWalletId ? "Đã cập nhật ví." : "Đã tạo ví mới.", "success");
        resetWalletForm();
        await loadWallets();
      } catch (error) {
        message("walletMessage", error.message, "error");
      }
    });

    transferForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("transferMessage", "", "");
      try {
        await window.SmartSpendApp.api("/api/wallets/transfer", {
          method: "POST",
          body: {
            fromWalletId: Number(fromWalletSelect.value),
            toWalletId: Number(toWalletSelect.value),
            amount: Number(transferAmountInput.value),
            note: transferNoteInput.value,
            transferDate: transferDateInput.value ? `${transferDateInput.value}T00:00:00Z` : null,
          },
        });

        transferForm.reset();
        message("transferMessage", "Chuyển tiền thành công.", "success");
        await loadWallets();
      } catch (error) {
        message("transferMessage", error.message, "error");
      }
    });

    document.getElementById("cancelWalletEdit")?.addEventListener("click", resetWalletForm);
    await loadWallets();
  }

  async function initTransactionsPage() {
    const transactionForm = document.getElementById("transactionForm");
    const filterForm = document.getElementById("transactionFilterForm");
    const tableBody = document.getElementById("transactionTableBody");
    let wallets = [];
    let categories = [];
    let transactions = [];
    let editingTransactionId = null;

    function setSelectOptions(selectId, items, placeholder) {
      const select = document.getElementById(selectId);
      if (!select) {
        return;
      }

      select.innerHTML = [`<option value="">${placeholder}</option>`]
        .concat(items.map((item) => `<option value="${item.walletId || item.categoryId}">${window.SmartSpendApp.escapeHtml(item.name || item.categoryName)}</option>`))
        .join("");
    }

    async function loadLookups() {
      wallets = await window.SmartSpendApp.api("/api/wallets");
      categories = await window.SmartSpendApp.api("/api/categories");

      setSelectOptions("walletId", wallets, "Chọn ví");
      setSelectOptions("filterWalletId", wallets, "Tất cả ví");
      setSelectOptions("categoryId", categories, "Chọn danh mục");
      setSelectOptions("filterCategoryId", categories, "Tất cả danh mục");

      if (wallets.length > 0 && transactionForm && !transactionForm.walletId.value) {
        const defaultWallet = wallets.find((item) => item.isDefault) || wallets[0];
        transactionForm.walletId.value = String(defaultWallet.walletId);
      }
    }

    function resetTransactionForm() {
      editingTransactionId = null;
      transactionForm?.reset();
      transactionForm.transactionDate.value = new Date().toISOString().slice(0, 10);
      transactionForm.querySelector("button[type='submit']").textContent = "Lưu giao dịch";
      document.getElementById("cancelTransactionEdit").hidden = true;
      if (wallets.length > 0) {
        const defaultWallet = wallets.find((item) => item.isDefault) || wallets[0];
        transactionForm.walletId.value = String(defaultWallet.walletId);
      }
    }

    function buildQueryFromFilters() {
      const params = new URLSearchParams();
      if (filterForm.from.value) params.set("from", filterForm.from.value);
      if (filterForm.to.value) params.set("to", filterForm.to.value);
      if (filterForm.walletId.value) params.set("walletId", filterForm.walletId.value);
      if (filterForm.categoryId.value) params.set("categoryId", filterForm.categoryId.value);
      if (filterForm.type.value) params.set("type", filterForm.type.value);
      return params.toString();
    }

    async function loadTransactions() {
      const query = filterForm ? buildQueryFromFilters() : "";
      transactions = await window.SmartSpendApp.api(`/api/transactions${query ? `?${query}` : ""}`);
      const totalNode = document.getElementById("transactionCount");
      if (totalNode) {
        totalNode.textContent = String(transactions.length);
      }

      if (!tableBody) {
        return;
      }

      if (transactions.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7"><div class="empty-state">Chưa có giao dịch phù hợp bộ lọc hiện tại.</div></td></tr>';
        return;
      }

      tableBody.innerHTML = transactions
        .map(
          (item) => `
            <tr>
              <td>${window.SmartSpendApp.formatDate(item.transactionDate)}</td>
              <td>${window.SmartSpendApp.escapeHtml(item.note)}</td>
              <td>${window.SmartSpendApp.escapeHtml(item.walletName)}</td>
              <td>${window.SmartSpendApp.escapeHtml(item.categoryName)}</td>
              <td>${statusBadge(item.type === "Income" ? "Safe" : "Warning")}</td>
              <td><strong>${window.SmartSpendApp.formatCurrency(item.amount)}</strong></td>
              <td>
                <div class="table-actions">
                  <button class="btn btn-ghost" data-edit-transaction="${item.transactionId}">Sửa</button>
                  <button class="btn btn-ghost" data-delete-transaction="${item.transactionId}">Xóa</button>
                </div>
              </td>
            </tr>`
        )
        .join("");

      tableBody.querySelectorAll("[data-edit-transaction]").forEach((button) => {
        button.addEventListener("click", () => {
          const transaction = transactions.find((item) => item.transactionId === Number(button.dataset.editTransaction));
          if (!transaction || !transactionForm) {
            return;
          }

          editingTransactionId = transaction.transactionId;
          transactionForm.walletId.value = transaction.walletId;
          transactionForm.categoryId.value = transaction.categoryId;
          transactionForm.type.value = transaction.type;
          transactionForm.amount.value = transaction.amount;
          transactionForm.note.value = transaction.note;
          transactionForm.transactionDate.value = window.SmartSpendApp.formatDateInput(transaction.transactionDate);
          transactionForm.receiptImagePath.value = "";
          transactionForm.querySelector("button[type='submit']").textContent = "Cập nhật giao dịch";
          document.getElementById("cancelTransactionEdit").hidden = false;
          window.location.hash = "quick-add";
          window.scrollTo({ top: 0, behavior: "smooth" });
        });
      });

      tableBody.querySelectorAll("[data-delete-transaction]").forEach((button) => {
        button.addEventListener("click", async () => {
          if (!window.confirm("Bạn có chắc muốn xóa giao dịch này?")) {
            return;
          }

          try {
            await window.SmartSpendApp.api(`/api/transactions/${button.dataset.deleteTransaction}`, { method: "DELETE" });
            window.SmartSpendApp.showToast("Đã xóa giao dịch.", "success");
            await loadTransactions();
          } catch (error) {
            message("transactionMessage", error.message, "error");
          }
        });
      });
    }

    transactionForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("transactionMessage", "", "");
      try {
        await window.SmartSpendApp.api(editingTransactionId ? `/api/transactions/${editingTransactionId}` : "/api/transactions", {
          method: editingTransactionId ? "PUT" : "POST",
          body: {
            walletId: Number(transactionForm.walletId.value),
            categoryId: Number(transactionForm.categoryId.value),
            type: transactionForm.type.value,
            amount: Number(transactionForm.amount.value),
            note: transactionForm.note.value,
            transactionDate: `${transactionForm.transactionDate.value}T00:00:00Z`,
            receiptImagePath: transactionForm.receiptImagePath.value || null,
          },
        });

        message("transactionMessage", editingTransactionId ? "Cập nhật giao dịch thành công." : "Đã tạo giao dịch mới.", "success");
        resetTransactionForm();
        await loadTransactions();
      } catch (error) {
        message("transactionMessage", error.message, "error");
      }
    });

    filterForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      await loadTransactions();
    });

    document.getElementById("resetFilters")?.addEventListener("click", async () => {
      filterForm?.reset();
      await loadTransactions();
    });

    document.getElementById("cancelTransactionEdit")?.addEventListener("click", resetTransactionForm);

    await loadLookups();
    resetTransactionForm();
    await loadTransactions();
  }

  async function initBudgetsPage() {
    const budgetForm = document.getElementById("budgetForm");
    const budgetList = document.getElementById("budgetList");
    let categories = [];

    async function loadCategories() {
      categories = await window.SmartSpendApp.api("/api/categories?type=Expense");
      const select = document.getElementById("budgetCategoryId");
      if (select) {
        select.innerHTML = ['<option value="">Chọn danh mục</option>']
          .concat(categories.map((item) => `<option value="${item.categoryId}">${window.SmartSpendApp.escapeHtml(item.name)}</option>`))
          .join("");
      }
    }

    async function loadBudgets() {
      const monthValue = document.getElementById("budgetMonth").value;
      const params = monthValue ? `?month=${monthValue}-01` : "";
      const budgets = await window.SmartSpendApp.api(`/api/budgets${params}`);
      const count = document.getElementById("budgetCount");
      if (count) {
        count.textContent = String(budgets.length);
      }

      if (!budgetList) {
        return;
      }

      if (budgets.length === 0) {
        budgetList.innerHTML = '<div class="empty-state">Chưa có ngân sách nào cho tháng đang chọn.</div>';
        return;
      }

      budgetList.innerHTML = budgets
        .map(
          (item) => `
            <div class="budget-row">
              <div class="panel-header" style="margin:0;">
                <div>
                  <strong>${window.SmartSpendApp.escapeHtml(item.categoryName)}</strong>
                  <div class="muted">Tháng ${window.SmartSpendApp.formatDate(item.month)}</div>
                </div>
                <div class="table-actions">
                  ${statusBadge(item.status)}
                  <button class="btn btn-ghost" data-delete-budget="${item.budgetId}">Xóa</button>
                </div>
              </div>
              <div class="muted">${window.SmartSpendApp.formatCurrency(item.spentAmount)} / ${window.SmartSpendApp.formatCurrency(item.limitAmount)}</div>
              <div class="progress-track"><div class="progress-fill ${budgetTone(item.status)}" style="width:${Math.min(Number(item.progressPercentage || 0), 100)}%;"></div></div>
            </div>`
        )
        .join("");

      budgetList.querySelectorAll("[data-delete-budget]").forEach((button) => {
        button.addEventListener("click", async () => {
          if (!window.confirm("Xóa ngân sách này?")) {
            return;
          }

          try {
            await window.SmartSpendApp.api(`/api/budgets/${button.dataset.deleteBudget}`, { method: "DELETE" });
            window.SmartSpendApp.showToast("Đã xóa ngân sách.", "success");
            await loadBudgets();
          } catch (error) {
            message("budgetMessage", error.message, "error");
          }
        });
      });
    }

    budgetForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("budgetMessage", "", "");
      try {
        await window.SmartSpendApp.api("/api/budgets", {
          method: "POST",
          body: {
            categoryId: Number(budgetForm.categoryId.value),
            month: `${budgetForm.month.value}-01T00:00:00Z`,
            limitAmount: Number(budgetForm.limitAmount.value),
          },
        });

        message("budgetMessage", "Đã lưu ngân sách.", "success");
        await loadBudgets();
      } catch (error) {
        message("budgetMessage", error.message, "error");
      }
    });

    document.getElementById("reloadBudgets")?.addEventListener("click", async () => {
      await loadBudgets();
    });

    const monthField = document.getElementById("budgetMonth");
    if (monthField && !monthField.value) {
      monthField.value = new Date().toISOString().slice(0, 7);
    }

    await loadCategories();
    await loadBudgets();
  }

  async function initProfilePage() {
    const profileForm = document.getElementById("profileForm");
    const passwordForm = document.getElementById("passwordForm");

    async function loadProfile() {
      const profile = await window.SmartSpendApp.api("/api/profile");
      document.getElementById("profileCreatedAt").textContent = window.SmartSpendApp.formatDate(profile.createdAt);
      document.getElementById("profileEmailVerified").textContent = profile.isEmailVerified ? "Đã xác thực OTP" : "Chưa xác thực";
      document.getElementById("profileLockStatus").textContent = profile.isLocked ? "Đang bị khóa" : "Hoạt động bình thường";
      profileForm.fullName.value = profile.fullName || "";
      profileForm.email.value = profile.email || "";
      profileForm.username.value = profile.username || "";
    }

    profileForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("profileMessage", "", "");
      try {
        await window.SmartSpendApp.api("/api/profile", {
          method: "PUT",
          body: {
            fullName: profileForm.fullName.value,
            email: profileForm.email.value,
          },
        });
        message("profileMessage", "Đã cập nhật hồ sơ.", "success");
      } catch (error) {
        message("profileMessage", error.message, "error");
      }
    });

    passwordForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("passwordMessage", "", "");
      try {
        await window.SmartSpendApp.api("/api/profile/change-password", {
          method: "POST",
          body: {
            currentPassword: passwordForm.currentPassword.value,
            newPassword: passwordForm.newPassword.value,
            confirmPassword: passwordForm.confirmPassword.value,
          },
        });
        passwordForm.reset();
        message("passwordMessage", "Đổi mật khẩu thành công.", "success");
      } catch (error) {
        message("passwordMessage", error.message, "error");
      }
    });

    await loadProfile();
  }

  async function initAdminDashboardPage() {
    try {
      const summary = await window.SmartSpendApp.api("/api/admin/summary");
      document.getElementById("adminNewUsersToday").textContent = String(summary.newUsersToday || 0);
      document.getElementById("adminTransactionsToday").textContent = String(summary.transactionsToday || 0);
      document.getElementById("adminTotalUsers").textContent = String(summary.totalUsers || 0);
      document.getElementById("adminTotalKeywords").textContent = String(summary.totalKeywords || 0);
    } catch (error) {
      message("adminDashboardMessage", error.message, "error");
    }
  }

  async function initAdminUsersPage() {
    const body = document.getElementById("adminUsersBody");

    async function loadUsers() {
      const users = await window.SmartSpendApp.api("/api/admin/users");
      if (!body) {
        return;
      }

      if (users.length === 0) {
        body.innerHTML = '<tr><td colspan="7"><div class="empty-state">Chưa có user nào.</div></td></tr>';
        return;
      }

      body.innerHTML = users
        .map(
          (user) => `
            <tr>
              <td>${user.userId}</td>
              <td>${window.SmartSpendApp.escapeHtml(user.fullName)}</td>
              <td>${window.SmartSpendApp.escapeHtml(user.username)}</td>
              <td>${window.SmartSpendApp.escapeHtml(user.email)}</td>
              <td>${window.SmartSpendApp.escapeHtml(user.role)}</td>
              <td>${user.isLocked ? '<span class="status-pill status-danger">Locked</span>' : '<span class="status-pill status-safe">Active</span>'}</td>
              <td>
                <button class="btn btn-ghost" data-user-action="${user.isLocked ? "unlock" : "lock"}" data-user-id="${user.userId}">
                  ${user.isLocked ? "Mở khóa" : "Khóa"}
                </button>
              </td>
            </tr>`
        )
        .join("");

      body.querySelectorAll("[data-user-action]").forEach((button) => {
        button.addEventListener("click", async () => {
          try {
            await window.SmartSpendApp.api(`/api/admin/users/${button.dataset.userId}/${button.dataset.userAction}`, {
              method: "POST",
            });
            window.SmartSpendApp.showToast("Cập nhật trạng thái user thành công.", "success");
            await loadUsers();
          } catch (error) {
            message("adminUsersMessage", error.message, "error");
          }
        });
      });
    }

    await loadUsers();
  }

  document.addEventListener("smartspend:ready", () => {
    const page = document.body.dataset.page || "";
    const handler = appReadyHandlers[page];
    if (handler) {
      handler().catch((error) => {
        window.SmartSpendApp.showToast(error.message, "error");
      });
    }
  });
})();
