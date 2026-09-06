document.addEventListener("DOMContentLoaded", () => {
  const safeReturn = value => {
    if (!value) return null;

    try {
      const decoded =
        decodeURIComponent(value);

      if (
        decoded.startsWith("//") ||
        /^[a-z][a-z0-9+.-]*:/i
          .test(decoded)
      ) {
        return null;
      }

      return decoded.replace(
        /^\//,
        ""
      );
    } catch {
      return null;
    }
  };

  if (MV.auth.isAuthenticated()) {
    location.replace(
      safeReturn(
        new URLSearchParams(
          location.search
        ).get("returnUrl")
      ) ||
      "index.html"
    );

    return;
  }

  const form =
    document.getElementById(
      "loginForm"
    );

  const button =
    document.getElementById(
      "loginSubmit"
    );

  const errorBox =
    document.getElementById(
      "loginError"
    );

  if (!form) return;

  MV.ui.useBackendValidation(form);

  let submitting = false;

  form.addEventListener(
    "submit",
    async event => {
      event.preventDefault();

      if (submitting) return;

      submitting = true;

      MV.ui.clearInlineError(
        errorBox
      );

      MV.ui.buttonBusy(
        button,
        true,
        "Signing in…"
      );

      try {
        const data =
          await MV.api.post(
            "auth/login",
            {
              email:
                form.elements.email
                  .value
                  .trim(),
              password:
                form.elements.password
                  .value
            },
            {
              auth: false,
              handle401: false
            }
          );

        if (!data?.token) {
          throw new MV.ApiError({
            status: 0,
            title:
              "Login failed",
            detail:
              "The server did not return an authentication token."
          });
        }

        MV.auth.setToken(
          data.token
        );

        MV.ui.setFlash(
          "Welcome back",
          "success"
        );

        location.href =
          safeReturn(
            new URLSearchParams(
              location.search
            ).get("returnUrl")
          ) ||
          "index.html";
      } catch (err) {
        MV.ui.showInlineError(
          errorBox,
          err,
          "Login failed."
        );
      } finally {
        submitting = false;

        MV.ui.buttonBusy(
          button,
          false
        );
      }
    }
  );
});
