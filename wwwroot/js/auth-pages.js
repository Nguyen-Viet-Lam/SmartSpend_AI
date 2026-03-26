(function () {
  const tokenStorageKey = "auth.accessToken";
  const userStorageKey = "auth.currentUser";
  const pendingRegisterKey = "smartspend.pendingRegisterEmail";
  const pendingResetKey = "smartspend.pendingResetEmail";

  function setSession(response, rememberMe) {
    const storage = rememberMe ? window.localStorage : window.sessionStorage;
    window.localStorage.removeItem(tokenStorageKey);
    window.localStorage.removeItem(userStorageKey);
    window.sessionStorage.removeItem(tokenStorageKey);
    window.sessionStorage.removeItem(userStorageKey);
    storage.setItem(tokenStorageKey, response.accessToken);
    storage.setItem(userStorageKey, JSON.stringify(response));
  }

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

  function query(name) {
    return new URLSearchParams(window.location.search).get(name) || "";
  }

  function redirectAfterLogin(response) {
    const returnUrl = query("returnUrl");
    if (returnUrl) {
      window.location.href = returnUrl;
      return;
    }

    const role = String(response.role || "").toLowerCase();
    const isAdmin = role === "systemadmin" || role === "admin";
    window.location.href = isAdmin ? "/home/admin-dashboard.html" : "/home/dashboard.html";
  }

  async function submitJson(url, payload) {
    return window.SmartSpendApp.api(url, {
      method: "POST",
      body: payload,
    });
  }

  function initLoginPage() {
    const form = document.getElementById("loginForm");
    if (!form) {
      return;
    }

    const incomingMessage = query("message");
    if (incomingMessage) {
      message("loginMessage", decodeURIComponent(incomingMessage), "warning");
    }

    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("loginMessage", "", "");
      try {
        const response = await submitJson("/api/auth/login", {
          emailOrUsername: form.emailOrUsername.value,
          password: form.password.value,
          rememberMe: form.rememberMe.checked,
        });

        setSession(response, form.rememberMe.checked);
        message("loginMessage", "Đăng nhập thành công, đang chuyển hướng...", "success");
        window.setTimeout(() => redirectAfterLogin(response), 500);
      } catch (error) {
        message("loginMessage", error.message, "error");
      }
    });
  }

  function initRegisterPage() {
    const form = document.getElementById("registerForm");
    if (!form) {
      return;
    }

    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("registerMessage", "", "");
      try {
        const response = await submitJson("/api/auth/register", {
          username: form.username.value,
          fullName: form.fullName.value,
          email: form.email.value,
          password: form.password.value,
          confirmPassword: form.confirmPassword.value,
          acceptTerms: form.acceptTerms.checked,
        });

        window.sessionStorage.setItem(pendingRegisterKey, response.email);
        message("registerMessage", response.message || "Đăng ký thành công.", "success");
        window.setTimeout(() => {
          window.location.href = `/home/otp.html?email=${encodeURIComponent(response.email)}`;
        }, 700);
      } catch (error) {
        message("registerMessage", error.message, "error");
      }
    });
  }

  function initOtpPage() {
    const form = document.getElementById("otpForm");
    if (!form) {
      return;
    }

    const email = query("email") || window.sessionStorage.getItem(pendingRegisterKey) || "";
    const emailInput = document.getElementById("otpEmail");
    if (emailInput) {
      emailInput.value = email;
    }

    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("otpMessage", "", "");
      try {
        await submitJson("/api/auth/verify-email-otp", {
          email: form.email.value,
          otpCode: form.otpCode.value,
        });
        window.sessionStorage.removeItem(pendingRegisterKey);
        message("otpMessage", "Xác thực OTP thành công. Bạn có thể đăng nhập ngay bây giờ.", "success");
        window.setTimeout(() => {
          window.location.href = "/home/login.html?message=Email đã xác thực thành công.";
        }, 700);
      } catch (error) {
        message("otpMessage", error.message, "error");
      }
    });

    document.getElementById("resendOtpButton")?.addEventListener("click", async () => {
      message("otpMessage", "", "");
      try {
        const response = await submitJson("/api/auth/resend-email-otp", { email: form.email.value });
        message("otpMessage", response.message || "Đã gửi lại OTP.", "success");
      } catch (error) {
        message("otpMessage", error.message, "error");
      }
    });
  }

  function initForgotPasswordPage() {
    const form = document.getElementById("forgotPasswordForm");
    if (!form) {
      return;
    }

    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("forgotMessage", "", "");
      try {
        const response = await submitJson("/api/auth/request-password-reset", {
          email: form.email.value,
        });
        window.sessionStorage.setItem(pendingResetKey, form.email.value);
        message("forgotMessage", response.message || "Đã gửi mã OTP đặt lại mật khẩu.", "success");
        window.setTimeout(() => {
          window.location.href = `/home/reset-password.html?email=${encodeURIComponent(form.email.value)}`;
        }, 700);
      } catch (error) {
        message("forgotMessage", error.message, "error");
      }
    });
  }

  function initResetPasswordPage() {
    const form = document.getElementById("resetPasswordForm");
    if (!form) {
      return;
    }

    const email = query("email") || window.sessionStorage.getItem(pendingResetKey) || "";
    const emailInput = document.getElementById("resetEmail");
    if (emailInput) {
      emailInput.value = email;
    }

    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      message("resetMessage", "", "");
      try {
        const response = await submitJson("/api/auth/reset-password", {
          email: form.email.value,
          otpCode: form.otpCode.value,
          newPassword: form.newPassword.value,
          confirmNewPassword: form.confirmNewPassword.value,
        });
        window.sessionStorage.removeItem(pendingResetKey);
        message("resetMessage", response.message || "Đổi mật khẩu thành công.", "success");
        window.setTimeout(() => {
          window.location.href = "/home/login.html?message=Bạn đã đặt lại mật khẩu thành công.";
        }, 700);
      } catch (error) {
        message("resetMessage", error.message, "error");
      }
    });
  }

  document.addEventListener("smartspend:ready", () => {
    initLoginPage();
    initRegisterPage();
    initOtpPage();
    initForgotPasswordPage();
    initResetPasswordPage();
  });
})();
