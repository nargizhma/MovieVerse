document.addEventListener("DOMContentLoaded", () => {
  const safeReturn = value => {
    if (!value) return null;
    try {
      const decoded = decodeURIComponent(value);
      if (decoded.startsWith("//") || /^[a-z][a-z0-9+.-]*:/i.test(decoded)) return null;
      return decoded.replace(/^\//, "");
    } catch {
      return null;
    }
  };

  if (MV.auth.isAuthenticated()) {
    location.replace("index.html");
    return;
  }

  const formEl = document.getElementById("registerForm");
  const btn = document.getElementById("registerSubmit");
  const errorBox = document.getElementById("registerError");
  if (!formEl) return;

  if (window.jQuery && jQuery.validator && jQuery.fn && typeof jQuery.fn.validate === "function") {
    jQuery.validator.addMethod("hasUpper", v => /[A-Z]/.test(v), "Password must contain at least one uppercase letter.");
    jQuery.validator.addMethod("hasLower", v => /[a-z]/.test(v), "Password must contain at least one lowercase letter.");
    jQuery.validator.addMethod("hasDigit", v => /[0-9]/.test(v), "Password must contain at least one digit.");
    jQuery.validator.addMethod("hasSpecial", v => /[^a-zA-Z0-9]/.test(v), "Password must contain at least one special character.");

    jQuery(formEl).validate({
      rules: {
        email: { required: true, email: true },
        userName: { required: true, minlength: 3, maxlength: 50 },
        displayName: { maxlength: 50 },
        password: { required: true, minlength: 6, hasUpper: true, hasLower: true, hasDigit: true, hasSpecial: true },
        confirmPassword: { required: true, equalTo: "#password" }
      },
      messages: {
        email: { required: "Email is required.", email: "Please enter a valid email address." },
        userName: { required: "Username is required.", minlength: "Username must be at least 3 characters.", maxlength: "Username cannot exceed 50 characters." },
        displayName: { maxlength: "Display name cannot exceed 50 characters." },
        password: { required: "Password is required.", minlength: "Password must be at least 6 characters." },
        confirmPassword: { required: "Please confirm your password.", equalTo: "Passwords do not match." }
      }
    });
  }

  let submitting = false;

  formEl.addEventListener("submit", async event => {
    event.preventDefault();
    if (submitting) return;

    if (window.jQuery && jQuery.fn && typeof jQuery.fn.validate === "function") {
      if (!jQuery(formEl).valid()) return;
    } else if (!formEl.checkValidity()) {
      formEl.reportValidity();
      return;
    }

    submitting = true;
    errorBox.innerHTML = "";
    MV.ui.buttonBusy(btn, true, "Creating account…");

    try {
      const body = {
        email: formEl.elements.email.value.trim(),
        userName: formEl.elements.userName.value.trim(),
        password: formEl.elements.password.value,
        displayName: formEl.elements.displayName.value.trim() || null
      };

      const data = await MV.api.post("auth/register", body, { auth: false, handle401: false });
      if (!data?.token) {
        throw new MV.ApiError({ status: 0, title: "Registration failed", detail: "The server did not return an authentication token." });
      }

      MV.auth.setToken(data.token);
      MV.ui.setFlash("Account created", "success");
      location.href = safeReturn(new URLSearchParams(location.search).get("returnUrl")) || "index.html";
    } catch (err) {
      errorBox.innerHTML = `<div class="alert alert-danger py-2">${MV.ui.escapeHtml(err.detail || "Registration failed.")}</div>`;
    } finally {
      submitting = false;
      MV.ui.buttonBusy(btn, false);
    }
  });
});
