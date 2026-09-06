window.MV = window.MV || {};

class MVApiError extends Error {
  constructor({ status = 0, title = "Request failed", detail = "", data = null, url = "" } = {}) {
    super(detail || title || "Request failed");
    this.name = "MVApiError";
    this.status = status;
    this.title = title;
    this.detail = detail || title;
    this.data = data;
    this.url = url;
  }
}

MV.ApiError = MVApiError;

MV.api = (() => {
  const base = MV.config.API_BASE_URL.replace(/\/$/, "");

  function buildUrl(path, query) {
    const clean = String(path || "").replace(/^\//, "");
    const url = new URL(`${base}/${clean}`);
    if (query) {
      Object.entries(query).forEach(([key, value]) => {
        if (value === undefined || value === null || value === "") return;
        if (Array.isArray(value)) value.forEach(v => url.searchParams.append(key, v));
        else url.searchParams.set(key, value);
      });
    }
    return url.toString();
  }

  async function request(method, path, options = {}) {
    const url = buildUrl(path, options.query);
    const headers = new Headers(options.headers || {});
    const token = MV.auth?.getToken?.() || localStorage.getItem(MV.config.TOKEN_KEY);
    if (token && options.auth !== false) headers.set("Authorization", `Bearer ${token}`);

    let body = options.body;
    if (body !== undefined && body !== null && !(body instanceof FormData) && !(body instanceof Blob) && typeof body !== "string") {
      headers.set("Content-Type", "application/json");
      body = JSON.stringify(body);
    }

    let response;
    try {
      response = await fetch(url, { method, headers, body, signal: options.signal, cache: options.cache || "no-store" });
    } catch (error) {
      if (error?.name === "AbortError") throw error;
      throw new MVApiError({ status: 0, title: "Network error", detail: `Could not reach the MovieVerse API. Make sure the backend is running on ${MV.config.BACKEND_ORIGIN}.`, url });
    }

    if (response.status === 204) return null;

    const contentType = response.headers.get("content-type") || "";
    let data = null;
    if (contentType.includes("application/json")) {
      try { data = await response.json(); } catch { data = null; }
    } else {
      const text = await response.text();
      data = text || null;
    }

    if (!response.ok) {
      const fallback = {
        400: "The request was not valid.",
        401: "Authentication is required.",
        403: "You do not have permission to perform this action.",
        404: "The requested item was not found.",
        409: "This action conflicts with existing data.",
        500: "The server could not complete the request."
      }[response.status] || `Request failed with status ${response.status}.`;
      const error = new MVApiError({
        status: response.status,
        title: data?.title || response.statusText || "Request failed",
        detail: data?.detail || (typeof data === "string" ? data : "") || fallback,
        data,
        url
      });
      if (response.status === 401 && options.handle401 !== false && token && MV.auth?.clearToken) MV.auth.clearToken();
      throw error;
    }
    return data;
  }

  return {
    buildUrl,
    request,
    get: (path, query, options = {}) => request("GET", path, { ...options, query }),
    post: (path, body, options = {}) => request("POST", path, { ...options, body }),
    put: (path, body, options = {}) => request("PUT", path, { ...options, body }),
    delete: (path, options = {}) => request("DELETE", path, options)
  };
})();
