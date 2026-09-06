window.MV = window.MV || {};

class MVApiError extends Error {
  constructor({
    status = 0,
    title = "Request failed",
    detail = "",
    messages = [],
    data = null,
    url = ""
  } = {}) {
    const cleanMessages = [...new Set(
      (messages || [])
        .map(message => String(message ?? "").trim())
        .filter(Boolean)
    )];

    const finalDetail =
      String(detail ?? "").trim() ||
      cleanMessages[0] ||
      String(title ?? "").trim() ||
      "Request failed";

    super(finalDetail);

    this.name = "MVApiError";
    this.status = status;
    this.title = title || "Request failed";
    this.detail = finalDetail;
    this.messages = cleanMessages.length ? cleanMessages : [finalDetail];
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
        if (
          value === undefined ||
          value === null ||
          value === ""
        ) {
          return;
        }

        if (Array.isArray(value)) {
          value.forEach(item =>
            url.searchParams.append(key, item)
          );
        } else {
          url.searchParams.set(key, value);
        }
      });
    }

    return url.toString();
  }

  function uniqueMessages(messages) {
    return [...new Set(
      messages
        .map(message => String(message ?? "").trim())
        .filter(Boolean)
    )];
  }

  function collectMessages(value, messages) {
    if (value === null || value === undefined) return;

    if (typeof value === "string") {
      if (value.trim()) messages.push(value.trim());
      return;
    }

    if (Array.isArray(value)) {
      value.forEach(item => collectMessages(item, messages));
      return;
    }

    if (typeof value !== "object") return;

    if (typeof value.description === "string") {
      collectMessages(value.description, messages);
    }

    if (typeof value.message === "string") {
      collectMessages(value.message, messages);
    }

    if (typeof value.detail === "string") {
      collectMessages(value.detail, messages);
    }

    if (value.errors !== undefined) {
      if (
        value.errors &&
        typeof value.errors === "object" &&
        !Array.isArray(value.errors)
      ) {
        Object.values(value.errors).forEach(errorValue =>
          collectMessages(errorValue, messages)
        );
      } else {
        collectMessages(value.errors, messages);
      }
    }
  }

  function extractErrorMessages(data, fallback = "Request failed.") {
    const messages = [];

    collectMessages(data, messages);

    const specific = uniqueMessages(messages);

    // ASP.NET ValidationProblemDetails often gives a generic title plus
    // useful messages in "errors". Prefer the useful messages.
    if (specific.length) return specific;

    if (
      data &&
      typeof data === "object" &&
      typeof data.title === "string" &&
      data.title.trim() &&
      data.title.trim() !== "One or more validation errors occurred."
    ) {
      return [data.title.trim()];
    }

    if (typeof data === "string" && data.trim()) {
      return [data.trim()];
    }

    return [fallback];
  }

  async function readResponseBody(response) {
    const contentType =
      response.headers.get("content-type") || "";

    // Handles both application/json and application/problem+json.
    if (contentType.toLowerCase().includes("json")) {
      try {
        return await response.json();
      } catch {
        return null;
      }
    }

    try {
      const text = await response.text();
      if (!text) return null;

      // Some servers/proxies return JSON with a wrong/missing content type.
      try {
        return JSON.parse(text);
      } catch {
        return text;
      }
    } catch {
      return null;
    }
  }

  async function request(method, path, options = {}) {
    const url = buildUrl(path, options.query);
    const headers = new Headers(options.headers || {});
    const token =
      MV.auth?.getToken?.() ||
      localStorage.getItem(MV.config.TOKEN_KEY);

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
        detail:
          `Could not reach the MovieVerse API. ` +
          `Make sure the backend is running on ${MV.config.BACKEND_ORIGIN}.`,
        url
      });
    }

    if (response.status === 204) return null;

    const data = await readResponseBody(response);

    if (!response.ok) {
      const fallback = {
        400: "The submitted data is invalid.",
        401: "Authentication is required.",
        403: "You do not have permission to perform this action.",
        404: "The requested item was not found.",
        409: "This action conflicts with existing data.",
        500: "The server could not complete the request."
      }[response.status] ||
        `Request failed with status ${response.status}.`;

      const messages = extractErrorMessages(data, fallback);

      const error = new MVApiError({
        status: response.status,
        title:
          (data &&
            typeof data === "object" &&
            typeof data.title === "string" &&
            data.title) ||
          response.statusText ||
          "Request failed",
        detail: messages[0] || fallback,
        messages,
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
    extractErrorMessages,
    request,
    get: (path, query, options = {}) =>
      request("GET", path, { ...options, query }),
    post: (path, body, options = {}) =>
      request("POST", path, { ...options, body }),
    put: (path, body, options = {}) =>
      request("PUT", path, { ...options, body }),
    delete: (path, options = {}) =>
      request("DELETE", path, options)
  };
})();
