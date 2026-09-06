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
    location.replace("index.html");
    return;
  }

  const form =
    document.getElementById(
      "registerForm"
    );

  const button =
    document.getElementById(
      "registerSubmit"
    );

  const errorBox =
    document.getElementById(
      "registerError"
    );

  if (!form) return;

  MV.ui.useBackendValidation(form);

  let submitting = false;

  form.addEventListener(
    "submit",
    async event => {
      event.preventDefault();

      if (submitting) return;

      MV.ui.clearInlineError(
        errorBox
      );

      // ConfirmPassword is not part of RegisterDto, so this is the only
      // frontend-only check kept here.
      if (
        form.elements.password.value !==
        form.elements.confirmPassword.value
      ) {
        MV.ui.showInlineError(
          errorBox,
          new MV.ApiError({
            detail:
              "Passwords do not match."
          }),
          "Account could not be created."
        );

        return;
      }

      submitting = true;

      MV.ui.buttonBusy(
        button,
        true,
        "Creating account…"
      );

      try {
        const data =
          await MV.api.post(
            "auth/register",
            {
              email:
                form.elements.email
                  .value
                  .trim(),
              userName:
                form.elements.userName
                  .value
                  .trim(),
              password:
                form.elements.password
                  .value,
              displayName:
                form.elements.displayName
                  .value
                  .trim() ||
                null
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
              "Registration failed",
            detail:
              "The server did not return an authentication token."
          });
        }

        MV.auth.setToken(
          data.token
        );

        MV.ui.setFlash(
          "Account created",
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
          "Account could not be created."
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
