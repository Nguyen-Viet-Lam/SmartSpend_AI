(() => {
    const forms = document.querySelectorAll("form[data-auth-action]");
    const api = window.smartSpendApi?.auth;
    if (!forms.length || !api) {
        return;
    }

    const resendCooldownSeconds = 45;
    const resendCooldownPrefix = "smartspend-resendotp-until:";

    const actionMap = {
        register: api.register,
        login: api.login,
        verifyOtp: api.verifyOtp,
        resendOtp: api.resendOtp
    };

    const setStatus = (statusEl, text, type) => {
        statusEl.textContent = text;
        statusEl.className = `status-text ${type}`;
    };

    const getCooldownStorageKey = (email) => `${resendCooldownPrefix}${(email || "").trim().toLowerCase()}`;

    const readCooldownUntil = (email) => {
        if (!email) {
            return 0;
        }

        const raw = localStorage.getItem(getCooldownStorageKey(email));
        const parsed = Number(raw || 0);
        return Number.isFinite(parsed) ? parsed : 0;
    };

    const writeCooldownUntil = (email, unixSeconds) => {
        if (!email) {
            return;
        }

        localStorage.setItem(getCooldownStorageKey(email), String(Math.max(0, Math.floor(unixSeconds))));
    };

    const setLoading = (form, isLoading) => {
        const controls = form.querySelectorAll("input, button, select, textarea");
        controls.forEach((control) => {
            if (isLoading) {
                control.setAttribute("data-original-disabled", control.disabled ? "1" : "0");
                control.disabled = true;
            } else {
                const wasDisabled = control.getAttribute("data-original-disabled") === "1";
                control.disabled = wasDisabled;
                control.removeAttribute("data-original-disabled");
            }
        });

        const submit = form.querySelector("button[type='submit']");
        if (submit) {
            if (!submit.dataset.originalText) {
                submit.dataset.originalText = submit.textContent || "Submit";
            }

            submit.classList.toggle("is-loading", isLoading);
            submit.textContent = isLoading ? "Dang xu ly..." : submit.dataset.originalText;
        }
    };

    const formToObject = (form) => {
        const formData = new FormData(form);
        return Object.fromEntries(formData.entries());
    };

    const normalizeEmail = (value) => (value || "").trim().toLowerCase();
    const isValidEmail = (value) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value || "");
    const readInputValue = (form, name) => {
        const input = form.querySelector(`[name='${name}']`);
        return input?.value ?? "";
    };

    const buildPayload = (form, action, rawPayload) => {
        const email = normalizeEmail(rawPayload.email || readInputValue(form, "email"));
        switch (action) {
            case "register":
                return {
                    displayName: (rawPayload.displayName || "").trim(),
                    email,
                    password: rawPayload.password || ""
                };
            case "login":
                return {
                    email,
                    password: rawPayload.password || ""
                };
            case "verifyOtp":
                return {
                    email,
                    otpCode: (rawPayload.otpCode || "").trim()
                };
            case "resendOtp":
                return {
                    email
                };
            default:
                return rawPayload;
        }
    };

    const validatePayload = (action, payload, rawPayload) => {
        const email = (payload.email || "").trim();
        if (!isValidEmail(email)) {
            throw new Error("Email khong hop le.");
        }

        if (action === "register") {
            const displayName = (payload.displayName || "").trim();
            const password = payload.password || "";
            const confirmPassword = rawPayload.confirmPassword || "";

            if (displayName.length < 2) {
                throw new Error("Ten hien thi toi thieu 2 ky tu.");
            }

            if (password.length < 8) {
                throw new Error("Mat khau toi thieu 8 ky tu.");
            }

            if (confirmPassword !== password) {
                throw new Error("Xac nhan mat khau khong khop.");
            }
        }

        if (action === "login") {
            if (!(payload.password || "").trim()) {
                throw new Error("Ban chua nhap mat khau.");
            }
        }

        if (action === "verifyOtp") {
            if (!/^\d{6}$/.test(payload.otpCode || "")) {
                throw new Error("OTP gom dung 6 chu so.");
            }
        }
    };

    const navigateAfterSuccess = (action, payload, result) => {
        if (action === "register") {
            const email = encodeURIComponent(payload.email || "");
            window.setTimeout(() => {
                window.location.href = `/Auth/Otp?email=${email}`;
            }, 700);
            return;
        }

        if (action === "login") {
            const needOtp = result?.data?.requiresOtpVerification === true;
            if (needOtp) {
                const email = encodeURIComponent(payload.email || "");
                window.setTimeout(() => {
                    window.location.href = `/Auth/Otp?email=${email}`;
                }, 700);
                return;
            }

            window.setTimeout(() => {
                window.location.href = "/Dashboard";
            }, 700);
            return;
        }

        if (action === "verifyOtp") {
            window.setTimeout(() => {
                window.location.href = "/Auth/Login?verified=1";
            }, 700);
        }
    };

    const getResendButton = (form) => form.querySelector("button[type='submit']");

    const updateResendCooldownUi = (form) => {
        const action = form.getAttribute("data-auth-action");
        if (action !== "resendOtp") {
            return;
        }

        const emailInput = form.querySelector("input[name='email']");
        const button = getResendButton(form);
        if (!emailInput || !button) {
            return;
        }

        const email = normalizeEmail(emailInput.value);
        const until = readCooldownUntil(email);
        const now = Math.floor(Date.now() / 1000);
        const remaining = Math.max(0, until - now);

        if (!button.dataset.defaultText) {
            button.dataset.defaultText = button.textContent || "Gui lai OTP";
        }

        if (remaining > 0) {
            button.disabled = true;
            button.textContent = `Gui lai sau ${remaining}s`;
            return;
        }

        button.disabled = false;
        button.textContent = button.dataset.defaultText;
    };

    forms.forEach((form) => {
        const action = form.getAttribute("data-auth-action");
        const statusEl = form.querySelector("[data-status]");
        if (!action || !statusEl || !actionMap[action]) {
            return;
        }

        if (action === "resendOtp") {
            const emailInput = form.querySelector("input[name='email']");
            emailInput?.addEventListener("input", () => updateResendCooldownUi(form));
            window.setInterval(() => updateResendCooldownUi(form), 1000);
            updateResendCooldownUi(form);
        }

        form.addEventListener("submit", async (event) => {
            event.preventDefault();

            setStatus(statusEl, "Dang gui yeu cau...", "pending");

            try {
                const rawPayload = formToObject(form);
                const payload = buildPayload(form, action, rawPayload);
                validatePayload(action, payload, rawPayload);
                setLoading(form, true);

                const result = await actionMap[action](payload);
                const message = result?.message || form.getAttribute("data-success-message") || "Thanh cong.";
                setStatus(statusEl, message, "success");

                if (action === "resendOtp") {
                    const until = Math.floor(Date.now() / 1000) + resendCooldownSeconds;
                    writeCooldownUntil(payload.email, until);
                    updateResendCooldownUi(form);
                }

                navigateAfterSuccess(action, payload, result);
            } catch (error) {
                setStatus(statusEl, error.message || "Co loi xay ra.", "error");
            } finally {
                setLoading(form, false);
                if (action === "resendOtp") {
                    updateResendCooldownUi(form);
                }
            }
        });
    });

    if (window.location.pathname.toLowerCase().includes("/auth/otp")) {
        const params = new URLSearchParams(window.location.search);
        const email = params.get("email");
        if (email) {
            document.querySelectorAll("input[name='email']").forEach((input) => {
                input.value = email;
            });
            document.querySelectorAll("form[data-auth-action='resendOtp']").forEach((form) => updateResendCooldownUi(form));
        }
    }
})();
