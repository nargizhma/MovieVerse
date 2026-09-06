window.MV = window.MV || {};

MV.auth = (() => {
  let cachedToken = null;
  let cachedUser = null;

  function decodeBase64Url(input) {
    const normalized = input.replace(/-/g, "+").replace(/_/g, "/");
    const padded = normalized + "=".repeat((4 - normalized.length % 4) % 4);
    return decodeURIComponent(atob(padded).split("").map(c => `%${("00" + c.charCodeAt(0).toString(16)).slice(-2)}`).join(""));
  }

  function getToken() { return localStorage.getItem(MV.config.TOKEN_KEY); }
  function setToken(token) { localStorage.setItem(MV.config.TOKEN_KEY, token); cachedToken = null; cachedUser = null; }
  function clearToken() { localStorage.removeItem(MV.config.TOKEN_KEY); cachedToken = null; cachedUser = null; MV.library?.reset?.(); }

  function getUser() {
    const token = getToken();
    if (!token) return null;
    if (token === cachedToken && cachedUser) return cachedUser;
    try {
      const payload = JSON.parse(decodeBase64Url(token.split(".")[1]));
      if (payload.exp && Date.now() >= payload.exp * 1000) { clearToken(); return null; }
      const roleClaim = payload.role ?? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? [];
      const roles = Array.isArray(roleClaim) ? roleClaim : [roleClaim].filter(Boolean);
      const user = {
        id: payload.sub || payload.nameid || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null,
        userName: payload.unique_name || payload.name || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "",
        email: payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || "",
        roles,
        exp: payload.exp || null
      };
      cachedToken = token;
      cachedUser = user;
      return user;
    } catch {
      clearToken();
      return null;
    }
  }

  function isAuthenticated() { return !!getUser(); }
  function hasRole(...roles) { const user = getUser(); return !!user && roles.some(r => user.roles.some(ur => ur.toLowerCase() === r.toLowerCase())); }
  function isAdmin() { return hasRole("Admin", "SuperAdmin"); }
  function isSuperAdmin() { return hasRole("SuperAdmin"); }

  function currentRelativeUrl() {
    return location.pathname.split("/").pop() + location.search + location.hash;
  }

  function loginUrl(returnUrl = currentRelativeUrl()) {
    return `login.html?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  function requireAuth(returnUrl) {
    if (isAuthenticated()) return true;
    location.href = loginUrl(returnUrl);
    return false;
  }

  function requireAdmin() {
    if (!isAuthenticated()) {
      const rel = location.pathname.includes("/admin/") ? `admin/${location.pathname.split("/").pop()}${location.search}` : currentRelativeUrl();
      location.replace(`../login.html?returnUrl=${encodeURIComponent(rel)}`);
      return false;
    }
    if (!isAdmin()) {
      location.replace("../index.html");
      return false;
    }
    return true;
  }

  return { getToken, setToken, clearToken, getUser, isAuthenticated, hasRole, isAdmin, isSuperAdmin, requireAuth, requireAdmin, loginUrl };
})();
