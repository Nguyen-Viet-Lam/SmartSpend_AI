(() => {
    const storage = window.smartSpendStorage;
    const api = window.smartSpendApi;
    const usersKey = "smartspend-admin-users";
    const categoriesKey = "smartspend-categories";

    const usersTable = document.querySelector("[data-admin-users]");
    const categoriesTable = document.querySelector("[data-admin-categories]");
    const categoryForm = document.querySelector("[data-admin-category-form]");
    const resetButton = document.querySelector("[data-admin-category-reset]");
    const statusEl = document.querySelector("[data-admin-status]");

    if (!storage || !usersTable || !categoriesTable || !categoryForm || !statusEl) {
        return;
    }

    const makeId = () => window.crypto?.randomUUID?.() || `id-${Date.now()}-${Math.floor(Math.random() * 1_000_000)}`;

    const setStatus = (text, type) => {
        statusEl.textContent = text;
        statusEl.className = `status-text ${type}`;
    };

    const normalizeStatus = (status) => {
        if (status === "Locked") {
            return "Locked";
        }

        if (status === "PendingVerification") {
            return "PendingVerification";
        }

        return "Active";
    };

    const statusTextMap = {
        Active: "Dang hoat dong",
        Locked: "Da khoa",
        PendingVerification: "Cho OTP"
    };

    const readUsers = () => {
        const users = storage.read(usersKey, []);
        if (Array.isArray(users) && users.length) {
            return users;
        }

        const seed = [
            { id: makeId(), email: "admin@smartspend.local", displayName: "System Admin", status: "Active" },
            { id: makeId(), email: "user1@gmail.com", displayName: "Nguyen Van A", status: "PendingVerification" },
            { id: makeId(), email: "user2@gmail.com", displayName: "Tran Thi B", status: "Locked" }
        ];
        storage.write(usersKey, seed);
        return seed;
    };

    const readCategories = () => {
        const categories = storage.read(categoriesKey, []);
        if (Array.isArray(categories) && categories.length) {
            return categories;
        }

        const seed = [
            { id: makeId(), name: "An uong", icon: "fa-utensils", isActive: true },
            { id: makeId(), name: "Di chuyen", icon: "fa-car", isActive: true },
            { id: makeId(), name: "Giai tri", icon: "fa-film", isActive: true },
            { id: makeId(), name: "Hoa don", icon: "fa-file-invoice", isActive: true }
        ];
        storage.write(categoriesKey, seed);
        return seed;
    };

    const writeUsers = (users) => storage.write(usersKey, users);
    const writeCategories = (categories) => storage.write(categoriesKey, categories);

    const toStatusClass = (status) => {
        if (status === "Locked") {
            return "locked";
        }

        if (status === "PendingVerification") {
            return "pending";
        }

        return "active";
    };

    const renderUsers = () => {
        const users = readUsers();
        if (!users.length) {
            usersTable.innerHTML = "<tr><td colspan='4' class='empty-row'>Chua co user nao.</td></tr>";
            return;
        }

        usersTable.innerHTML = users
            .map((user) => {
                const status = normalizeStatus(user.status);
                const action = status === "Locked" ? "Mo khoa" : "Khoa";
                return `
                    <tr>
                        <td>${user.email}</td>
                        <td>${user.displayName}</td>
                        <td><span class="admin-badge ${toStatusClass(status)}">${statusTextMap[status] || status}</span></td>
                        <td>
                            <button type="button" class="action-btn ${status === "Locked" ? "" : "warn"}" data-admin-toggle-user="${user.id}">
                                ${action}
                            </button>
                        </td>
                    </tr>
                `;
            })
            .join("");
    };

    const renderCategories = () => {
        const categories = readCategories();
        if (!categories.length) {
            categoriesTable.innerHTML = "<tr><td colspan='4' class='empty-row'>Chua co category nao.</td></tr>";
            return;
        }

        categoriesTable.innerHTML = categories
            .map((category) => `
                <tr>
                    <td>${category.name}</td>
                    <td>${category.icon || "-"}</td>
                    <td><span class="admin-badge ${category.isActive ? "active" : "locked"}">${category.isActive ? "Dang dung" : "Da tat"}</span></td>
                    <td>
                        <div class="inline-actions">
                            <button type="button" class="action-btn" data-admin-edit-category="${category.id}">Sua</button>
                            <button type="button" class="action-btn ${category.isActive ? "warn" : ""}" data-admin-toggle-category="${category.id}">
                                ${category.isActive ? "Tat" : "Bat"}
                            </button>
                            <button type="button" class="action-btn danger" data-admin-delete-category="${category.id}">Xoa</button>
                        </div>
                    </td>
                </tr>
            `)
            .join("");
    };

    const renderAll = () => {
        renderUsers();
        renderCategories();
    };

    const resetCategoryForm = () => {
        categoryForm.reset();
        categoryForm.elements.id.value = "";
        categoryForm.elements.isActive.checked = true;
    };

    const syncUserStatus = async (userId, status) => {
        if (!api?.admin?.updateUserStatus) {
            return;
        }

        try {
            await api.admin.updateUserStatus(userId, { status });
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da cap nhat local. API admin users chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da cap nhat local, nhung sync user API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    const syncCategoryUpsert = async (category, isCreate) => {
        if (!api?.admin) {
            return;
        }

        if (isCreate) {
            return;
        }

        try {
            if (api.admin.updateCategory) {
                await api.admin.updateCategory(category.id, {
                    name: category.name,
                    icon: category.icon,
                    isActive: category.isActive
                });
            }
        } catch (error) {
            const message = (error?.message || "").toLowerCase();
            if (message.includes("501")) {
                setStatus("Da cap nhat local. API admin categories chua hoan thien (501).", "warning");
                return;
            }

            setStatus(`Da cap nhat local, nhung sync category API loi: ${error.message || "khong xac dinh"}.`, "warning");
        }
    };

    const syncCategoryDelete = async () => {
        await Promise.resolve();
    };

    usersTable.addEventListener("click", async (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        const userId = target.getAttribute("data-admin-toggle-user");
        if (!userId) {
            return;
        }

        const users = readUsers();
        const user = users.find((item) => item.id === userId);
        if (!user) {
            setStatus("Khong tim thay user de cap nhat.", "error");
            return;
        }

        const nextStatus = normalizeStatus(user.status) === "Locked" ? "Active" : "Locked";
        user.status = nextStatus;
        writeUsers(users);
        renderUsers();
        setStatus(`Da cap nhat trang thai user: ${user.email}.`, "success");
        await syncUserStatus(userId, nextStatus);
    });

    categoriesTable.addEventListener("click", async (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        const editId = target.getAttribute("data-admin-edit-category");
        if (editId) {
            const category = readCategories().find((item) => item.id === editId);
            if (!category) {
                setStatus("Khong tim thay category de sua.", "error");
                return;
            }

            categoryForm.elements.id.value = category.id;
            categoryForm.elements.name.value = category.name;
            categoryForm.elements.icon.value = category.icon || "";
            categoryForm.elements.isActive.checked = category.isActive !== false;
            setStatus("Dang sua category. Nhan 'Luu category' de cap nhat.", "info");
            return;
        }

        const toggleId = target.getAttribute("data-admin-toggle-category");
        if (toggleId) {
            const categories = readCategories();
            const category = categories.find((item) => item.id === toggleId);
            if (!category) {
                setStatus("Khong tim thay category de bat/tat.", "error");
                return;
            }

            category.isActive = !category.isActive;
            writeCategories(categories);
            renderCategories();
            setStatus(`Da cap nhat trang thai category: ${category.name}.`, "success");
            await syncCategoryUpsert(category, false);
            return;
        }

        const deleteId = target.getAttribute("data-admin-delete-category");
        if (deleteId) {
            const categories = readCategories();
            const category = categories.find((item) => item.id === deleteId);
            if (!category) {
                setStatus("Khong tim thay category de xoa.", "error");
                return;
            }

            const confirmed = window.confirm(`Ban chac chan muon xoa category "${category.name}"?`);
            if (!confirmed) {
                return;
            }

            const nextCategories = categories.filter((item) => item.id !== deleteId);
            writeCategories(nextCategories);
            renderCategories();
            resetCategoryForm();
            setStatus(`Da xoa category: ${category.name}.`, "success");
            await syncCategoryDelete(deleteId);
        }
    });

    categoryForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const id = (categoryForm.elements.id.value || "").trim();
        const name = (categoryForm.elements.name.value || "").trim();
        const icon = (categoryForm.elements.icon.value || "").trim();
        const isActive = Boolean(categoryForm.elements.isActive.checked);

        if (name.length < 2) {
            setStatus("Ten category toi thieu 2 ky tu.", "error");
            return;
        }

        const categories = readCategories();
        if (id) {
            const index = categories.findIndex((item) => item.id === id);
            if (index === -1) {
                setStatus("Khong tim thay category de cap nhat.", "error");
                return;
            }

            categories[index] = { ...categories[index], name, icon, isActive };
            writeCategories(categories);
            renderCategories();
            resetCategoryForm();
            setStatus("Da cap nhat category.", "success");
            await syncCategoryUpsert(categories[index], false);
            return;
        }

        const created = { id: makeId(), name, icon, isActive };
        categories.push(created);
        writeCategories(categories);
        renderCategories();
        resetCategoryForm();
        setStatus("Da tao category moi.", "success");
        await syncCategoryUpsert(created, true);
    });

    resetButton?.addEventListener("click", () => {
        resetCategoryForm();
        setStatus("Form category da reset.", "pending");
    });

    window.addEventListener("storage", renderAll);
    renderAll();
    setStatus("San sang quan tri User va Category.", "pending");
})();
