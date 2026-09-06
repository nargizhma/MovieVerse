window.MV = window.MV || {};

MV.ui = (() => {
  const $id = id => document.getElementById(id);
  const qs = (selector, root = document) =>
    root.querySelector(selector);
  const qsa = (selector, root = document) =>
    [...root.querySelectorAll(selector)];

  const escapeHtml = value =>
    String(value ?? "").replace(
      /[&<>'"]/g,
      ch =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          "'": "&#39;",
          '"': "&quot;"
        })[ch]
    );

  const debounce = (fn, wait = 300) => {
    let timer;

    return (...args) => {
      clearTimeout(timer);
      timer = setTimeout(
        () => fn(...args),
        wait
      );
    };
  };

  function optionalText(value) {
    if (
      value === null ||
      value === undefined
    ) {
      return "";
    }

    const text = String(value).trim();

    if (!text) return "";

    const normalized = text.toLowerCase();

    if (
      ["string", "null", "undefined"]
        .includes(normalized)
    ) {
      return "";
    }

    return text;
  }

  function hasOptionalText(value) {
    return optionalText(value) !== "";
  }

  function ensureGlobalUi() {
    if (!$id("mvToastContainer")) {
      document.body.insertAdjacentHTML(
        "beforeend",
        '<div id="mvToastContainer" class="toast-container position-fixed bottom-0 end-0 p-3 mv-toast-container"></div>'
      );
    }

    if (!$id("mvConfirmModal")) {
      document.body.insertAdjacentHTML(
        "beforeend",
        `
        <div class="modal fade" id="mvConfirmModal" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="mvConfirmTitle">Confirm action</h5>
                <button class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body" id="mvConfirmBody"></div>
              <div class="modal-footer">
                <button class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                <button class="btn btn-danger" id="mvConfirmOk">Confirm</button>
              </div>
            </div>
          </div>
        </div>`
      );
    }
  }

  function toast(message, type = "info") {
    ensureGlobalUi();

    const icons = {
      success: "fa-circle-check",
      danger: "fa-circle-exclamation",
      warning: "fa-triangle-exclamation",
      info: "fa-circle-info"
    };

    const id =
      `toast-${Date.now()}-` +
      Math.random().toString(36).slice(2);

    $id("mvToastContainer").insertAdjacentHTML(
      "beforeend",
      `
      <div id="${id}" class="toast" role="status" aria-live="polite" aria-atomic="true">
        <div class="toast-header bg-transparent text-light border-0">
          <i class="fa-solid ${icons[type] || icons.info} me-2"></i>
          <strong class="me-auto">MovieVerse</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body pt-0">${escapeHtml(message)}</div>
      </div>`
    );

    const element = $id(id);
    const instance =
      bootstrap.Toast.getOrCreateInstance(
        element,
        { delay: 5000 }
      );

    element.addEventListener(
      "hidden.bs.toast",
      () => element.remove(),
      { once: true }
    );

    instance.show();
  }

  function errorMessages(
    error,
    fallback = "Could not complete the request."
  ) {
    if (
      Array.isArray(error?.messages) &&
      error.messages.length
    ) {
      return [...new Set(
        error.messages
          .map(message =>
            String(message ?? "").trim()
          )
          .filter(Boolean)
      )];
    }

    if (MV.api?.extractErrorMessages) {
      const fromData =
        MV.api.extractErrorMessages(
          error?.data,
          ""
        ).filter(Boolean);

      if (fromData.length) return fromData;
    }

    const message =
      String(
        error?.detail ||
        error?.message ||
        fallback
      ).trim();

    return [message || fallback];
  }

  function showError(
    error,
    fallback = "Could not complete the request."
  ) {
    const messages =
      errorMessages(error, fallback);

    toast(
      messages.join(" • "),
      "danger"
    );
  }

  function validationEntries(error) {
    const errors = error?.data?.errors;

    if (
      !errors ||
      typeof errors !== "object" ||
      Array.isArray(errors)
    ) {
      return [];
    }

    return Object.entries(errors)
      .map(([key, value]) => {
        const messages = Array.isArray(value)
          ? value
          : [value];

        return {
          key,
          messages: messages
            .map(message =>
              String(message ?? "").trim()
            )
            .filter(Boolean)
        };
      })
      .filter(entry =>
        entry.messages.length
      );
  }

  function findFieldForError(form, key) {
    if (!form || !key) return null;

    const raw =
      String(key)
        .replace(/^\$\./, "")
        .replace(/\[(\d+)\]/g, "")
        .split(".")
        .filter(Boolean)
        .pop();

    if (!raw) return null;

    return [...form.elements]
      .find(element =>
        element.name &&
        element.name.toLowerCase() ===
          raw.toLowerCase()
      ) || null;
  }

  function clearFormError(form) {
    if (!form) return;

    form
      .querySelectorAll("[data-mv-form-error]")
      .forEach(element => element.remove());

    form
      .querySelectorAll("[data-mv-server-invalid='true']")
      .forEach(element => {
        element.classList.remove(
          "is-invalid"
        );
        delete element.dataset
          .mvServerInvalid;
      });
  }

  function buildErrorAlert(
    error,
    heading,
    compact = false
  ) {
    const messages = errorMessages(error);
    const content =
      messages.length === 1
        ? `<div class="${heading ? "mt-1" : ""}">${escapeHtml(messages[0])}</div>`
        : `
          <ul class="mb-0 ${heading ? "mt-2" : ""} ps-3">
            ${messages
              .map(
                message =>
                  `<li>${escapeHtml(message)}</li>`
              )
              .join("")}
          </ul>`;

    return `
      <div class="alert alert-danger ${compact ? "py-2" : ""}" role="alert">
        ${
          heading
            ? `<div class="fw-semibold"><i class="fa-solid fa-circle-exclamation me-2"></i>${escapeHtml(heading)}</div>`
            : ""
        }
        ${content}
      </div>`;
  }

  function showFormError(
    form,
    error,
    heading = "Could not save changes."
  ) {
    if (!form) {
      showError(error);
      return;
    }

    clearFormError(form);

    const wrapper =
      document.createElement("div");

    wrapper.dataset.mvFormError = "";
    wrapper.innerHTML =
      buildErrorAlert(
        error,
        heading
      );

    form.prepend(wrapper);

    validationEntries(error)
      .forEach(entry => {
        const field =
          findFieldForError(
            form,
            entry.key
          );

        if (!field) return;

        field.classList.add(
          "is-invalid"
        );

        field.dataset
          .mvServerInvalid = "true";
      });

    if (!form.dataset.mvErrorResetBound) {
      form.dataset.mvErrorResetBound =
        "true";

      form.addEventListener(
        "input",
        event => {
          const field = event.target;

          if (
            field instanceof HTMLElement &&
            field.dataset
              .mvServerInvalid === "true"
          ) {
            field.classList.remove(
              "is-invalid"
            );
            delete field.dataset
              .mvServerInvalid;
          }
        }
      );
    }

    wrapper.scrollIntoView({
      behavior: "smooth",
      block: "center"
    });
  }

  function showInlineError(
    host,
    error,
    heading = ""
  ) {
    if (!host) {
      showError(error);
      return;
    }

    host.innerHTML =
      buildErrorAlert(
        error,
        heading,
        true
      );
  }

  function clearInlineError(host) {
    if (host) host.innerHTML = "";
  }

  function useBackendValidation(form) {
    if (!form) return form;

    // Browser/jQuery rules must not stop the request before the API
    // gets a chance to run the FluentValidation validators.
    form.noValidate = true;
    form.setAttribute(
      "novalidate",
      "novalidate"
    );

    try {
      const validator =
        window.jQuery &&
        jQuery(form).data("validator");

      validator?.destroy?.();
    } catch {
      // The page does not need jQuery Validation for backend validation.
    }

    return form;
  }

  function confirm({
    title = "Confirm action",
    message = "Are you sure?",
    confirmText = "Confirm",
    danger = true
  } = {}) {
    ensureGlobalUi();

    return new Promise(resolve => {
      const modalElement =
        $id("mvConfirmModal");

      const modal =
        bootstrap.Modal.getOrCreateInstance(
          modalElement
        );

      $id("mvConfirmTitle").textContent =
        title;

      $id("mvConfirmBody").innerHTML =
        `<p class="mb-0">${escapeHtml(message)}</p>`;

      const ok =
        $id("mvConfirmOk");

      ok.textContent =
        confirmText;

      ok.className =
        `btn ${danger
          ? "btn-danger"
          : "btn-primary"}`;

      let settled = false;

      const yes = () => {
        settled = true;
        ok.removeEventListener(
          "click",
          yes
        );
        modal.hide();
        resolve(true);
      };

      ok.addEventListener(
        "click",
        yes
      );

      modalElement.addEventListener(
        "hidden.bs.modal",
        () => {
          ok.removeEventListener(
            "click",
            yes
          );

          if (!settled) resolve(false);
        },
        { once: true }
      );

      modal.show();
    });
  }

  function buttonBusy(
    button,
    busy,
    busyText = "Working…"
  ) {
    if (!button) return;

    if (busy) {
      button.dataset.originalHtml =
        button.innerHTML;

      button.disabled = true;

      button.innerHTML =
        `<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>${escapeHtml(busyText)}`;
    } else {
      button.disabled = false;

      if (
        button.dataset.originalHtml
      ) {
        button.innerHTML =
          button.dataset.originalHtml;

        delete button.dataset
          .originalHtml;
      }
    }
  }

  function emptyState({
    icon = "fa-film",
    title = "Nothing here yet",
    text = "",
    actionText = "",
    actionHref = ""
  } = {}) {
    return `
      <div class="mv-empty">
        <i class="fa-solid ${icon}"></i>
        <h4>${escapeHtml(title)}</h4>
        ${text ? `<p>${escapeHtml(text)}</p>` : ""}
        ${
          actionText && actionHref
            ? `<a class="btn btn-outline-primary" href="${escapeHtml(actionHref)}">${escapeHtml(actionText)}</a>`
            : ""
        }
      </div>`;
  }

  function skeletonCards(count = 6) {
    return Array.from(
      { length: count },
      () =>
        '<div class="mv-skeleton mv-skeleton-card"></div>'
    ).join("");
  }

  function skeletonLines(count = 5) {
    return Array.from(
      { length: count },
      (_, index) =>
        `<div class="mv-skeleton mv-skeleton-line" style="width:${88 - index * 7}%"></div>`
    ).join("");
  }

  function setFlash(
    message,
    type = "success"
  ) {
    sessionStorage.setItem(
      MV.config.FLASH_KEY,
      JSON.stringify({
        message,
        type
      })
    );
  }

  function consumeFlash() {
    const raw =
      sessionStorage.getItem(
        MV.config.FLASH_KEY
      );

    if (!raw) return;

    sessionStorage.removeItem(
      MV.config.FLASH_KEY
    );

    try {
      const flash = JSON.parse(raw);
      toast(
        flash.message,
        flash.type
      );
    } catch {
      // Ignore a malformed stale flash value.
    }
  }

  function getParam(name) {
    return new URLSearchParams(
      location.search
    ).get(name);
  }

  function requireParam(
    name,
    redirect = "index.html"
  ) {
    const value = getParam(name);

    if (!value) {
      location.replace(redirect);
      return null;
    }

    return value;
  }

  function setText(
    id,
    value,
    fallback = ""
  ) {
    const element = $id(id);

    if (element) {
      element.textContent =
        value ?? fallback;
    }
  }

  function visible(
    element,
    show = true
  ) {
    if (element) {
      element.classList.toggle(
        "d-none",
        !show
      );
    }
  }

  function safeHref(url) {
    try {
      const parsed =
        new URL(
          url,
          location.href
        );

      return [
        "http:",
        "https:"
      ].includes(parsed.protocol)
        ? parsed.href
        : "#";
    } catch {
      return "#";
    }
  }

  document.addEventListener(
    "DOMContentLoaded",
    () => {
      ensureGlobalUi();
      consumeFlash();
    }
  );

  return {
    $id,
    qs,
    qsa,
    escapeHtml,
    debounce,
    optionalText,
    hasOptionalText,
    toast,
    errorMessages,
    showError,
    showFormError,
    clearFormError,
    showInlineError,
    clearInlineError,
    useBackendValidation,
    confirm,
    buttonBusy,
    emptyState,
    skeletonCards,
    skeletonLines,
    setFlash,
    consumeFlash,
    getParam,
    requireParam,
    setText,
    visible,
    safeHref
  };
})();
