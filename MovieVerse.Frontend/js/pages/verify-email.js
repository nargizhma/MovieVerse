document.addEventListener(
  "DOMContentLoaded",
  async () => {

    const params =
      new URLSearchParams(
        location.search
      );

    const userId =
      params.get("userId");

    const token =
      params.get("token");

    const email =
      params.get("email");

    const returnUrl =
      params.get("returnUrl");

    const message =
      document.getElementById(
        "verificationMessage"
      );

    const errorBox =
      document.getElementById(
        "verificationError"
      );

    const resendButton =
      document.getElementById(
        "resendButton"
      );

    const loginLink =
      document.getElementById(
        "loginLink"
      );

    if (!userId) {
      message.textContent =
        "The verification link is invalid.";

      resendButton.classList.add(
        "d-none"
      );

      return;
    }

    function verified() {
      message.textContent =
        "Your email has been verified successfully.";

      resendButton.classList.add(
        "d-none"
      );

      loginLink.classList.remove(
        "d-none"
      );

      if (returnUrl) {
        loginLink.href =
          `login.html?returnUrl=${
            encodeURIComponent(
              returnUrl
            )
          }`;
      }
    }

    // ---------------------------------
    // USER OPENED LINK FROM THE EMAIL
    // ---------------------------------

    if (token) {
      resendButton.classList.add(
        "d-none"
      );

      message.textContent =
        "Verifying your email…";

      try {
        await MV.api.post(
          "auth/confirm-email",
          {
            userId,
            token
          },
          {
            auth: false,
            handle401: false
          }
        );

        verified();
      } catch (err) {
        MV.ui.showInlineError(
          errorBox,
          err,
          "Email verification failed."
        );

        message.textContent =
          "We could not verify this email.";
      }

      return;
    }

    // ---------------------------------
    // NORMAL WAITING PAGE
    // ---------------------------------

    if (email) {
      message.textContent =
        `We sent a verification email to ${email}. ` +
        `Waiting for verification…`;
    }

    const connection =
      new signalR.HubConnectionBuilder()
        .withUrl(
          MV.config
            .EMAIL_VERIFICATION_HUB_URL
        )
        .withAutomaticReconnect()
        .build();

    connection.on(
      "EmailVerified",
      () => {
        verified();
      }
    );

    try {
      await connection.start();

      await connection.invoke(
        "WatchVerification",
        userId
      );
    } catch (err) {
      console.error(
        "SignalR connection failed:",
        err
      );

      // Email verification itself still works.
      // Only automatic live updating is unavailable.
      message.textContent +=
        " Refresh this page after verifying if necessary.";
    }

    resendButton.addEventListener(
      "click",
      async () => {

        if (!email)
            return;

        MV.ui.clearInlineError(
          errorBox
        );

        MV.ui.buttonBusy(
          resendButton,
          true,
          "Sending…"
        );

        try {
          await MV.api.post(
            "auth/resend-confirmation",
            {
              email
            },
            {
              auth: false,
              handle401: false
            }
          );

          message.textContent =
            `A new verification email was sent to ${email}.`;
        } catch (err) {
          MV.ui.showInlineError(
            errorBox,
            err,
            "Could not resend the verification email."
          );
        } finally {
          MV.ui.buttonBusy(
            resendButton,
            false
          );
        }
      }
    );
  }
);