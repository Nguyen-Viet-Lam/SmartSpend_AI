(() => {
    const storage = window.smartSpendStorage;
    const api = window.smartSpendApi;
    const walletsKey = "smartspend-wallets";

    const tableBody = document.querySelector("[data-wallet-table]");
    const statusEl = document.querySelector("[data-wallet-status]");
    const form = document.querySelector("[data-wallet-form]");
    const resetButton = document.querySelector("[data-wallet-reset]");
    const titleEl = document.querySelector("[data-wallet-form-title]");

    if (!storage || !tableBody || !statusEl || !form || !titleEl) {
        return;
    }

    const walletTypeText = {
        Cash: "Tien mat",
        Bank: "Ngan hang",
        Savings: "Tiet kiem",
        Other: "Khac"
    };

    const currency = (value) => `${Math.round(Number(value || 0)).toLocaleString("vi-VN")} d`;
    const makeId = () => window.crypto?.randomUUID?.() || `id-${Date.now()}-${Math.floor(Math.random() * 1_000_000)}`;

    const setStatus = (text, type) => {
        statusEl.textContent = text;
        statusEl.className = `status-text ${type}`;
    };

    const readWallets = () => {
        const wallets = storage.read(walletsKey, []);
        if (Array.isArray(wallets) && wallets.length) {
            return wallets;
        }

        const seed = [
            { id: makeId(), name: "Vi tien mat", walletType: "Cash", balance: 2_500_000, isActive: true },
            { id: makeId(), name: "Tai khoan ngan hang", walletType: "Bank", balance: 8_000_000, isActive: true },
            { id: makeId(), name: "Tiet kiem", walletType: "Savings", balance: 15_000_000, isActive: true }
        ];

        storage.write(walletsKey, seed);
        return seed;
    };

    const writeWallets = (wallets) => {
        storage.write(walletsKey, wallets);
        window.dispatchEvent(new CustomEvent("smartspend:wallets-changed", { detail: wallets }));
    };

    const render = () => {
        const wallets = readWallets();
        if (!wallets.length) {
            tableBody.innerHTML = "<tr><td colspan='5' class='empty-row'>Chua co vi nao.</td></tr>";
            return;
        }

        tableBody.innerHTML = wallets
            .map((wallet) => {
                const badgeClass = wallet.isActive ? "active" : "locked";
                const badgeText = wallet.isActive ? "Dang hoat dong" : "Tam khoa";
                return `
                    <tr>
                        <td>${wallet.name}</td>
                        <td>${walletTypeText[wallet.walletType] || wallet.walletType}</td>
                        <td>${currency(wallet.balance)}</td>
                        <td><span class="admin-badge ${badgeClass}">${badgeText}</span></td>
                        <td>
                            <div class="inline-actions">
                                <button type="button" class="action-btn" data-wallet-edit="${wallet.id}">Sua</button>
                                <button type="button" class="action-btn danger" data-wallet-delete="${wallet.id}">Xoa</button>
                            </div>
                        </td>
                    </tr>
                `;
            })
            .join("");
    };

    const resetForm = () => {
        form.reset();
        form.elements.id.value = "";
        form.elements.isActive.checked = true;
        titleEl.textContent = "Tao vi moi";
    };

    const fillForm = (wallet) => {
        form.elements.id.value = wallet.id;
        form.elements.name.value = wallet.name;
        form.elements.walletType.value = wallet.walletType;
        form.elements.balance.value = Number(wallet.balance || 0);
        form.elements.isActive.checked = wallet.isActive !== false;
        titleEl.textContent = "Cap nhat vi";
    };

    const syncCreate = async (wallet) => {
        if (!api?.wallets?.create) {
            return;
        }

        try {
            await api.wallets.create({
                name: wallet.name,
                walletType: wallet.walletType,
                initialBalance: Number(wallet.balance || 0)
            });
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da luu local. API wallet chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da luu local, nhung sync create API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    const syncUpdate = async (wallet) => {
        if (!api?.wallets?.update) {
            return;
        }

        try {
            await api.wallets.update(wallet.id, {
                name: wallet.name,
                walletType: wallet.walletType,
                balance: Number(wallet.balance || 0),
                isActive: wallet.isActive !== false
            });
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da cap nhat local. API wallet chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da cap nhat local, nhung sync update API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    const syncDelete = async (walletId) => {
        if (!api?.wallets?.remove) {
            return;
        }

        try {
            await api.wallets.remove(walletId);
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da xoa local. API wallet chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da xoa local, nhung sync delete API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const id = (form.elements.id.value || "").trim();
        const name = (form.elements.name.value || "").trim();
        const walletType = form.elements.walletType.value || "Cash";
        const balance = Number(form.elements.balance.value || 0);
        const isActive = Boolean(form.elements.isActive.checked);

        if (name.length < 2) {
            setStatus("Ten vi toi thieu 2 ky tu.", "error");
            return;
        }

        if (!Number.isFinite(balance) || balance < 0) {
            setStatus("So du ban dau phai >= 0.", "error");
            return;
        }

        const wallets = readWallets();
        if (id) {
            const index = wallets.findIndex((wallet) => wallet.id === id);
            if (index === -1) {
                setStatus("Khong tim thay vi can cap nhat.", "error");
                return;
            }

            wallets[index] = {
                ...wallets[index],
                name,
                walletType,
                balance,
                isActive
            };

            writeWallets(wallets);
            render();
            resetForm();
            setStatus("Da cap nhat vi.", "success");
            await syncUpdate(wallets[index]);
            return;
        }

        const created = {
            id: makeId(),
            name,
            walletType,
            balance,
            isActive
        };

        wallets.push(created);
        writeWallets(wallets);
        render();
        resetForm();
        setStatus("Da tao vi moi.", "success");
        await syncCreate(created);
    });

    tableBody.addEventListener("click", async (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        const editId = target.getAttribute("data-wallet-edit");
        if (editId) {
            const wallet = readWallets().find((item) => item.id === editId);
            if (!wallet) {
                setStatus("Khong tim thay vi de sua.", "error");
                return;
            }

            fillForm(wallet);
            setStatus("Dang sua vi. Nhan 'Luu vi' de cap nhat.", "info");
            return;
        }

        const deleteId = target.getAttribute("data-wallet-delete");
        if (deleteId) {
            const wallets = readWallets();
            const wallet = wallets.find((item) => item.id === deleteId);
            if (!wallet) {
                setStatus("Khong tim thay vi de xoa.", "error");
                return;
            }

            const confirmed = window.confirm(`Ban chac chan muon xoa vi "${wallet.name}"?`);
            if (!confirmed) {
                return;
            }

            const nextWallets = wallets.filter((item) => item.id !== deleteId);
            writeWallets(nextWallets);
            render();
            resetForm();
            setStatus("Da xoa vi.", "success");
            await syncDelete(deleteId);
        }
    });

    resetButton?.addEventListener("click", () => {
        resetForm();
        setStatus("Form da reset.", "pending");
    });

    window.addEventListener("storage", render);
    render();
    setStatus("San sang tao/sua/xoa vi nhanh.", "pending");
})();
