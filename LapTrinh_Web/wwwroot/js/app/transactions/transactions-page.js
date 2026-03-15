(() => {
    const storage = window.smartSpendStorage;
    const form = document.querySelector("[data-tx-form]");
    if (!storage || !form) {
        return;
    }

    const api = window.smartSpendApi;
    const walletsKey = "smartspend-wallets";
    const transactionsKey = "smartspend-transactions";

    const descriptionInput = form.querySelector("[data-tx-description]");
    const amountInput = form.querySelector("[data-tx-amount]");
    const walletSelect = form.querySelector("[data-tx-wallet]");
    const categoryInput = form.querySelector("[data-tx-category]");
    const typeSelect = form.querySelector("[data-tx-type]");
    const occurredAtInput = form.querySelector("[data-tx-occurred]");
    const suggestionEl = form.querySelector("[data-tx-suggestion]");
    const statusEl = form.querySelector("[data-tx-status]");
    const timelineEl = document.querySelector("[data-tx-timeline]");
    const tableEl = document.querySelector("[data-tx-table]");
    const resetButton = form.querySelector("[data-tx-reset]");

    if (!descriptionInput || !amountInput || !walletSelect || !categoryInput || !typeSelect || !occurredAtInput || !suggestionEl || !statusEl || !timelineEl || !tableEl) {
        return;
    }

    const keywordMap = [
        { key: "xang", category: "Di chuyen" },
        { key: "grab", category: "Di chuyen" },
        { key: "bus", category: "Di chuyen" },
        { key: "an", category: "An uong" },
        { key: "com", category: "An uong" },
        { key: "cafe", category: "Giai tri" },
        { key: "netflix", category: "Giai tri" },
        { key: "dien", category: "Hoa don" },
        { key: "nuoc", category: "Hoa don" },
        { key: "luong", category: "Thu nhap" },
        { key: "thuong", category: "Thu nhap" },
        { key: "freelance", category: "Thu nhap" }
    ];

    const walletTypeText = {
        Cash: "Tien mat",
        Bank: "Ngan hang",
        Savings: "Tiet kiem",
        Other: "Khac"
    };

    const currency = (value) => `${Math.round(Number(value || 0)).toLocaleString("vi-VN")} d`;
    const toShortDate = (value) => new Date(value).toLocaleDateString("vi-VN");
    const toTimelineDate = (value) => {
        const date = new Date(value);
        return `${date.toLocaleDateString("vi-VN")} ${date.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" })}`;
    };

    const makeId = () => {
        if (window.crypto?.randomUUID) {
            return window.crypto.randomUUID();
        }

        return `id-${Date.now()}-${Math.floor(Math.random() * 1_000_000)}`;
    };

    const setStatus = (text, type) => {
        statusEl.textContent = text;
        statusEl.className = `status-text ${type}`;
    };

    const setSuggestion = (text, type) => {
        suggestionEl.textContent = text;
        suggestionEl.className = `status-text ${type}`;
    };

    const readWallets = () => {
        const wallets = storage.read(walletsKey, []);
        if (Array.isArray(wallets) && wallets.length) {
            return wallets;
        }

        const seed = [
            { id: makeId(), name: "Vi tien mat", walletType: "Cash", balance: 2_400_000, isActive: true },
            { id: makeId(), name: "Tai khoan ngan hang", walletType: "Bank", balance: 8_200_000, isActive: true },
            { id: makeId(), name: "Tiet kiem", walletType: "Savings", balance: 15_000_000, isActive: true }
        ];

        storage.write(walletsKey, seed);
        return seed;
    };

    const readTransactions = () => {
        const transactions = storage.read(transactionsKey, []);
        if (Array.isArray(transactions) && transactions.length) {
            return transactions;
        }

        const wallets = readWallets();
        const cashWallet = wallets[0];
        const bankWallet = wallets[1];
        const now = Date.now();

        const seed = [
            {
                id: makeId(),
                walletId: cashWallet?.id,
                walletName: cashWallet?.name || "Vi tien mat",
                type: "Expense",
                amount: 120_000,
                description: "Do xang xe",
                category: "Di chuyen",
                occurredAt: new Date(now - 60 * 60 * 1000).toISOString()
            },
            {
                id: makeId(),
                walletId: bankWallet?.id,
                walletName: bankWallet?.name || "Tai khoan ngan hang",
                type: "Income",
                amount: 3_000_000,
                description: "Luong part-time",
                category: "Thu nhap",
                occurredAt: new Date(now - 26 * 60 * 60 * 1000).toISOString()
            }
        ];

        storage.write(transactionsKey, seed);
        return seed;
    };

    const writeWallets = (wallets) => {
        storage.write(walletsKey, wallets);
        window.dispatchEvent(new CustomEvent("smartspend:wallets-changed", { detail: wallets }));
    };

    const writeTransactions = (transactions) => {
        storage.write(transactionsKey, transactions);
        window.dispatchEvent(new CustomEvent("smartspend:transactions-changed", { detail: transactions }));
    };

    const suggestCategory = (text, type) => {
        if (type === "Income") {
            return "Thu nhap";
        }

        const normalized = (text || "").toLowerCase();
        if (!normalized) {
            return "";
        }

        const found = keywordMap.find((x) => normalized.includes(x.key));
        return found?.category || "Khac";
    };

    const renderWalletOptions = () => {
        const wallets = readWallets().filter((wallet) => wallet.isActive !== false);
        const oldValue = walletSelect.value;
        walletSelect.innerHTML = "<option value=''>Chon vi...</option>";

        wallets.forEach((wallet) => {
            const option = document.createElement("option");
            option.value = wallet.id;
            option.textContent = `${wallet.name} (${walletTypeText[wallet.walletType] || wallet.walletType})`;
            walletSelect.appendChild(option);
        });

        if (wallets.some((wallet) => wallet.id === oldValue)) {
            walletSelect.value = oldValue;
        } else if (wallets.length) {
            walletSelect.value = wallets[0].id;
        }
    };

    const toSortedTransactions = (transactions) => {
        return [...transactions].sort((a, b) => new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime());
    };

    const renderTimeline = () => {
        const transactions = toSortedTransactions(readTransactions()).slice(0, 10);
        if (!transactions.length) {
            timelineEl.innerHTML = "<li><div></div><div class='empty-row'>Chua co giao dich nao.</div></li>";
            return;
        }

        timelineEl.innerHTML = transactions
            .map((item) => {
                const dotClass = item.type === "Income" ? "income" : "expense";
                return `
                    <li>
                        <span class="dot ${dotClass}"></span>
                        <div>
                            <strong>${item.description}</strong>
                            <p>${toTimelineDate(item.occurredAt)} - ${currency(item.amount)} - ${item.category || "Khac"}</p>
                        </div>
                    </li>
                `;
            })
            .join("");
    };

    const renderTable = () => {
        const transactions = toSortedTransactions(readTransactions());
        if (!transactions.length) {
            tableEl.innerHTML = "<tr><td colspan='6' class='empty-row'>Chua co du lieu giao dich.</td></tr>";
            return;
        }

        tableEl.innerHTML = transactions
            .map((item) => {
                const isIncome = item.type === "Income";
                const amountText = isIncome ? `+${currency(item.amount)}` : `-${currency(item.amount)}`;
                return `
                    <tr>
                        <td>${toShortDate(item.occurredAt)}</td>
                        <td>${item.description}</td>
                        <td>${item.walletName || "-"}</td>
                        <td>${item.category || "Khac"}</td>
                        <td><span class="chip ${isIncome ? "income" : "expense"}">${isIncome ? "Thu" : "Chi"}</span></td>
                        <td>${amountText}</td>
                    </tr>
                `;
            })
            .join("");
    };

    const renderAll = () => {
        renderWalletOptions();
        renderTimeline();
        renderTable();
    };

    const setNowDefault = () => {
        const now = new Date();
        const tzOffsetMs = now.getTimezoneOffset() * 60 * 1000;
        const localIso = new Date(now.getTime() - tzOffsetMs).toISOString().slice(0, 16);
        occurredAtInput.value = localIso;
    };

    const applySuggestion = () => {
        const suggested = suggestCategory(descriptionInput.value, typeSelect.value);
        if (suggested) {
            categoryInput.value = suggested;
            setSuggestion(`AI goi y danh muc: ${suggested}`, "info");
        } else {
            setSuggestion("Nhap noi dung de nhan goi y category.", "pending");
        }
    };

    const adjustWalletBalance = (walletId, transactionType, amount) => {
        const wallets = readWallets();
        const wallet = wallets.find((item) => item.id === walletId);
        if (!wallet) {
            return;
        }

        const delta = transactionType === "Income" ? amount : -amount;
        wallet.balance = Number(wallet.balance || 0) + delta;
        writeWallets(wallets);
    };

    const syncTransactionToApi = async (transaction) => {
        if (!api?.transactions?.create) {
            return;
        }

        try {
            await api.transactions.create({
                walletId: transaction.walletId,
                categoryId: null,
                transactionType: transaction.type,
                amount: Number(transaction.amount),
                description: transaction.description,
                occurredAtUtc: transaction.occurredAt
            });
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da luu local. API giao dich chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da luu local, nhung sync API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const description = (descriptionInput.value || "").trim();
        const category = (categoryInput.value || "").trim() || "Khac";
        const walletId = walletSelect.value;
        const txType = typeSelect.value;
        const amount = Number(amountInput.value || 0);
        const occurredAtRaw = occurredAtInput.value;

        if (description.length < 2) {
            setStatus("Noi dung can toi thieu 2 ky tu.", "error");
            return;
        }

        if (!walletId) {
            setStatus("Ban can chon vi.", "error");
            return;
        }

        if (!Number.isFinite(amount) || amount <= 0) {
            setStatus("So tien phai lon hon 0.", "error");
            return;
        }

        const occurredAt = occurredAtRaw ? new Date(occurredAtRaw).toISOString() : new Date().toISOString();
        const wallets = readWallets();
        const wallet = wallets.find((item) => item.id === walletId);

        const transaction = {
            id: makeId(),
            walletId,
            walletName: wallet?.name || "Khong ro",
            type: txType === "Income" ? "Income" : "Expense",
            amount,
            description,
            category,
            occurredAt
        };

        const transactions = readTransactions();
        transactions.push(transaction);
        writeTransactions(transactions);
        adjustWalletBalance(walletId, transaction.type, amount);
        renderAll();
        setStatus("Da luu giao dich thanh cong.", "success");

        form.reset();
        setNowDefault();
        renderWalletOptions();
        typeSelect.value = "Expense";
        applySuggestion();

        await syncTransactionToApi(transaction);
    });

    resetButton?.addEventListener("click", () => {
        form.reset();
        setNowDefault();
        typeSelect.value = "Expense";
        renderWalletOptions();
        applySuggestion();
        setStatus("Form da reset.", "pending");
    });

    descriptionInput.addEventListener("input", applySuggestion);
    typeSelect.addEventListener("change", applySuggestion);

    window.addEventListener("smartspend:wallets-changed", renderAll);
    window.addEventListener("storage", renderAll);

    setNowDefault();
    renderAll();
    applySuggestion();
    setStatus("San sang nhap giao dich nhanh.", "pending");
    if (!readWallets().length) {
        setStatus("Chua co vi nao, hay tao vi trong trang Wallet truoc.", "warning");
    });
})();
