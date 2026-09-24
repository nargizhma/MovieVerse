window.MV = window.MV || {};

MV.reports = (() => {

  let searchController = null;
  let searchTimer = null;

  let lastPersonQuery = "";
  let lastPeopleResults = [];



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
                  class="modal-title mb-1"
                  id="mvReportModalTitle">

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
                  "
                  id="mvReportModalSubtitle">

                  Turn MovieVerse information
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


            <div
              class="modal-body"
              id="mvReportModalBody">
            </div>


          </div>

        </div>

      </div>
      `
    );


    const modalElement =
      document.getElementById(
        "mvReportModal"
      );



    modalElement.addEventListener(
      "hidden.bs.modal",
      () => {

        if (searchController) {
          searchController.abort();
          searchController = null;
        }

        clearTimeout(
          searchTimer
        );

        lastPersonQuery = "";
        lastPeopleResults = [];

        renderHome();
      }
    );


    renderHome();
  }


  function setHeader(
    title,
    subtitle,
    icon = "fa-regular fa-file-pdf"
  ) {

    const titleElement =
      document.getElementById(
        "mvReportModalTitle"
      );


    const subtitleElement =
      document.getElementById(
        "mvReportModalSubtitle"
      );


    if (titleElement) {

      titleElement.innerHTML = `
        <i
          class="${MV.ui.escapeHtml(icon)} me-2">
        </i>

        ${MV.ui.escapeHtml(title)}
      `;
    }


    if (subtitleElement) {

      subtitleElement.textContent =
        subtitle;
    }
  }



  function renderHome() {

    const body =
      document.getElementById(
        "mvReportModalBody"
      );


    if (!body)
      return;


    setHeader(
      "Create MovieVerse Report",
      "Turn MovieVerse information into a structured PDF report."
    );


    body.innerHTML = `

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
              class="mv-report-icon">

              <i
                class="fa-solid fa-user">
              </i>

            </div>


            <h5>
              My MovieVerse Report
            </h5>


            <p>

              Export your MovieVerse activity
              into one structured report,
              including your profile, ratings,
              reviews, watchlist and
              watch history.

            </p>


            <span
              class="mv-report-action">

              Create my report

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
              class="mv-report-icon">

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

              Search for an actor, director
              or writer and create a report
              containing their biography,
              personal information and
              MovieVerse filmography.

            </p>


            <span
              class="mv-report-action">

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
          mv-report-info
          mt-4
        ">

        <i
          class="
            fa-solid
            fa-file-pdf
          ">
        </i>

        Reports are generated as PDF documents
        using the information available in
        MovieVerse.

      </div>
    `;


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
        () =>
          renderPersonSearch()
      );
  }



  function renderPersonSearch(
    restoreQuery = ""
  ) {

    const body =
      document.getElementById(
        "mvReportModalBody"
      );


    if (!body)
      return;


    setHeader(
      "Person Report",
      "Search MovieVerse actors, directors and writers.",
      "fa-solid fa-user-tie"
    );


    body.innerHTML = `

      <button
        type="button"
        class="
          btn
          btn-sm
          btn-outline-secondary
          mv-report-back
          mb-4
        "
        id="mvReportBack">

        <i
          class="
            fa-solid
            fa-arrow-left
            me-1
          ">
        </i>

        Back

      </button>


      <div class="mv-report-search-section">

        <label
          for="mvPersonSearchInput"
          class="
            form-label
            fw-semibold
          ">

          Search for a person

        </label>


        <div
          class="
            mv-report-search-box
          ">

          <i
            class="
              fa-solid
              fa-magnifying-glass
            ">
          </i>


          <input
            type="text"
            class="
              form-control
              mv-report-search-input
            "
            id="mvPersonSearchInput"
            autocomplete="off"
            maxlength="200"
            placeholder="
              Search actors, directors or writers…
            ">

        </div>


        <div
          class="
            form-text
            text-secondary
            mt-2
          ">

          Enter at least 2 characters.

        </div>

      </div>


      <div
        class="
          mv-report-search-results
          mt-4
        "
        id="mvPersonSearchResults">

        <div
          class="
            mv-report-search-empty
          ">

          <i
            class="
              fa-solid
              fa-users
            ">
          </i>

          <span>
            Search for somebody in MovieVerse
            to create their report.
          </span>

        </div>

      </div>
    `;


    document
      .getElementById(
        "mvReportBack"
      )
      ?.addEventListener(
        "click",
        renderHome
      );


    const input =
      document.getElementById(
        "mvPersonSearchInput"
      );


    input?.addEventListener(
      "input",
      () => {

        clearTimeout(
          searchTimer
        );


        const query =
          input.value.trim();


        lastPersonQuery =
          query;


        if (query.length < 2) {

          if (searchController) {

            searchController.abort();

            searchController = null;
          }


          renderPersonSearchMessage(
            query.length === 0
              ? "Search for somebody in MovieVerse to create their report."
              : "Enter at least 2 characters."
          );

          return;
        }


        searchTimer =
          setTimeout(
            () =>
              searchPeople(query),
            300
          );
      }
    );



    if (restoreQuery) {

      input.value =
        restoreQuery;


      lastPersonQuery =
        restoreQuery;


      if (
        lastPeopleResults.length
      ) {

        renderPeopleResults(
          lastPeopleResults
        );
      }
      else {

        searchPeople(
          restoreQuery
        );
      }
    }


    setTimeout(
      () => input?.focus(),
      150
    );
  }



  async function searchPeople(
    query
  ) {

    const resultHost =
      document.getElementById(
        "mvPersonSearchResults"
      );


    if (!resultHost)
      return;


    if (searchController) {

      searchController.abort();
    }


    searchController =
      new AbortController();


    resultHost.innerHTML = `

      <div
        class="
          mv-report-search-loading
        ">

        <span
          class="
            spinner-border
            spinner-border-sm
          ">
        </span>

        Searching MovieVerse…

      </div>
    `;


    try {

      const data =
        await MV.api.get(
          "search",
          {
            query,
            limit: 10
          },
          {
            signal:
              searchController.signal,

            auth: false
          }
        );


      const people = [

        ...(data?.actors || []),

        ...(data?.directors || []),

        ...(data?.writers || [])

      ];


      lastPeopleResults =
        people;


      renderPeopleResults(
        people
      );

    }
    catch (error) {

      if (
        error?.name ===
        "AbortError"
      ) {
        return;
      }


      resultHost.innerHTML = `

        <div
          class="
            alert
            alert-danger
            mb-0
          ">

          <i
            class="
              fa-solid
              fa-circle-exclamation
              me-2
            ">
          </i>

          ${
            MV.ui.escapeHtml(
              error?.detail ||
              error?.message ||
              "Could not search for people."
            )
          }

        </div>
      `;
    }
  }


  function renderPeopleResults(
    people
  ) {

    const resultHost =
      document.getElementById(
        "mvPersonSearchResults"
      );


    if (!resultHost)
      return;


    if (!people.length) {

      resultHost.innerHTML = `

        <div
          class="
            mv-report-search-empty
          ">

          <i
            class="
              fa-solid
              fa-user-slash
            ">
          </i>

          <span>
            No actors, directors or writers
            were found.
          </span>

        </div>
      `;

      return;
    }


    resultHost.innerHTML = `

      <div
        class="
          mv-report-result-count
          mb-2
        ">

        ${
          people.length === 1
            ? "1 person found"
            : `${people.length} people found`
        }

      </div>


      <div
        class="
          mv-report-person-list
        ">

        ${
          people
            .map(
              (person, index) =>
                personResultHtml(
                  person,
                  index
                )
            )
            .join("")
        }

      </div>
    `;


    resultHost
      .querySelectorAll(
        "[data-person-index]"
      )
      .forEach(
        button => {

          button.addEventListener(
            "click",
            () => {

              const index =
                Number(
                  button.dataset
                    .personIndex
                );


              const person =
                people[index];


              if (!person)
                return;


              renderSelectedPerson(
                person
              );
            }
          );
        }
      );
  }


  function personResultHtml(
    person,
    index
  ) {

    const imageUrl =
      MV.media
        .searchResultImageUrl(
          person
        );


    return `

      <button
        type="button"
        class="
          mv-report-person-result
        "
        data-person-index="${index}">


        <img
          src="${
            MV.ui.escapeHtml(
              imageUrl
            )
          }"

          ${
            MV.media
              .imageFallbackAttributes(
                "people",
                false
              )
          }

          alt="${
            MV.ui.escapeHtml(
              person.title
            )
          }">


        <div
          class="
            mv-report-person-info
          ">

          <strong>

            ${
              MV.ui.escapeHtml(
                person.title
              )
            }

          </strong>


          <span>

            ${
              MV.ui.escapeHtml(
                person.resultType
              )
            }

          </span>

        </div>


        <div
          class="
            mv-report-person-arrow
          ">

          <i
            class="
              fa-solid
              fa-chevron-right
            ">
          </i>

        </div>

      </button>
    `;
  }


  function renderPersonSearchMessage(
    message
  ) {

    const resultHost =
      document.getElementById(
        "mvPersonSearchResults"
      );


    if (!resultHost)
      return;


    resultHost.innerHTML = `

      <div
        class="
          mv-report-search-empty
        ">

        <i
          class="
            fa-solid
            fa-magnifying-glass
          ">
        </i>

        <span>

          ${
            MV.ui.escapeHtml(
              message
            )
          }

        </span>

      </div>
    `;
  }

  function renderSelectedPerson(
    person
  ) {

    const body =
      document.getElementById(
        "mvReportModalBody"
      );


    if (!body)
      return;


    setHeader(
      "Person Report",
      "Review your selection before generating the report.",
      "fa-solid fa-user-tie"
    );


    const imageUrl =
      MV.media
        .searchResultImageUrl(
          person
        );


    body.innerHTML = `

      <button
        type="button"
        class="
          btn
          btn-sm
          btn-outline-secondary
          mv-report-back
          mb-4
        "
        id="mvBackToPersonSearch">

        <i
          class="
            fa-solid
            fa-arrow-left
            me-1
          ">
        </i>

        Back to search

      </button>


      <div
        class="
          mv-report-selected-person
        ">


        <div
          class="
            mv-report-selected-header
          ">

          <img
            src="${
              MV.ui.escapeHtml(
                imageUrl
              )
            }"

            ${
              MV.media
                .imageFallbackAttributes(
                  "people",
                  false
                )
            }

            alt="${
              MV.ui.escapeHtml(
                person.title
              )
            }">


          <div>

            <span
              class="
                badge
                mv-report-person-type
                mb-2
              ">

              ${
                MV.ui.escapeHtml(
                  person.resultType
                )
              }

            </span>


            <h4 class="mb-1">

              ${
                MV.ui.escapeHtml(
                  person.title
                )
              }

            </h4>


            <p
              class="
                text-secondary
                mb-0
              ">

              MovieVerse person report

            </p>

          </div>

        </div>


        <div
          class="
            mv-report-includes
            mt-4
          ">

          <h6>
            Your report will include
          </h6>


          <div
            class="
              mv-report-includes-grid
            ">

            <div>

              <i
                class="
                  fa-solid
                  fa-address-card
                ">
              </i>

              Biography and
              personal information

            </div>


            <div>

              <i
                class="
                  fa-solid
                  fa-film
                ">
              </i>

              MovieVerse filmography

            </div>


            <div>

              <i
                class="
                  fa-solid
                  fa-circle-info
                ">
              </i>

              Available career and
              personal details

            </div>


            <div>

              <i
                class="
                  fa-solid
                  fa-file-pdf
                ">
              </i>

              Structured PDF document

            </div>

          </div>

        </div>


        <div
          class="
            d-flex
            justify-content-end
            mt-4
          ">

          <button
            type="button"
            class="
              btn
              btn-primary
              px-4
            "
            id="mvGeneratePersonReport">

            <i
              class="
                fa-regular
                fa-file-pdf
                me-2
              ">
            </i>

            Generate report

          </button>

        </div>

      </div>
    `;


    document
      .getElementById(
        "mvBackToPersonSearch"
      )
      ?.addEventListener(
        "click",
        () =>
          renderPersonSearch(
            lastPersonQuery
          )
      );


    document
      .getElementById(
        "mvGeneratePersonReport"
      )
      ?.addEventListener(
        "click",
        event =>
          generatePersonReport(
            person,
            event.currentTarget
          )
      );
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


    button.disabled =
      true;


    button.innerHTML = `

      <div
        class="
          mv-report-generating
        ">

        <span
          class="
            spinner-border
            spinner-border-sm
          ">
        </span>

        <span>
          Generating your report…
        </span>

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


      hideModal();

    }
    catch (error) {

      MV.ui.showError(
        error,
        "Could not generate your report."
      );

    }
    finally {

      if (
        document.body.contains(
          button
        )
      ) {

        button.disabled =
          false;

        button.innerHTML =
          oldHtml;
      }
    }
  }



  async function generatePersonReport(
    person,
    button
  ) {

    if (
      !person?.id ||
      !person?.resultType
    ) {

      MV.ui.toast(
        "The selected person is invalid.",
        "danger"
      );

      return;
    }


    const oldHtml =
      button.innerHTML;


    button.disabled =
      true;


    button.innerHTML = `

      <span
        class="
          spinner-border
          spinner-border-sm
          me-2
        ">
      </span>

      Generating…
    `;


    try {

      const type =
        String(
          person.resultType
        )
          .trim()
          .toLowerCase();


      const path =
        `reports/person/${
          encodeURIComponent(type)
        }/${
          encodeURIComponent(
            person.id
          )
        }`;


      await openPdf(
        path
      );


      MV.ui.toast(
        `${person.title} report was generated.`,
        "success"
      );


      hideModal();

    }
    catch (error) {

      MV.ui.showError(
        error,
        "Could not generate the person report."
      );

    }
    finally {

      if (
        document.body.contains(
          button
        )
      ) {

        button.disabled =
          false;

        button.innerHTML =
          oldHtml;
      }
    }
  }



  async function openPdf(
    path
  ) {

    const token =
      MV.auth.getToken();


    if (!token) {

      location.href =
        MV.auth.loginUrl();

      return;
    }



    const pdfTab =
      window.open(
        "",
        "_blank"
      );


    if (!pdfTab) {

      throw new Error(
        "The browser blocked the PDF tab. Please allow pop-ups for MovieVerse."
      );
    }



    pdfTab.document.write(`
      <!DOCTYPE html>

      <html>

        <head>

          <meta charset="UTF-8">

          <title>
            Generating MovieVerse report…
          </title>

        </head>


        <body
          style="
            margin:0;
            min-height:100vh;
            display:flex;
            align-items:center;
            justify-content:center;
            background:#070b12;
            color:#f3f7fb;
            font-family:
              Arial,
              sans-serif;
          ">

          <div
            style="
              text-align:center;
            ">

            <div
              style="
                font-size:24px;
                font-weight:700;
                margin-bottom:10px;
              ">

              MovieVerse

            </div>


            <div
              style="
                color:#9eacc0;
              ">

              Generating your report…

            </div>

          </div>

        </body>

      </html>
    `);


    pdfTab.document.close();


    try {

      const response =
        await fetch(
          MV.api.buildUrl(
            path
          ),
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


        if (
          response.status ===
          401
        ) {

          MV.auth.clearToken();
        }


        const messages =
          MV.api
            .extractErrorMessages(
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
            type:
              "application/pdf"
          }
        );


      const objectUrl =
        URL.createObjectURL(
          pdfBlob
        );


      pdfTab.location.href =
        objectUrl;


      setTimeout(
        () => {

          URL.revokeObjectURL(
            objectUrl
          );

        },
        60_000
      );

    }
    catch (error) {

      if (
        pdfTab &&
        !pdfTab.closed
      ) {

        pdfTab.close();
      }


      throw error;
    }
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

    renderHome();


    const modalElement =
      document.getElementById(
        "mvReportModal"
      );


    bootstrap.Modal
      .getOrCreateInstance(
        modalElement
      )
      .show();
  }


  function hideModal() {

    const modalElement =
      document.getElementById(
        "mvReportModal"
      );


    if (!modalElement)
      return;


    bootstrap.Modal
      .getInstance(
        modalElement
      )
      ?.hide();
  }



  function bind() {

    const button =
      document.getElementById(
        "mvOpenReport"
      );


    if (!button)
      return;



    if (
      button.dataset
        .mvReportBound ===
      "true"
    ) {
      return;
    }


    button.dataset
      .mvReportBound =
      "true";


    button.addEventListener(
      "click",
      open
    );
  }


  return {
    bind,
    open
  };

})();