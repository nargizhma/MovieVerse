window.MV = window.MV || {};

MV.reports = (() => {

  function ensureModal() {

    if (
      document.getElementById(
        "mvReportModal"
      )
    ) {
      return;
    }


    document.body.insertAdjacentHTML(
      "beforeend",
      `
      <div
        class="modal fade"
        id="mvReportModal"
        tabindex="-1"
        aria-hidden="true">

        <div
          class="
            modal-dialog
            modal-dialog-centered
            modal-lg
          ">

          <div
            class="
              modal-content
              mv-report-modal
            ">


            <div class="modal-header">

              <div>

                <h5
                  class="
                    modal-title
                    mb-1
                  ">

                  <i
                    class="
                      fa-regular
                      fa-file-pdf
                      me-2
                    ">
                  </i>

                  Create MovieVerse Report

                </h5>


                <p
                  class="
                    text-secondary
                    small
                    mb-0
                  ">

                  Turn your MovieVerse data
                  into a structured PDF report.

                </p>

              </div>


              <button
                type="button"
                class="btn-close"
                data-bs-dismiss="modal"
                aria-label="Close">
              </button>

            </div>


            <div class="modal-body">

              <div class="row g-3">


                <!-- MY REPORT -->

                <div class="col-md-6">

                  <button
                    type="button"
                    class="
                      mv-report-option
                      w-100
                    "
                    id="mvMyReport">

                    <div
                      class="
                        mv-report-icon
                      ">

                      <i
                        class="
                          fa-solid
                          fa-user
                        ">
                      </i>

                    </div>


                    <h5>
                      My MovieVerse Report
                    </h5>


                    <p>

                      Export your profile activity,
                      ratings, written reviews,
                      watchlist and watch history
                      into one structured PDF.

                    </p>


                    <span
                      class="
                        mv-report-action
                      ">

                      Generate PDF

                      <i
                        class="
                          fa-solid
                          fa-arrow-down
                          ms-1
                        ">
                      </i>

                    </span>

                  </button>

                </div>


                <!-- PERSON REPORT -->

                <div class="col-md-6">

                  <button
                    type="button"
                    class="
                      mv-report-option
                      w-100
                    "
                    id="mvPersonReport">

                    <div
                      class="
                        mv-report-icon
                      ">

                      <i
                        class="
                          fa-solid
                          fa-clapperboard
                        ">
                      </i>

                    </div>


                    <h5>
                      Person Report
                    </h5>


                    <p>

                      Create a structured report
                      about an actor, director or
                      writer including biography,
                      personal details and
                      filmography.

                    </p>


                    <span
                      class="
                        mv-report-action
                      ">

                      Choose a person

                      <i
                        class="
                          fa-solid
                          fa-arrow-right
                          ms-1
                        ">
                      </i>

                    </span>

                  </button>

                </div>

              </div>


              <div
                class="
                  text-secondary
                  small
                  mt-4
                  d-flex
                  align-items-center
                  gap-2
                ">

                <i
                  class="
                    fa-solid
                    fa-lock
                  ">
                </i>

                Payment will be required
                before generation once the
                Stripe integration is enabled.

              </div>

            </div>

          </div>

        </div>

      </div>
      `
    );


    document
      .getElementById(
        "mvMyReport"
      )
      ?.addEventListener(
        "click",
        generateMyReport
      );


    document
      .getElementById(
        "mvPersonReport"
      )
      ?.addEventListener(
        "click",
        openPersonReport
      );
  }


  function open() {

    if (
      !MV.auth.isAuthenticated()
    ) {

      location.href =
        MV.auth.loginUrl();

      return;
    }


    ensureModal();


    const modalElement =
      document.getElementById(
        "mvReportModal"
      );


    const modal =
      bootstrap.Modal
        .getOrCreateInstance(
          modalElement
        );


    modal.show();
  }


  async function generateMyReport() {

    const button =
      document.getElementById(
        "mvMyReport"
      );


    if (!button)
      return;


    const oldHtml =
      button.innerHTML;


    button.disabled = true;


    button.innerHTML = `
      <div
        class="
          d-flex
          align-items-center
          justify-content-center
          gap-2
          py-4
        ">

        <span
          class="
            spinner-border
            spinner-border-sm
          ">
        </span>

        Generating report...

      </div>
    `;


    try {

      await openPdf(
          "reports/me"
        );


      MV.ui.toast(
        "Your MovieVerse report was generated.",
        "success"
      );

    }
    catch (error) {

      MV.ui.showError(
        error,
        "Could not generate your report."
      );

    }
    finally {

      button.disabled = false;

      button.innerHTML =
        oldHtml;
    }
  }

  function openPersonReport() {

    MV.ui.toast(
      "Person report selection will be connected next.",
      "info"
    );
  }



async function openPdf(path) {

  const token =
    MV.auth.getToken();

  if (!token) {
    location.href =
      MV.auth.loginUrl();

    return;
  }


  const pdfTab =
    window.open("", "_blank");


  if (!pdfTab) {
    throw new Error(
      "The browser blocked the PDF tab. Please allow pop-ups for MovieVerse."
    );
  }


  pdfTab.document.write(`
    <html>
      <head>
        <title>Generating report...</title>
      </head>

      <body style="
        background:#111;
        color:#fff;
        font-family:Arial,sans-serif;
        display:flex;
        align-items:center;
        justify-content:center;
        height:100vh;
        margin:0;
      ">
        Generating your MovieVerse report...
      </body>
    </html>
  `);


  try {

    const response =
      await fetch(
        MV.api.buildUrl(path),
        {
          method: "GET",

          headers: {
            Authorization:
              `Bearer ${token}`
          },

          cache: "no-store"
        }
      );


    if (!response.ok) {

      pdfTab.close();

      let data = null;


      try {

        const contentType =
          response.headers.get(
            "content-type"
          ) || "";


        if (
          contentType.includes("json")
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
        data = null;
      }


      if (response.status === 401) {
        MV.auth.clearToken();
      }


      const messages =
        MV.api.extractErrorMessages(
          data,
          "Could not generate report."
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
          type: "application/pdf"
        }
      );


    const objectUrl =
      URL.createObjectURL(
        pdfBlob
      );


 
    pdfTab.location.href =
      objectUrl;



    setTimeout(
      () =>
        URL.revokeObjectURL(
          objectUrl
        ),
      60_000
    );

  }
  catch (error) {

    if (!pdfTab.closed) {
      pdfTab.close();
    }

    throw error;
  }
}


  function bind() {

    document
      .getElementById(
        "mvOpenReport"
      )
      ?.addEventListener(
        "click",
        open
      );
  }


  return {
    bind,
    open
  };

})();