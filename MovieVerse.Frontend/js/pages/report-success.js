window.MV =
  window.MV || {};


document.addEventListener(
  "DOMContentLoaded",
  initializeReportSuccess
);


async function initializeReportSuccess() {


  if (!MV.auth.isAuthenticated()) {

    showError(
      "Your MovieVerse session is no longer available. Please sign in again."
    );

    return;
  }

  const params =
    new URLSearchParams(
      location.search
    );


  const sessionId =
    params.get(
      "session_id"
    );


  if (!sessionId) {

    showError(
      "Stripe session information is missing."
    );

    return;
  }


  try {

    setStatus(
      "Confirming your payment",
      "MovieVerse is verifying your Stripe payment."
    );

    const purchase =
      await MV.api.post(
        "report-payments/confirm",
        null,
        {
          query: {
            sessionId
          }
        }
      );


    if (
      !purchase ||
      !purchase.purchaseId
    ) {

      throw new Error(
        "MovieVerse could not identify the purchased report."
      );
    }


    setStatus(
      "Payment confirmed",
      "Your payment was successful. MovieVerse is generating your PDF report."
    );


    await openPurchasedReport(
      purchase.purchaseId
    );

  }
  catch (error) {

    console.error(
      "Report payment confirmation failed:",
      error
    );


    showError(
      error?.detail
      ||
      error?.message
      ||
      "MovieVerse could not verify your payment."
    );
  }
}


async function openPurchasedReport(
  purchaseId
) {

  const token =
    MV.auth.getToken();


  if (!token) {

    throw new Error(
      "Authentication is required to open this report."
    );
  }


  const url =
    MV.api.buildUrl(
      `reports/purchase/${
        encodeURIComponent(
          purchaseId
        )
      }`
    );


  const response =
    await fetch(
      url,
      {
        method:
          "GET",

        headers: {
          Authorization:
            `Bearer ${token}`
        },

        cache:
          "no-store"
      }
    );


  if (!response.ok) {

    let data =
      null;


    try {

      const contentType =
        response.headers.get(
          "content-type"
        ) || "";


      if (
        contentType
          .toLowerCase()
          .includes("json")
      ) {

        data =
          await response.json();

      }
      else {

        data =
          await response.text();

      }

    }
    catch {

      data =
        null;

    }


    const messages =
      MV.api.extractErrorMessages(
        data,
        "Could not generate the purchased report."
      );


    throw new MV.ApiError({
      status:
        response.status,

      title:
        "Report generation failed",

      detail:
        messages[0],

      messages,

      data,

      url:
        response.url
    });
  }


  const blob =
    await response.blob();


  const pdfBlob =
    new Blob(
      [blob],
      {
        type:
          "application/pdf"
      }
    );


  const objectUrl =
    URL.createObjectURL(
      pdfBlob
    );


  setStatus(
    "Report ready",
    "Opening your MovieVerse report..."
  );



  location.replace(
    objectUrl
  );


  setTimeout(
    () => {

      URL.revokeObjectURL(
        objectUrl
      );

    },
    60_000
  );
}


function setStatus(
  title,
  message
) {

  const titleElement =
    document.getElementById(
      "reportStatusTitle"
    );


  const messageElement =
    document.getElementById(
      "reportStatusMessage"
    );


  if (titleElement) {

    titleElement.textContent =
      title;

  }


  if (messageElement) {

    messageElement.textContent =
      message;

  }
}



function showError(
  message
) {

  const loader =
    document.getElementById(
      "reportLoader"
    );


  const error =
    document.getElementById(
      "reportError"
    );


  const actions =
    document.getElementById(
      "reportActions"
    );


  const icon =
    document.getElementById(
      "reportStatusIcon"
    );


  if (loader) {

    loader.style.display =
      "none";

  }


  if (error) {

    error.textContent =
      message;

    error.style.display =
      "block";

  }


  if (actions) {

    actions.style.display =
      "block";

  }


  if (icon) {

    icon.innerHTML = `
      <i
        class="
          fa-solid
          fa-circle-exclamation
        ">
      </i>
    `;

  }


  setStatus(
    "Report could not be opened",
    "Something went wrong while confirming or generating your report."
  );
}