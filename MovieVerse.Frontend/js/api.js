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

        if (Array.isArray(value)) {
          value.forEach(item => url.searchParams.append(key, item));
        } else {
          url.searchParams.set(key, value);
        }
      });
    }

    return url.toString();
  }

  async function readResponseBody(response) {
    const contentType = (response.headers.get("content-type") || "").toLowerCase();
    const isJson = contentType.includes("application/json") || contentType.includes("+json");

    if (isJson) {
      try {
        return await response.json();
      } catch {
        return null;
      }
    }

    const text = await response.text();
    if (!text) return null;

    // Some APIs/proxies return JSON with the wrong content type.
    try {
      return JSON.parse(text);
    } catch {
      return text;
    }
  }

  function validationMessages(errors) {
    if (!errors || typeof errors !== "object") return [];

    const messages = [];

    Object.values(errors).forEach(value => {
      const values = Array.isArray(value) ? value : [value];

      values.forEach(message => {
        if (typeof message === "string" && message.trim()) {
          messages.push(message.trim());
        }
      });
    });

    return [...new Set(messages)];
  }

  function getErrorDetail(data, fallback) {
    if (!data) return fallback;

    if (typeof data === "string") {
      return data.trim() || fallback;
    }

    // ASP.NET ValidationProblemDetails stores the useful validation messages
    // in the "errors" object rather than in "detail".
    const validation = validationMessages(data.errors);
    if (validation.length) {
      return validation.join(" ");
    }

    if (typeof data.detail === "string" && data.detail.trim()) {
      return data.detail.trim();
    }

    if (typeof data.message === "string" && data.message.trim()) {
      return data.message.trim();
    }

    const genericTitles = new Set([
      "bad request",
      "unauthorized",
      "forbidden",
      "not found",
      "conflict",
      "internal server error",
      "one or more validation errors occurred."
    ]);

    if (
      typeof data.title === "string" &&
      data.title.trim() &&
      !genericTitles.has(data.title.trim().toLowerCase())
    ) {
      return data.title.trim();
    }

    return fallback;
  }

  async function request(method, path, options = {}) {
    const url = buildUrl(path, options.query);
    const headers = new Headers(options.headers || {});
    const token = MV.auth?.getToken?.() || localStorage.getItem(MV.config.TOKEN_KEY);

    if (token && options.auth !== false) {
      headers.set("Authorization", `Bearer ${token}`);
    }

    let body = options.body;

    if (
      body !== undefined &&
      body !== null &&
      !(body instanceof FormData) &&
      !(body instanceof Blob) &&
      typeof body !== "string"
    ) {
      headers.set("Content-Type", "application/json");
      body = JSON.stringify(body);
    }

    let response;

    try {
      response = await fetch(url, {
        method,
        headers,
        body,
        signal: options.signal,
        cache: options.cache || "no-store"
      });
    } catch (error) {
      if (error?.name === "AbortError") throw error;

      throw new MVApiError({
        status: 0,
        title: "Network error",
        detail: `Could not reach the MovieVerse API. Make sure the backend is running on ${MV.config.BACKEND_ORIGIN}.`,
        url
      });
    }

    if (response.status === 204) return null;

    const data = await readResponseBody(response);

    if (!response.ok) {
      const fallback = {
        400: "The request was not valid. Check the entered information and try again.",
        401: "Authentication is required.",
        403: "You do not have permission to perform this action.",
        404: "The requested item was not found.",
        409: "This action conflicts with existing data.",
        500: "The server could not complete the request."
      }[response.status] || `Request failed with status ${response.status}.`;

      const error = new MVApiError({
        status: response.status,
        title: data?.title || response.statusText || "Request failed",
        detail: getErrorDetail(data, fallback),
        data,
        url
      });

      if (
        response.status === 401 &&
        options.handle401 !== false &&
        token &&
        MV.auth?.clearToken
      ) {
        MV.auth.clearToken();
      }

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
