(function () {
  const pendingVerificationKey = "pendingEmailVerification";
  const otpLength = 6;

  const row = document.getElementById("otpRow");
  const confirmBtn = document.getElementById("confirmBtn");
  const resendLink = document.getElementById("resendLink");
  const resendText = document.getElementById("resendText");
  const verifyEmailElement = document.getElementById("verifyEmail");
  const statusElement = document.getElementById("otpStatus");
  const card = document.querySelector(".card");

  if (
    !row ||
    !confirmBtn ||
    !resendLink ||
    !resendText ||
    !verifyEmailElement ||
    !statusElement
  ) {
    return;
  }

  const state = {
    email: "",
    digits: Array(otpLength).fill(""),
    activeIndex: 0,
    boxes: [],
    countdownTimer: null,
    isSubmitting: false,
    isResending: false,
    hasVerified: false,
  };

  const normalizeEmail = (value) => String(value || "").trim().toLowerCase();

  const getPendingVerification = () => {
    try {
      const raw = sessionStorage.getItem(pendingVerificationKey);
      if (!raw) {
        return null;
      }

      const parsed = JSON.parse(raw);
      if (parsed && typeof parsed === "object") {
        return parsed;
      }
    } catch (error) {
      console.warn("cannot_read_pending_verification", error);
    }

    return null;
  };

  const savePendingVerification = (payload) => {
    try {
      sessionStorage.setItem(pendingVerificationKey, JSON.stringify(payload));
    } catch (error) {
      console.warn("cannot_save_pending_verification", error);
    }
  };

  const clearPendingVerification = (email) => {
    const targetEmail = normalizeEmail(email);
    const pending = getPendingVerification();
    const pendingEmail = normalizeEmail(pending?.email);

    if (!pending || !targetEmail || pendingEmail !== targetEmail) {
      return;
    }

    try {
      sessionStorage.removeItem(pendingVerificationKey);
    } catch (error) {
      console.warn("cannot_clear_pending_verification", error);
    }
  };

  const extractMessageFromResponse = (data, fallbackMessage) => {
    if (data && typeof data === "object") {
      if (typeof data.message === "string" && data.message.trim()) {
        return data.message.trim();
      }

      if (data.errors && typeof data.errors === "object") {
        for (const messages of Object.values(data.errors)) {
          if (Array.isArray(messages) && messages.length > 0) {
            return String(messages[0]);
          }

          if (typeof messages === "string" && messages.trim()) {
            return messages.trim();
          }
        }
      }
    }

    return fallbackMessage;
  };

  const setStatus = (message, type) => {
    statusElement.textContent = message || "";
    statusElement.className = `status-message ${type || "info"}`;
  };

  const maskEmail = (email) => {
    const value = normalizeEmail(email);
    const [localPart, domainPart] = value.split("@");

    if (!localPart || !domainPart) {
      return value;
    }

    if (localPart.length <= 2) {
      return `${localPart[0] || "*"}***@${domainPart}`;
    }

    const first = localPart.slice(0, 2);
    return `${first}${"*".repeat(Math.max(localPart.length - 2, 1))}@${domainPart}`;
  };

  const setVerifyEmail = (email) => {
    verifyEmailElement.textContent = "";

    if (!email) {
      return;
    }

    verifyEmailElement.append("Email đang xác thực: ");
    const strong = document.createElement("strong");
    strong.textContent = maskEmail(email);
    verifyEmailElement.append(strong);
  };

  const disableResend = (label) => {
    resendText.textContent = "";
    resendLink.textContent = label;
    resendLink.style.pointerEvents = "none";
    resendLink.style.color = "var(--text-muted)";
  };

  const stopCountdown = () => {
    if (state.countdownTimer) {
      window.clearInterval(state.countdownTimer);
      state.countdownTimer = null;
    }
  };

  const enableResend = () => {
    resendText.textContent = "Chưa nhận được mã? ";
    resendLink.textContent = "Gửi lại ngay";
    resendLink.style.pointerEvents = "";
    resendLink.style.color = "";
  };

  const startCountdown = () => {
    if (state.hasVerified) {
      return;
    }

    stopCountdown();

    let seconds = 60;
    disableResend(`Gửi lại sau ${seconds}s`);

    state.countdownTimer = window.setInterval(() => {
      seconds -= 1;
      if (seconds <= 0) {
        stopCountdown();

        if (!state.isResending && !state.hasVerified && state.email) {
          enableResend();
        }

        return;
      }

      disableResend(`Gửi lại sau ${seconds}s`);
    }, 1000);
  };

  const setSubmitting = (isSubmitting) => {
    state.isSubmitting = isSubmitting;
    confirmBtn.textContent = isSubmitting ? "Đang xác thực..." : "Xác nhận";
    render();
  };

  const clearOtp = () => {
    state.digits = Array(otpLength).fill("");
    state.activeIndex = 0;
    render();
  };

  const getOtpCode = () => state.digits.join("");

  const render = () => {
    state.boxes.forEach((box, index) => {
      const value = state.digits[index];
      box.textContent = value;
      box.classList.toggle("has-value", value !== "");
      box.style.removeProperty("border-color");
      box.style.removeProperty("background");
      box.style.removeProperty("box-shadow");
      box.style.removeProperty("transform");

      if (index === state.activeIndex) {
        box.style.borderColor = "var(--accent)";
        box.style.background = "#1e2f44";
        box.style.boxShadow = "0 0 0 3px var(--accent-glow)";
        box.style.transform = "translateY(-2px) scale(1.04)";
      } else if (value) {
        box.style.borderColor = "rgba(59,127,245,0.5)";
      }
    });

    const isOtpCompleted = state.digits.every((digit) => digit !== "");
    confirmBtn.disabled =
      !state.email || !isOtpCompleted || state.isSubmitting || state.hasVerified;
  };

  const focusBox = (index) => {
    state.activeIndex = Math.max(0, Math.min(otpLength - 1, index));
    render();
    row.focus({ preventScroll: true });
  };

  const createOtpBoxes = () => {
    for (let i = 0; i < otpLength; i += 1) {
      const box = document.createElement("div");
      box.className = "otp-input";
      box.setAttribute("tabindex", "0");
      box.setAttribute("role", "textbox");
      box.setAttribute("aria-label", `Số ${i + 1}`);

      box.addEventListener("mousedown", (event) => {
        event.preventDefault();
        focusBox(i);
      });

      row.append(box);
      state.boxes.push(box);
    }
  };

  const readJsonSafely = async (response) => {
    const contentType = response.headers.get("content-type") || "";
    if (!contentType.includes("application/json")) {
      return null;
    }

    try {
      return await response.json();
    } catch {
      return null;
    }
  };

  const verifyOtp = async () => {
    if (!state.email || state.hasVerified) {
      return;
    }

    const otpCode = getOtpCode();
    if (!/^\d{6}$/.test(otpCode)) {
      setStatus("OTP phải gồm đúng 6 chữ số.", "error");
      return;
    }

    setSubmitting(true);
    setStatus("Đang xác thực OTP...", "info");

    try {
      const response = await fetch("/api/auth/verify-email-otp", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: state.email,
          otpCode,
        }),
      });

      const data = await readJsonSafely(response);
      if (!response.ok) {
        setStatus(
          extractMessageFromResponse(data, "Xác thực OTP thất bại. Vui lòng thử lại."),
          "error"
        );
        return;
      }

      state.hasVerified = true;
      state.isSubmitting = false;
      clearPendingVerification(state.email);
      setStatus(
        extractMessageFromResponse(
          data,
          "Xác thực email thành công. Đang chuyển sang trang đăng nhập..."
        ),
        "success"
      );
      stopCountdown();
      disableResend("Đã xác thực");
      confirmBtn.textContent = "Đã xác thực";
      render();

      window.setTimeout(() => {
        window.location.href = `login.html?verified=1&email=${encodeURIComponent(state.email)}`;
      }, 1200);
    } catch (error) {
      console.error("verify_otp_failed", error);
      setStatus("Không thể kết nối tới máy chủ.", "error");
    } finally {
      if (!state.hasVerified) {
        setSubmitting(false);
      }
    }
  };

  const resendOtp = async () => {
    if (
      !state.email ||
      state.hasVerified ||
      state.isResending ||
      state.countdownTimer
    ) {
      return;
    }

    state.isResending = true;
    disableResend("Đang gửi...");

    try {
      const response = await fetch("/api/auth/resend-email-otp", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: state.email,
        }),
      });

      const data = await readJsonSafely(response);
      if (!response.ok) {
        setStatus(
          extractMessageFromResponse(data, "Không thể gửi lại OTP."),
          "error"
        );
        enableResend();
        return;
      }

      const pending = getPendingVerification() || {};
      savePendingVerification({
        ...pending,
        email: state.email,
        otpDispatched: true,
        otpExpiresAt: data?.expiresAt ?? pending.otpExpiresAt ?? null,
        savedAt: new Date().toISOString(),
      });

      clearOtp();
      focusBox(0);
      setStatus(
        extractMessageFromResponse(data, "Đã gửi lại OTP. Vui lòng kiểm tra email."),
        "info"
      );
      startCountdown();
    } catch (error) {
      console.error("resend_otp_failed", error);
      setStatus("Không thể kết nối tới máy chủ.", "error");
      enableResend();
    } finally {
      state.isResending = false;
      if (!state.countdownTimer && !state.hasVerified && state.email) {
        enableResend();
      }
    }
  };

  createOtpBoxes();
  row.setAttribute("tabindex", "0");
  row.style.outline = "none";

  row.addEventListener("keydown", (event) => {
    if (state.isSubmitting || state.hasVerified) {
      event.preventDefault();
      return;
    }

    if (event.key === "ArrowLeft") {
      event.preventDefault();
      focusBox(state.activeIndex - 1);
      return;
    }

    if (event.key === "ArrowRight") {
      event.preventDefault();
      focusBox(state.activeIndex + 1);
      return;
    }

    if (event.key === "Tab") {
      return;
    }

    if (event.key === "Backspace") {
      event.preventDefault();
      if (state.digits[state.activeIndex]) {
        state.digits[state.activeIndex] = "";
      } else if (state.activeIndex > 0) {
        state.activeIndex -= 1;
        state.digits[state.activeIndex] = "";
      }
      render();
      return;
    }

    if (event.key === "Delete") {
      event.preventDefault();
      state.digits[state.activeIndex] = "";
      render();
      return;
    }

    if (/^\d$/.test(event.key)) {
      event.preventDefault();
      state.digits[state.activeIndex] = event.key;
      if (state.activeIndex < otpLength - 1) {
        state.activeIndex += 1;
      }
      render();
      return;
    }

    event.preventDefault();
  });

  row.addEventListener("paste", (event) => {
    if (state.isSubmitting || state.hasVerified) {
      event.preventDefault();
      return;
    }

    event.preventDefault();
    const text = (event.clipboardData || window.clipboardData)
      .getData("text")
      .replace(/\D/g, "");

    if (!text) {
      return;
    }

    for (let i = 0; i < otpLength; i += 1) {
      state.digits[i] = text[i] || "";
    }

    state.activeIndex = Math.min(text.length, otpLength - 1);
    render();
  });

  confirmBtn.addEventListener("click", async () => {
    await verifyOtp();
  });

  resendLink.addEventListener("click", async (event) => {
    event.preventDefault();
    await resendOtp();
  });

  if (card) {
    card.addEventListener("click", () => {
      row.focus();
    });
  }

  const pending = getPendingVerification();
  const queryEmail = normalizeEmail(new URLSearchParams(window.location.search).get("email"));
  const pendingEmail = normalizeEmail(pending?.email);

  state.email = queryEmail || pendingEmail;

  if (state.email) {
    setVerifyEmail(state.email);

    if (pending && pendingEmail === state.email) {
      savePendingVerification({
        ...pending,
        email: state.email,
      });
    }

    if (pending && pendingEmail === state.email && pending.otpDispatched === false) {
      setStatus(
        pending.message ||
          "Đăng ký thành công nhưng OTP chưa gửi được. Vui lòng bấm gửi lại mã.",
        "error"
      );
      enableResend();
    } else {
      setStatus("Mã OTP đã được gửi. Vui lòng nhập mã để hoàn tất đăng ký.", "info");
      startCountdown();
    }
  } else {
    setStatus(
      "Không tìm thấy email cần xác thực. Vui lòng đăng ký tài khoản lại từ đầu.",
      "error"
    );
    stopCountdown();
    disableResend("Không khả dụng");
  }

  render();
  focusBox(0);
})();
