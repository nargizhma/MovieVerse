window.MV = window.MV || {};

MV.ui = (() => {
  const $id = id => document.getElementById(id);
  const qs = (selector, root = document) => root.querySelector(selector);
  const qsa = (selector, root = document) => [...root.querySelectorAll(selector)];
  const escapeHtml = value => String(value ?? "").replace(/[&<>'"]/g, ch => ({ "&":"&amp;", "<":"&lt;", ">":"&gt;", "'":"&#39;", '"':"&quot;" }[ch]));
  const debounce = (fn, wait = 300) => { let t; return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), wait); }; };

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
      document.body.insertAdjacentHTML("beforeend", '<div id="mvToastContainer" class="toast-container position-fixed bottom-0 end-0 p-3 mv-toast-container"></div>');
    }
    if (!$id("mvConfirmModal")) {
      document.body.insertAdjacentHTML("beforeend", `
        <div class="modal fade" id="mvConfirmModal" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-dialog-centered"><div class="modal-content">
            <div class="modal-header"><h5 class="modal-title" id="mvConfirmTitle">Confirm action</h5><button class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
            <div class="modal-body" id="mvConfirmBody"></div>
            <div class="modal-footer"><button class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button><button class="btn btn-danger" id="mvConfirmOk">Confirm</button></div>
          </div></div>
        </div>`);
    }
  }

  function toast(message, type = "info") {
    ensureGlobalUi();
    const icons = { success:"fa-circle-check", danger:"fa-circle-exclamation", warning:"fa-triangle-exclamation", info:"fa-circle-info" };
    const id = `toast-${Date.now()}-${Math.random().toString(36).slice(2)}`;
    $id("mvToastContainer").insertAdjacentHTML("beforeend", `
      <div id="${id}" class="toast" role="status" aria-live="polite" aria-atomic="true">
        <div class="toast-header bg-transparent text-light border-0">
          <i class="fa-solid ${icons[type] || icons.info} me-2"></i><strong class="me-auto">MovieVerse</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div><div class="toast-body pt-0">${escapeHtml(message)}</div>
      </div>`);
    const el = $id(id); const instance = bootstrap.Toast.getOrCreateInstance(el, { delay: 3400 });
    el.addEventListener("hidden.bs.toast", () => el.remove(), { once:true }); instance.show();
  }

  function showError(error, fallback = "Could not complete the request.") {
    const message = error?.detail || error?.message || fallback;
    toast(message, "danger");
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
      const yes = () => { settled = true; ok.removeEventListener("click", yes); modal.hide(); resolve(true); };
      ok.addEventListener("click", yes);
      modalEl.addEventListener("hidden.bs.modal", () => { ok.removeEventListener("click", yes); if (!settled) resolve(false); }, { once:true });
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
      if (button.dataset.originalHtml) { button.innerHTML = button.dataset.originalHtml; delete button.dataset.originalHtml; }
    }
  }

  function emptyState({ icon = "fa-film", title = "Nothing here yet", text = "", actionText = "", actionHref = "" } = {}) {
    return `<div class="mv-empty"><i class="fa-solid ${icon}"></i><h4>${escapeHtml(title)}</h4>${text ? `<p>${escapeHtml(text)}</p>` : ""}${actionText && actionHref ? `<a class="btn btn-outline-primary" href="${escapeHtml(actionHref)}">${escapeHtml(actionText)}</a>` : ""}</div>`;
  }

  function skeletonCards(count = 6) { return Array.from({ length:count }, () => '<div class="mv-skeleton mv-skeleton-card"></div>').join(""); }
  function skeletonLines(count = 5) { return Array.from({ length:count }, (_,i) => `<div class="mv-skeleton mv-skeleton-line" style="width:${88 - i*7}%"></div>`).join(""); }

  function setFlash(message, type = "success") { sessionStorage.setItem(MV.config.FLASH_KEY, JSON.stringify({ message, type })); }
  function consumeFlash() {
    const raw = sessionStorage.getItem(MV.config.FLASH_KEY); if (!raw) return;
    sessionStorage.removeItem(MV.config.FLASH_KEY);
    try { const f = JSON.parse(raw); toast(f.message, f.type); } catch { }
  }

  function getParam(name) { return new URLSearchParams(location.search).get(name); }
  function requireParam(name, redirect = "index.html") { const v = getParam(name); if (!v) { location.replace(redirect); return null; } return v; }
  function setText(id, value, fallback = "") { const el = $id(id); if (el) el.textContent = value ?? fallback; }
  function visible(el, show = true) { if (el) el.classList.toggle("d-none", !show); }
  function safeHref(url) { try { const u = new URL(url, location.href); return ["http:","https:"].includes(u.protocol) ? u.href : "#"; } catch { return "#"; } }

  document.addEventListener("DOMContentLoaded", () => { ensureGlobalUi(); consumeFlash(); });
  return { $id, qs, qsa, escapeHtml, debounce, optionalText, hasOptionalText, toast, showError, confirm, buttonBusy, emptyState, skeletonCards, skeletonLines, setFlash, consumeFlash, getParam, requireParam, setText, visible, safeHref };
})();
