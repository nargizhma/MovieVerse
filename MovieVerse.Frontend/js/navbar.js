window.MV = window.MV || {};

MV.navbar = (() => {

  let activeIndex = -1;
  let currentItems = [];
  let controller = null;


  function relativePrefix() {
    return location.pathname.includes("/admin/")
      ? "../"
      : "";
  }


  function render() {

    const host =
      document.getElementById("site-navbar");

    if (!host) return;


    const p = relativePrefix();

    const user =
      MV.auth.getUser();


    const userLinks = user
      ? `
        <li class="nav-item">

          <a
            class="nav-link mv-nav-link"
            href="${p}profile.html#watchlist">

            <i class="fa-regular fa-bookmark me-1"></i>

            Watchlist
          </a>

        </li>


        ${
          MV.auth.isAdmin()
            ? `
              <li class="nav-item">

                <a
                  class="nav-link mv-nav-link"
                  href="${p}admin/dashboard.html">

                  <i class="fa-solid fa-gauge-high me-1"></i>

                  Admin
                </a>

              </li>
            `
            : ""
        }


        <li class="nav-item dropdown">

          <button
            class="
              nav-link
              mv-nav-link
              dropdown-toggle
              d-flex
              align-items-center
              gap-2
            "
            type="button"
            data-bs-toggle="dropdown"
            aria-label="Open profile menu">

            <img
              id="mvNavAvatar"
              class="mv-nav-avatar"
              src="${MV.media.profileImageUrl(
                null,
                Boolean(p)
              )}"
              ${MV.media.imageFallbackAttributes(
                "people",
                Boolean(p)
              )}
              alt="Profile picture">

            <span class="d-lg-none">
              ${MV.ui.escapeHtml(user.userName)}
            </span>

          </button>


          <ul class="dropdown-menu dropdown-menu-end">


            <!-- PROFILE -->

            <li>

              <a
                class="dropdown-item text-light"
                href="${p}profile.html">

                <i class="fa-regular fa-user me-2"></i>

                Profile

              </a>

            </li>


            <!-- GENERATE REPORT -->

            <li>

              <button
                class="dropdown-item text-light"
                type="button"
                id="mvOpenReport">

                <i class="fa-regular fa-file-pdf me-2"></i>

                Generate report

              </button>

            </li>


            <li>
              <hr class="dropdown-divider">
            </li>


            <!-- LOGOUT -->

            <li>

              <button
                class="dropdown-item text-light"
                type="button"
                id="mvLogout">

                <i
                  class="
                    fa-solid
                    fa-arrow-right-from-bracket
                    me-2
                  ">
                </i>

                Logout

              </button>

            </li>


          </ul>

        </li>
      `
      : `
        <li class="nav-item">

          <a
            class="nav-link mv-nav-link"
            href="${p}login.html">

            Login

          </a>

        </li>


        <li class="nav-item">

          <a
            class="btn btn-primary ms-lg-1"
            href="${p}register.html">

            Register

          </a>

        </li>
      `;


    host.innerHTML = `

      <nav class="navbar navbar-expand-lg mv-navbar">

        <div class="container-fluid px-3">


          <!-- LOGO -->

          <a
            class="navbar-brand mv-brand"
            href="${p}index.html">

            <img
              src="${p}assets/images/logo-horizontal.png"
              alt="MovieVerse">

          </a>


          <!-- MOBILE MENU -->

          <button
            class="navbar-toggler border-secondary text-light"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#mvNavbarCollapse"
            aria-controls="mvNavbarCollapse"
            aria-expanded="false"
            aria-label="Toggle navigation">

            <i class="fa-solid fa-bars"></i>

          </button>


          <div
            class="collapse navbar-collapse gap-lg-3"
            id="mvNavbarCollapse">


            <!-- GLOBAL SEARCH -->

            <div
              class="mv-search-shell mx-lg-auto"
              id="mvSearchShell">

              <i
                class="
                  fa-solid
                  fa-magnifying-glass
                  mv-search-icon
                ">
              </i>


              <input
                class="form-control mv-search-input"
                id="mvGlobalSearch"
                autocomplete="off"
                maxlength="200"
                aria-autocomplete="list"
                aria-controls="mvSearchDropdown"
                aria-expanded="false"
                placeholder="
                  Search movies, TV shows,
                  actors, directors, writers…
                ">


              <div
                class="mv-search-dropdown d-none"
                id="mvSearchDropdown"
                role="listbox">
              </div>

            </div>


            <!-- USER LINKS -->

            <ul
              class="
                navbar-nav
                align-items-lg-center
                ms-lg-auto
              ">

              ${userLinks}

            </ul>


          </div>

        </div>

      </nav>
    `;


    /*
     * LOGOUT
     */

    document
      .getElementById("mvLogout")
      ?.addEventListener(
        "click",
        () => {

          MV.auth.clearToken();

          location.href =
            `${p}index.html`;
        }
      );


    /*
     * SEARCH
     */

    bindSearch(p);


    /*
     * LOGGED-IN USER FEATURES
     */

    if (user) {

      refreshAvatar();

      /*
       * reports.js handles the report modal.
       * Optional chaining prevents errors
       * if reports.js has not loaded.
       */

      MV.reports?.bind?.();
    }
  }


  /*
   * =========================================================
   * SEARCH HISTORY
   * =========================================================
   */


  function searchHistoryKey() {

    const userId =
      MV.auth.getUser()?.id || "guest";

    return `movieverse_search_history_${userId}`;
  }


  function getSearchHistory() {

    try {

      return JSON.parse(
        localStorage.getItem(
          searchHistoryKey()
        ) || "[]"
      )
        .filter(Boolean)
        .slice(0, 3);

    }
    catch {

      return [];
    }
  }


  function rememberSearch(query) {

    const clean =
      String(query || "").trim();

    if (!clean) return;


    const history =
      getSearchHistory()
        .filter(
          x =>
            x.toLowerCase()
            !== clean.toLowerCase()
        );


    history.unshift(clean);


    localStorage.setItem(
      searchHistoryKey(),
      JSON.stringify(
        history.slice(0, 3)
      )
    );
  }


  /*
   * =========================================================
   * SEARCH DISCOVERY
   * =========================================================
   */


  function discovery(prefix) {

    const history =
      getSearchHistory();


    const recent =
      history.length
        ? `
          <div
            class="
              mv-search-group-title
              px-0
              pt-0
            ">

            Recent searches

          </div>


          <div
            class="
              mv-discovery-grid
              mb-3
            ">

            ${
              history
                .map(
                  query => `
                    <a
                      href="
                        ${prefix}search.html?q=${
                          encodeURIComponent(query)
                        }
                      ">

                      <i
                        class="
                          fa-solid
                          fa-clock-rotate-left
                          me-1
                        ">
                      </i>

                      ${
                        MV.ui.escapeHtml(query)
                      }

                    </a>
                  `
                )
                .join("")
            }

          </div>
        `
        : "";


    return `
      <div class="mv-discovery">

        ${recent}


        <div
          class="
            mv-search-group-title
            px-0
            pt-0
          ">

          Discover

        </div>


        <div class="mv-discovery-grid">

          <a href="${prefix}search.html">
            Browse titles
          </a>


          <a
            href="${prefix}search.html?type=Movie">

            Movies

          </a>


          <a
            href="${prefix}search.html?type=TVShow">

            TV Shows

          </a>


          <a
            href="${prefix}search.html?type=People">

            People

          </a>


          <a
            href="
              ${prefix}search.html?sortBy=rating&desc=true
            ">

            Top rated

          </a>


          <a
            href="
              ${prefix}search.html?year=${
                new Date().getFullYear()
              }
            ">

            This year

          </a>

        </div>

      </div>
    `;
  }


  /*
   * =========================================================
   * GLOBAL SEARCH
   * =========================================================
   */


  function bindSearch(prefix) {

    const input =
      document.getElementById(
        "mvGlobalSearch"
      );


    const dropdown =
      document.getElementById(
        "mvSearchDropdown"
      );


    if (!input || !dropdown)
      return;


    const show = html => {

      dropdown.innerHTML = html;

      dropdown.classList.remove(
        "d-none"
      );


      input.setAttribute(
        "aria-expanded",
        "true"
      );


      activeIndex = -1;


      currentItems = [
        ...dropdown.querySelectorAll(
          ".mv-search-result"
        )
      ];
    };


    const hide = () => {

      dropdown.classList.add(
        "d-none"
      );


      input.setAttribute(
        "aria-expanded",
        "false"
      );


      activeIndex = -1;

      currentItems = [];
    };


    /*
     * Show discovery when focused
     * and search is empty.
     */

    input.addEventListener(
      "focus",
      () => {

        if (!input.value.trim()) {

          show(
            discovery(prefix)
          );
        }
      }
    );


    /*
     * SEARCH REQUEST
     */

    const search =
      MV.ui.debounce(
        async () => {

          const q =
            input.value.trim();


          if (!q) {

            show(
              discovery(prefix)
            );

            return;
          }


          if (controller)
            controller.abort();


          controller =
            new AbortController();


          dropdown.classList.remove(
            "d-none"
          );


          dropdown.innerHTML = `
            <div class="p-3 text-secondary">

              <span
                class="
                  spinner-border
                  spinner-border-sm
                  me-2
                ">
              </span>

              Searching…

            </div>
          `;


          try {

            const data =
              await MV.api.get(
                "search",
                {
                  query: q,
                  limit: 5
                },
                {
                  signal:
                    controller.signal,

                  auth: false
                }
              );


            show(
              renderResults(
                data,
                prefix,
                q
              )
            );
          }

          catch (err) {

            if (
              err.name !==
              "AbortError"
            ) {

              dropdown.innerHTML = `
                <div
                  class="
                    p-3
                    text-secondary
                  ">

                  ${
                    MV.ui.escapeHtml(
                      err.detail
                      || "Search unavailable"
                    )
                  }

                </div>
              `;
            }
          }

        },
        300
      );


    input.addEventListener(
      "input",
      search
    );


    /*
     * KEYBOARD NAVIGATION
     */

    input.addEventListener(
      "keydown",
      event => {

        if (
          event.key ===
          "Escape"
        ) {

          hide();

          return;
        }


        if (
          event.key ===
          "Enter"
        ) {

          event.preventDefault();


          const query =
            input.value.trim();


          if (
            activeIndex >= 0
            &&
            currentItems[
              activeIndex
            ]
          ) {

            rememberSearch(
              query
            );


            location.href =
              currentItems[
                activeIndex
              ].dataset.href;
          }

          else if (query) {

            rememberSearch(
              query
            );


            location.href =
              `${prefix}search.html?q=${
                encodeURIComponent(query)
              }`;
          }


          return;
        }


        if (
          !currentItems.length
          ||
          ![
            "ArrowDown",
            "ArrowUp"
          ].includes(
            event.key
          )
        ) {

          return;
        }


        event.preventDefault();


        activeIndex =
          event.key ===
          "ArrowDown"

            ? Math.min(
                currentItems.length - 1,
                activeIndex + 1
              )

            : Math.max(
                0,
                activeIndex - 1
              );


        currentItems.forEach(
          (el, i) => {

            el.classList.toggle(
              "is-active",
              i === activeIndex
            );
          }
        );


        currentItems[
          activeIndex
        ]?.scrollIntoView({
          block: "nearest"
        });
      }
    );


    /*
     * CLICK SEARCH RESULT
     */

    dropdown.addEventListener(
      "click",
      event => {

        const item =
          event.target.closest(
            ".mv-search-result"
          );


        if (item) {

          rememberSearch(
            input.value.trim()
          );


          location.href =
            item.dataset.href;


          return;
        }


        if (
          event.target.closest(
            ".js-search-all"
          )
        ) {

          rememberSearch(
            input.value.trim()
          );
        }
      }
    );


    /*
     * CLICK OUTSIDE SEARCH
     */

    document.addEventListener(
      "click",
      event => {

        if (
          !document
            .getElementById(
              "mvSearchShell"
            )
            ?.contains(
              event.target
            )
        ) {

          hide();
        }
      }
    );
  }


  /*
   * =========================================================
   * SEARCH RESULTS
   * =========================================================
   */


  function renderResults(
    data,
    prefix,
    q
  ) {

    const groups = [

      [
        "Movies",
        data.movies || []
      ],

      [
        "TV Shows",
        data.tvShows || []
      ],

      [
        "People",
        [
          ...(data.actors || []),
          ...(data.directors || []),
          ...(data.writers || [])
        ]
      ]
    ];


    let html = "";

    let count = 0;


    for (
      const [label, items]
      of groups
    ) {

      if (!items.length)
        continue;


      count +=
        items.length;


      html += `
        <div
          class="mv-search-group-title">

          ${label}

        </div>
      `;


      html +=
        items
          .map(
            item => {

              const href =
                resultHref(
                  item,
                  prefix
                );


              const kind =
                MV.media
                  .searchResultPlaceholderKind(
                    item
                  );


              return `
                <div
                  class="mv-search-result"
                  role="option"
                  tabindex="-1"
                  data-href="${
                    MV.ui.escapeHtml(
                      href
                    )
                  }">


                  <img
                    src="${
                      MV.ui.escapeHtml(
                        MV.media
                          .searchResultImageUrl(
                            item,
                            Boolean(
                              prefix
                            )
                          )
                      )
                    }"

                    ${
                      MV.media
                        .imageFallbackAttributes(
                          kind,
                          Boolean(
                            prefix
                          )
                        )
                    }

                    alt="">


                  <div>

                    <strong>

                      ${
                        MV.ui.escapeHtml(
                          item.title
                        )
                      }

                    </strong>


                    <small>

                      ${
                        MV.ui.escapeHtml(
                          MV.ui.optionalText(
                            item.subtitle
                          )
                          ||
                          item.resultType
                        )
                      }

                    </small>

                  </div>


                  <span
                    class="
                      badge
                      mv-badge
                    ">

                    ${
                      MV.ui.escapeHtml(
                        item.resultType
                      )
                    }

                  </span>

                </div>
              `;
            }
          )
          .join("");
    }


    if (!count) {

      html = `
        <div
          class="
            p-3
            text-secondary
          ">

          No results for
          “${MV.ui.escapeHtml(q)}”.


          <a
            href="
              ${prefix}search.html?q=${
                encodeURIComponent(q)
              }
            ">

            Open browse search

          </a>

        </div>
      `;
    }

    else {

      html += `
        <div
          class="
            p-2
            border-top
          "
          style="
            border-color:
              var(--border)!important
          ">

          <a
            class="
              btn
              btn-sm
              btn-outline-primary
              w-100
              js-search-all
            "
            href="
              ${prefix}search.html?q=${
                encodeURIComponent(q)
              }
            ">

            See all results

          </a>

        </div>
      `;
    }


    return html;
  }


  /*
   * =========================================================
   * SEARCH RESULT URL
   * =========================================================
   */


  function resultHref(
    item,
    prefix
  ) {

    const t =
      String(
        item.resultType || ""
      )
        .toLowerCase();


    if (t === "movie") {

      return (
        `${prefix}movie-details.html?id=${
          encodeURIComponent(
            item.id
          )
        }`
      );
    }


    if (t === "tvshow") {

      return (
        `${prefix}tvshow-details.html?id=${
          encodeURIComponent(
            item.id
          )
        }`
      );
    }


    return (
      `${prefix}person-details.html`
      + `?type=${
        encodeURIComponent(t)
      }`
      + `&id=${
        encodeURIComponent(
          item.id
        )
      }`
    );
  }


  /*
   * Render navbar automatically.
   */

  document.addEventListener(
    "DOMContentLoaded",
    render
  );


  return {
    render,
    refreshAvatar
  };

})();


/*
 * =========================================================
 * PROFILE AVATAR
 * =========================================================
 */


async function refreshAvatar() {

  const avatar =
    document.getElementById(
      "mvNavAvatar"
    );


  if (
    !avatar
    ||
    !MV.auth.isAuthenticated()
  ) {

    return;
  }


  const adminDepth =
    location.pathname.includes(
      "/admin/"
    );


  try {

    const profile =
      await MV.api.get(
        "profiles/me"
      );


    delete avatar.dataset
      .mvFallbackApplied;


    avatar.src =
      MV.media.profileImageUrl(
        profile.profileImageUrl,
        adminDepth
      );
  }

  catch {

    /*
     * Keep the people placeholder
     * already rendered.
     */
  }
}