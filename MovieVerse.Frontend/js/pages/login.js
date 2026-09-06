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
    location.replace(safeReturn(new URLSearchParams(location.search).get("returnUrl")) || "index.html");
    return;
  }

  const formEl = document.getElementById("loginForm");
  const btn = document.getElementById("loginSubmit");
  const errorBox = document.getElementById("loginError");
  if (!formEl) return;

  // Use jQuery Validation when the CDN library is available, but do not make
  // the actual login submission depend on it. This prevents a failed CDN load
  // from falling back to the browser's normal form navigation.
  if (window.jQuery && jQuery.fn && typeof jQuery.fn.validate === "function") {
    jQuery(formEl).validate({
      rules: {
        email: { required: true, email: true },
        password: { required: true }
      },
      messages: {
        email: {
          required: "Email is required.",
          email: "Please enter a valid email address."
        },
        password: { required: "Password is required." }
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
    MV.ui.buttonBusy(btn, true, "Signing in…");

    try {
      const data = await MV.api.post(
        "auth/login",
        {
          email: formEl.elements.email.value.trim(),
          password: formEl.elements.password.value
        },
        { auth: false, handle401: false }
      );

      if (!data?.token) {
        throw new MV.ApiError({
          status: 0,
          title: "Login failed",
          detail: "The server did not return an authentication token."
        });
      }

      MV.auth.setToken(data.token);
      MV.ui.setFlash("Welcome back", "success");
      location.href = safeReturn(new URLSearchParams(location.search).get("returnUrl")) || "index.html";
    } catch (err) {
      errorBox.innerHTML = `<div class="alert alert-danger py-2">${MV.ui.escapeHtml(err.detail || "Login failed.")}</div>`;
    } finally {
      submitting = false;
      MV.ui.buttonBusy(btn, false);
    }
  });
});
