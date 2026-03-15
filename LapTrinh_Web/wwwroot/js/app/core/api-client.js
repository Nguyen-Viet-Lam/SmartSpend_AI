window.smartSpendApi = (() => {
    const parseBody = async (response) => {
        const contentType = response.headers.get("Content-Type") || "";
        if (contentType.includes("application/json")) {
            try {
                return await response.json();
            } catch {
                return null;
            }
        }

        try {
            const text = await response.text();
            return text ? { message: text } : null;
        } catch {
            return null;
        }
    };

    const request = async (url, options = {}) => {
        const hasBody = Boolean(options.body);
        const response = await fetch(url, {
            credentials: "same-origin",
            headers: {
                ...(hasBody ? { "Content-Type": "application/json" } : {}),
                ...(options.headers || {})
            },
            ...options
        });

        const body = await parseBody(response);
        if (!response.ok) {
            const message = body?.message || `Request failed (${response.status})`;
            throw new Error(message);
        }

        return body;
    };

    return {
        auth: {
            register: (payload) => request("/api/auth/register", { method: "POST", body: JSON.stringify(payload) }),
            login: (payload) => request("/api/auth/login", { method: "POST", body: JSON.stringify(payload) }),
            verifyOtp: (payload) => request("/api/auth/verify-otp", { method: "POST", body: JSON.stringify(payload) }),
            resendOtp: (payload) => request("/api/auth/resend-otp", { method: "POST", body: JSON.stringify(payload) }),
            logout: () => request("/api/auth/logout", { method: "POST" })
        },
        wallets: {
            getAll: () => request("/api/wallets"),
            create: (payload) => request("/api/wallets", { method: "POST", body: JSON.stringify(payload) }),
            update: (id, payload) => request(`/api/wallets/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
            remove: (id) => request(`/api/wallets/${id}`, { method: "DELETE" })
        },
        transactions: {
            getAll: () => request("/api/transactions"),
            create: (payload) => request("/api/transactions", { method: "POST", body: JSON.stringify(payload) }),
            suggestCategory: (description) => request("/api/transactions/suggest-category", { method: "POST", body: JSON.stringify(description) })
        },
        admin: {
            users: () => request("/api/admin/users"),
            updateUserStatus: (userId, payload) => request(`/api/admin/users/${userId}/status`, { method: "PUT", body: JSON.stringify(payload) }),
            categories: () => request("/api/admin/categories"),
            createCategory: (payload) => request("/api/admin/categories", { method: "POST", body: JSON.stringify(payload) }),
            updateCategory: (categoryId, payload) => request(`/api/admin/categories/${categoryId}`, { method: "PUT", body: JSON.stringify(payload) }),
            removeCategory: (categoryId) => request(`/api/admin/categories/${categoryId}`, { method: "DELETE" })
        }
    };
})();
