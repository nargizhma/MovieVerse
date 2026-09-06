window.MV = window.MV || {};

MV.ui = (() => {
  const $id = id => document.getElementById(id);
  const qs = (selector, root = document) => root.querySelector(selector);
  const qsa = (selector, root = document) => [...root.querySelectorAll(selector)];
  const escapeHtml = value => String(value ?? "").replace(/[&<>'"]/g, ch => ({
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    "'": "&#39;",
    '"': "&quot;"
  }[ch]));
  const debounce = (fn, wait = 300) => {
    let t;
    return (...args) => {
      clearTimeout(t);
      t = setTimeout(() => fn(...args), wait);
    };
  };

  function optionalText(value) {
    if (value === null || value === undefined) return "";

    const text = String(value).trim();
    if (!text) return "";

    // Swagger/OpenAPI examples often use the literal value "string".
    // Treat those placeholder-like values as missing only when a page asks
    // for optional content through this helper.
    const normalized = text.toLowerCase();
    if (["string", "null", "undefined"].includes(normalized)) return "";

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
      document.body.insertAdjacentHTML("beforeend", `
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
        </div>`);
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

    const id = `toast-${Date.now()}-${Math.random().toString(36).slice(2)}`;

    $id("mvToastContainer").insertAdjacentHTML("beforeend", `
      <div id="${id}" class="toast" role="status" aria-live="polite" aria-atomic="true">
        <div class="toast-header bg-transparent text-light border-0">
          <i class="fa-solid ${icons[type] || icons.info} me-2"></i>
          <strong class="me-auto">MovieVerse</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body pt-0">${escapeHtml(message)}</div>
      </div>`);

    const el = $id(id);
    const instance = bootstrap.Toast.getOrCreateInstance(el, { delay: 3400 });

    el.addEventListener("hidden.bs.toast", () => el.remove(), { once: true });
    instance.show();
  }

  function showError(error, fallback = "Could not complete the request.") {
    const message = error?.detail || error?.message || fallback;
    toast(message, "danger");
  }

  function errorMessages(error, fallback = "Could not complete the request.") {
    const validationErrors = error?.data?.errors;
    const validationMessages = [];

    if (validationErrors && typeof validationErrors === "object") {
      Object.values(validationErrors).forEach(value => {
        const values = Array.isArray(value) ? value : [value];

        values.forEach(message => {
          if (typeof message === "string" && message.trim()) {
            validationMessages.push(message.trim());
          }
        });
      });
    }

    if (validationMessages.length) {
      return [...new Set(validationMessages)];
    }

    if (typeof error?.detail === "string" && error.detail.trim()) {
      return [error.detail.trim()];
    }

    if (typeof error?.message === "string" && error.message.trim()) {
      return [error.message.trim()];
    }

    return [fallback];
  }

  function clearServerFieldErrors(form) {
    if (!form) return;

    form.querySelectorAll("[data-mv-server-invalid]").forEach(element => {
      element.classList.remove("is-invalid");
      delete element.dataset.mvServerInvalid;
    });
  }

  function markServerFields(form, error) {
    const errors = error?.data?.errors;
    if (!form || !errors || typeof errors !== "object") return;

    Object.keys(errors).forEach(key => {
      // Handles keys such as "Title", "dto.Title" and "Actors[0].ActorId".
      const cleanKey = String(key).replace(/\[\d+\]/g, "");
      const fieldName = cleanKey.split(".").pop();
      if (!fieldName) return;

      const field = form.elements.namedItem(fieldName);
      if (!field || field instanceof RadioNodeList) return;

      field.classList?.add("is-invalid");
      if (field.dataset) field.dataset.mvServerInvalid = "true";
    });
  }

  function clearFormError(form) {
    if (!form) return;

    form.querySelector("[data-mv-form-error]")?.remove();
    clearServerFieldErrors(form);
  }

  function showFormError(form, error, heading = "Could not save changes.") {
    if (!form) {
      showError(error);
      return;
    }

    clearFormError(form);
    markServerFields(form, error);

    const messages = errorMessages(error);
    const box = document.createElement("div");

    box.className = "alert alert-danger mb-4";
    box.dataset.mvFormError = "true";
    box.setAttribute("role", "alert");

    const details = messages.length === 1
      ? `<div class="mt-1">${escapeHtml(messages[0])}</div>`
      : `<ul class="mb-0 mt-2 ps-3">${messages.map(message => `<li>${escapeHtml(message)}</li>`).join("")}</ul>`;

    box.innerHTML = `
      <div class="d-flex gap-3 align-items-start">
        <i class="fa-solid fa-circle-exclamation mt-1" aria-hidden="true"></i>
        <div>
          <strong>${escapeHtml(heading)}</strong>
          ${details}
          <div class="small mt-2">
            Your entered data has not been cleared. Fix the problem and press Save again.
          </div>
        </div>
      </div>`;

    form.prepend(box);

    box.scrollIntoView({
      behavior: "smooth",
      block: "center"
    });
  }

  function confirm({ title = "Confirm action", message = "Are you sure?", confirmText = "Confirm", danger = true } = {}) {
    ensureGlobalUi();

    return new Promise(resolve => {
      const modalEl = $id("mvConfirmModal");
      const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

      $id("mvConfirmTitle").textContent = title;
      $id("mvConfirmBody").innerHTML = `<p class="mb-0">${escapeHtml(message)}</p>`;

      const ok = $id("mvConfirmOk");
      ok.textContent = confirmText;
      ok.className = `btn ${danger ? "btn-danger" : "btn-primary"}`;

      let settled = false;

      const yes = () => {
        settled = true;
        ok.removeEventListener("click", yes);
        modal.hide();
        resolve(true);
      };

      ok.addEventListener("click", yes);
      modalEl.addEventListener("hidden.bs.modal", () => {
        ok.removeEventListener("click", yes);
        if (!settled) resolve(false);
      }, { once: true });

      modal.show();
    });
  }

  function buttonBusy(button, busy, busyText = "Working…") {
    if (!button) return;

    if (busy) {
      button.dataset.originalHtml = button.innerHTML;
      button.disabled = true;
      button.innerHTML = `<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>${escapeHtml(busyText)}`;
    } else {
      button.disabled = false;

      if (button.dataset.originalHtml) {
        button.innerHTML = button.dataset.originalHtml;
        delete button.dataset.originalHtml;
      }
    }
  }

  function emptyState({ icon = "fa-film", title = "Nothing here yet", text = "", actionText = "", actionHref = "" } = {}) {
    return `<div class="mv-empty"><i class="fa-solid ${icon}"></i><h4>${escapeHtml(title)}</h4>${text ? `<p>${escapeHtml(text)}</p>` : ""}${actionText && actionHref ? `<a class="btn btn-outline-primary" href="${escapeHtml(actionHref)}">${escapeHtml(actionText)}</a>` : ""}</div>`;
  }

  function skeletonCards(count = 6) {
    return Array.from({ length: count }, () => '<div class="mv-skeleton mv-skeleton-card"></div>').join("");
  }

  function skeletonLines(count = 5) {
    return Array.from(
      { length: count },
      (_, i) => `<div class="mv-skeleton mv-skeleton-line" style="width:${88 - i * 7}%"></div>`
    ).join("");
  }

  function setFlash(message, type = "success") {
    sessionStorage.setItem(MV.config.FLASH_KEY, JSON.stringify({ message, type }));
  }

  function consumeFlash() {
    const raw = sessionStorage.getItem(MV.config.FLASH_KEY);
    if (!raw) return;

    sessionStorage.removeItem(MV.config.FLASH_KEY);

    try {
      const flash = JSON.parse(raw);
      toast(flash.message, flash.type);
    } catch {
      // Ignore malformed flash data.
    }
  }

  function getParam(name) {
    return new URLSearchParams(location.search).get(name);
  }

  function requireParam(name, redirect = "index.html") {
    const value = getParam(name);

    if (!value) {
      location.replace(redirect);
      return null;
    }

    return value;
  }

  function setText(id, value, fallback = "") {
    const el = $id(id);
    if (el) el.textContent = value ?? fallback;
  }

  function visible(el, show = true) {
    if (el) el.classList.toggle("d-none", !show);
  }

  function safeHref(url) {
    try {
      const parsed = new URL(url, location.href);
      return ["http:", "https:"].includes(parsed.protocol) ? parsed.href : "#";
    } catch {
      return "#";
    }
  }

  document.addEventListener("DOMContentLoaded", () => {
    ensureGlobalUi();
    consumeFlash();
  });

  return {
    $id,
    qs,
    qsa,
    escapeHtml,
    debounce,
    optionalText,
    hasOptionalText,
    toast,
    showError,
    showFormError,
    clearFormError,
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
