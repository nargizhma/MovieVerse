window.MV = window.MV || {};
MV.admin = MV.admin || {};

MV.admin.common = (() => {
  function init() {
    if (!MV.auth.requireAdmin()) return false;

    const side =
      document.getElementById("adminSidebar");

    const top =
      document.getElementById("adminTopbar");

    const page =
      location.pathname.split("/").pop();

    if (side) {
      side.innerHTML = `
        <div class="brand">
          <a href="../index.html">
            <img src="../assets/images/logo-horizontal.png" alt="MovieVerse">
          </a>
        </div>
        <nav class="mv-admin-nav">
          ${link("dashboard.html", "fa-gauge-high", "Dashboard", page)}
          ${link("movies.html", "fa-film", "Movies", page)}
          ${link("tvshows.html", "fa-tv", "TV Shows", page)}
          ${link("actors.html", "fa-masks-theater", "Actors", page)}
          ${link("directors.html", "fa-video", "Directors", page)}
          ${link("writers.html", "fa-pen-nib", "Writers", page)}
          ${link("genres.html", "fa-tags", "Genres", page)}
          ${link("reviews.html", "fa-comments", "Reviews", page)}
          ${link("users.html", "fa-users-gear", "Users", page)}
          <hr class="border-secondary">
          <a href="../index.html">
            <i class="fa-solid fa-arrow-left"></i>Public site
          </a>
        </nav>`;
    }

    if (top) {
      const user = MV.auth.getUser();

      top.innerHTML = `
        <div class="d-flex align-items-center gap-2">
          <button class="btn btn-secondary btn-sm mv-sidebar-toggle" id="adminSidebarToggle" aria-label="Toggle admin sidebar">
            <i class="fa-solid fa-bars"></i>
          </button>
          <strong>Administration</strong>
        </div>
        <div class="d-flex align-items-center gap-2">
          <span class="badge mv-badge">${MV.ui.escapeHtml((user?.roles || []).join(", "))}</span>
          <span class="text-secondary d-none d-sm-inline">${MV.ui.escapeHtml(user?.userName || "")}</span>
        </div>`;
    }

    document
      .getElementById("adminSidebarToggle")
      ?.addEventListener(
        "click",
        () =>
          document
            .getElementById("adminSidebar")
            ?.classList.toggle("open")
      );

    document.addEventListener(
      "click",
      event => {
        const sidebar =
          document.getElementById("adminSidebar");

        if (
          innerWidth < 992 &&
          sidebar?.classList.contains("open") &&
          !sidebar.contains(event.target) &&
          !event.target.closest("#adminSidebarToggle")
        ) {
          sidebar.classList.remove("open");
        }
      }
    );

    return true;
  }

  function link(
    href,
    icon,
    label,
    page
  ) {
    const isActive =
      page === href ||
      (
        href === "movies.html" &&
        page === "movie-form.html"
      ) ||
      (
        href === "tvshows.html" &&
        [
          "tvshow-form.html",
          "episode-form.html"
        ].includes(page)
      ) ||
      (
        href === "actors.html" &&
        page === "person-form.html" &&
        new URLSearchParams(location.search)
          .get("type") === "actor"
      ) ||
      (
        href === "directors.html" &&
        page === "person-form.html" &&
        new URLSearchParams(location.search)
          .get("type") === "director"
      ) ||
      (
        href === "writers.html" &&
        page === "person-form.html" &&
        new URLSearchParams(location.search)
          .get("type") === "writer"
      );

    return `
      <a class="${isActive ? "active" : ""}" href="${href}">
        <i class="fa-solid ${icon}"></i>${label}
      </a>`;
  }

  function bindImagePreview(
    input,
    preview,
    {
      kind = "default"
    } = {}
  ) {
    if (!input || !preview) return;

    preview.dataset.mvPlaceholderKind =
      kind;

    preview.dataset.mvAdminDepth =
      "true";

    if (!preview.src) {
      preview.src =
        MV.media.placeholderUrl(
          kind,
          true
        );
    }

    input.addEventListener(
      "change",
      () => {
        const file =
          input.files?.[0];

        if (!file) return;

        // Preview is presentation only. File type/size is intentionally
        // validated by the backend FluentValidation rules.
        if (
          file.type &&
          file.type.startsWith("image/")
        ) {
          delete preview.dataset
            .mvFallbackApplied;

          preview.src =
            URL.createObjectURL(file);
        } else {
          preview.src =
            MV.media.placeholderUrl(
              kind,
              true
            );
        }
      }
    );
  }

  function addList(
    formData,
    name,
    values
  ) {
    (values || []).forEach(
      (value, index) =>
        formData.append(
          `${name}[${index}]`,
          value
        )
    );
  }

  function addCast(
    formData,
    name,
    values
  ) {
    (values || []).forEach(
      (value, index) => {
        formData.append(
          `${name}[${index}].ActorId`,
          value.actorId
        );

        formData.append(
          `${name}[${index}].CharacterName`,
          value.characterName || ""
        );

        formData.append(
          `${name}[${index}].CastOrder`,
          value.castOrder
        );
      }
    );
  }

  function appendNullable(
    formData,
    name,
    value
  ) {
    const clean =
      typeof value === "string"
        ? MV.ui.optionalText(value)
        : value;

    if (
      clean !== undefined &&
      clean !== null &&
      String(clean) !== ""
    ) {
      formData.append(
        name,
        clean
      );
    }
  }

  function dateInput(value) {
    if (!value) return "";

    const date = new Date(value);

    return Number.isNaN(date.getTime())
      ? ""
      : date
          .toISOString()
          .slice(0, 10);
  }

  class RelationshipPicker {
    constructor(
      host,
      items,
      {
        cast = false,
        placeholder = "Search…"
      } = {}
    ) {
      this.host =
        typeof host === "string"
          ? document.getElementById(host)
          : host;

      this.items =
        items || [];

      this.cast =
        cast;

      this.selected =
        [];

      this.placeholder =
        placeholder;

      this.renderBase();
    }

    renderBase() {
      this.host.innerHTML = `
        <div class="mv-picker">
          <input class="form-control js-picker-search" type="search" placeholder="${MV.ui.escapeHtml(this.placeholder)}" autocomplete="off">
          <div class="mv-picker-results d-none js-picker-results"></div>
        </div>
        <div class="mv-selected-list js-selected-list"></div>`;

      this.search =
        this.host.querySelector(
          ".js-picker-search"
        );

      this.results =
        this.host.querySelector(
          ".js-picker-results"
        );

      this.list =
        this.host.querySelector(
          ".js-selected-list"
        );

      this.search.addEventListener(
        "input",
        () => this.renderResults()
      );

      this.search.addEventListener(
        "focus",
        () => this.renderResults()
      );

      this.results.addEventListener(
        "click",
        event => {
          const option =
            event.target.closest(
              "[data-picker-id]"
            );

          if (option) {
            this.add(
              option.dataset.pickerId
            );
          }
        }
      );

      this.list.addEventListener(
        "click",
        event => {
          const button =
            event.target.closest(
              "[data-remove-id]"
            );

          if (button) {
            this.remove(
              button.dataset.removeId
            );
          }
        }
      );

      document.addEventListener(
        "click",
        event => {
          if (
            !this.host.contains(
              event.target
            )
          ) {
            this.results.classList.add(
              "d-none"
            );
          }
        }
      );
    }

    itemId(item) {
      return String(item.id);
    }

    itemName(item) {
      return (
        item.fullName ||
        item.name ||
        item.title ||
        "Unknown"
      );
    }

    renderResults() {
      const query =
        this.search.value
          .trim()
          .toLowerCase();

      const ids =
        new Set(
          this.selected.map(item =>
            String(item.id)
          )
        );

      const hits =
        this.items
          .filter(item =>
            !ids.has(
              this.itemId(item)
            ) &&
            (
              !query ||
              this
                .itemName(item)
                .toLowerCase()
                .includes(query)
            )
          )
          .slice(0, 10);

      this.results.innerHTML =
        hits.length
          ? hits
              .map(
                item =>
                  `<div class="mv-picker-option" data-picker-id="${this.itemId(item)}">${MV.ui.escapeHtml(this.itemName(item))}</div>`
              )
              .join("")
          : '<div class="p-2 text-secondary small">No matches</div>';

      this.results.classList.remove(
        "d-none"
      );
    }

    add(id, data = {}) {
      if (
        this.selected.some(
          item =>
            String(item.id) ===
            String(id)
        )
      ) {
        return;
      }

      const item =
        this.items.find(
          candidate =>
            String(candidate.id) ===
            String(id)
        );

      if (!item) return;

      this.selected.push({
        id: item.id,
        name: this.itemName(item),
        characterName:
          data.characterName || "",
        castOrder:
          data.castOrder ??
          this.selected.length
      });

      this.search.value = "";
      this.results.classList.add(
        "d-none"
      );

      this.renderSelected();
    }

    remove(id) {
      this.selected =
        this.selected.filter(
          item =>
            String(item.id) !==
            String(id)
        );

      this.renderSelected();
    }

    setSelected(values) {
      this.selected = [];

      (values || []).forEach(
        (value, index) => {
          const id =
            typeof value === "string"
              ? value
              : (
                  value.actorId ||
                  value.id
                );

          const item =
            this.items.find(
              candidate =>
                String(candidate.id) ===
                String(id)
            );

          if (item) {
            this.selected.push({
              id: item.id,
              name:
                this.itemName(item),
              characterName:
                value.characterName || "",
              castOrder:
                value.castOrder ??
                index
            });
          }
        }
      );

      this.renderSelected();
    }

    renderSelected() {
      if (!this.selected.length) {
        this.list.innerHTML =
          '<div class="text-secondary small">Nothing selected.</div>';
        return;
      }

      this.list.innerHTML =
        this.selected
          .map(item =>
            this.cast
              ? `
                <div class="mv-selected-item" data-id="${item.id}">
                  <strong>${MV.ui.escapeHtml(item.name)}</strong>
                  <input class="form-control form-control-sm js-character" value="${MV.ui.escapeHtml(item.characterName)}" placeholder="Character name" aria-label="Character name for ${MV.ui.escapeHtml(item.name)}">
                  <input class="form-control form-control-sm js-order" type="number" value="${Number(item.castOrder) || 0}" aria-label="Cast order for ${MV.ui.escapeHtml(item.name)}">
                  <button class="btn btn-sm btn-outline-danger" type="button" data-remove-id="${item.id}" aria-label="Remove ${MV.ui.escapeHtml(item.name)} from selection">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>`
              : `
                <div class="mv-selected-chip">
                  <span>${MV.ui.escapeHtml(item.name)}</span>
                  <button class="btn btn-sm btn-link text-danger p-0" type="button" data-remove-id="${item.id}" aria-label="Remove ${MV.ui.escapeHtml(item.name)}">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>`
          )
          .join("");
    }

    ids() {
      return this.selected.map(
        item => item.id
      );
    }

    castValues() {
      if (!this.cast) return [];

      return this.selected.map(
        item => {
          const row =
            this.list.querySelector(
              `[data-id="${CSS.escape(String(item.id))}"]`
            );

          const orderValue =
            row
              ?.querySelector(".js-order")
              ?.value ?? "";

          return {
            actorId: item.id,
            characterName:
              row
                ?.querySelector(".js-character")
                ?.value
                .trim() || null,
            castOrder:
              orderValue === ""
                ? 0
                : Number(orderValue)
          };
        }
      );
    }
  }

  document.addEventListener(
    "DOMContentLoaded",
    init
  );

  return {
    init,
    bindImagePreview,
    addList,
    addCast,
    appendNullable,
    dateInput,
    RelationshipPicker
  };
})();
