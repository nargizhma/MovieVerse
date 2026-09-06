window.MV = window.MV || {};

MV.media = (() => {
  const actorCache = new Map();
  const boundCardRoots = new WeakSet();
  const personSessionPrefix = "mv_actor_";

  function placeholderUrl(kind = "default", adminDepth = false) {
    const configured = kind === "people"
      ? MV.config.PEOPLE_PLACEHOLDER_IMAGE
      : kind === "movie"
        ? MV.config.MOVIE_PLACEHOLDER_IMAGE
        : MV.config.PLACEHOLDER_IMAGE;

    return adminDepth ? `../${configured}` : configured;
  }

  function backendFolderFor(type) {
    switch (String(type || "").toLowerCase()) {
      case "movie": return "movies";
      case "tvshow":
      case "tv show": return "tvshows";
      case "episode": return "episodes";
      case "actor": return "actors";
      case "director": return "directors";
      case "writer": return "writers";
      case "profile":
      case "user": return "profiles";
      default: return null;
    }
  }

  function resolveImageUrl(url, backendFolder = null) {
    const raw = MV.ui.optionalText ? MV.ui.optionalText(url) : String(url ?? "").trim();
    if (!raw) return null;

    try {
      if (/^(https?:|blob:|data:)/i.test(raw)) return raw;
      if (raw.startsWith("assets/") || raw.startsWith("../assets/")) return raw;
      if (raw.startsWith("/")) return `${MV.config.BACKEND_ORIGIN}${raw}`;

      // Some endpoints may return only the stored file name. If we know what
      // kind of image it is, rebuild the proper static-file URL here.
      if (backendFolder && !raw.includes("/")) {
        return `${MV.config.BACKEND_ORIGIN}/images/${backendFolder}/${encodeURIComponent(raw)}`;
      }

      return `${MV.config.BACKEND_ORIGIN}/${raw.replace(/^\//, "")}`;
    } catch {
      return null;
    }
  }

  function imageUrl(url, adminDepth = false, kind = "default", backendFolder = null) {
    return resolveImageUrl(url, backendFolder) || placeholderUrl(kind, adminDepth);
  }

  function titleImageUrl(url, type = "Movie", adminDepth = false) {
    return imageUrl(url, adminDepth, "movie", backendFolderFor(type));
  }

  // Explicit helpers for title artwork. These all intentionally use
  // MOVIE_PLACEHOLDER_IMAGE when the URL is missing OR the real image fails.
  function movieImageUrl(url, adminDepth = false) {
    return imageUrl(url, adminDepth, "movie", "movies");
  }

  function tvShowImageUrl(url, adminDepth = false) {
    return imageUrl(url, adminDepth, "movie", "tvshows");
  }

  function episodeImageUrl(url, adminDepth = false) {
    return imageUrl(url, adminDepth, "movie", "episodes");
  }

  function personImageUrl(url, type = "actor", adminDepth = false) {
    return imageUrl(url, adminDepth, "people", backendFolderFor(type));
  }

  function profileImageUrl(url, adminDepth = false) {
    return imageUrl(url, adminDepth, "people", "profiles");
  }

  function imageFallbackAttributes(kind = "default", adminDepth = false) {
    return `data-mv-placeholder-kind="${kind}" data-mv-admin-depth="${adminDepth ? "true" : "false"}"`;
  }

  // This catches real image-load failures too (404, bad stored URL, deleted file),
  // not only null/empty URLs. That prevents the browser from showing alt text
  // instead of a placeholder.
  document.addEventListener("error", event => {
    const img = event.target;
    if (!(img instanceof HTMLImageElement)) return;
    if (img.dataset.mvFallbackApplied === "true") return;

    img.dataset.mvFallbackApplied = "true";
    const kind = img.dataset.mvPlaceholderKind || "default";
    const adminDepth = img.dataset.mvAdminDepth === "true";
    img.src = placeholderUrl(kind, adminDepth);
  }, true);

  function year(date) {
    const d = date ? new Date(date) : null;
    return d && !Number.isNaN(d.getTime()) ? d.getFullYear() : "";
  }

  function date(dateValue) {
    if (!dateValue) return "";
    const d = new Date(dateValue);
    return Number.isNaN(d.getTime()) ? "" : d.toLocaleDateString(undefined, { year:"numeric", month:"short", day:"numeric" });
  }

  function runtime(minutes) {
    if (!minutes) return "";
    const h = Math.floor(minutes / 60), m = minutes % 60;
    return h ? `${h}h ${m ? `${m}m` : ""}`.trim() : `${m}m`;
  }

  function rating(value) {
    return value === null || value === undefined ? "—" : Number(value).toFixed(1);
  }

  function money(value) {
    if (value === null || value === undefined || value === "") return "";
    return new Intl.NumberFormat(undefined, { style:"currency", currency:"USD", maximumFractionDigits:0 }).format(Number(value));
  }

  function detailsUrl(type, id) {
    return type === "Movie" || type === "movie"
      ? `movie-details.html?id=${encodeURIComponent(id)}`
      : `tvshow-details.html?id=${encodeURIComponent(id)}`;
  }

  function personUrl(type, id) {
    return `person-details.html?type=${encodeURIComponent(String(type).toLowerCase())}&id=${encodeURIComponent(id)}`;
  }

  function searchResultImageUrl(item, adminDepth = false) {
    const resultType = String(item?.resultType || "").toLowerCase();
    if (["actor", "director", "writer", "user", "profile", "admin", "superadmin", "super admin"].includes(resultType)) {
      const imageType = ["actor", "director", "writer"].includes(resultType) ? resultType : "user";
      return personImageUrl(item?.imageUrl, imageType, adminDepth);
    }
    if (resultType === "movie") return titleImageUrl(item?.imageUrl, "Movie", adminDepth);
    if (resultType === "tvshow" || resultType === "tv show") return titleImageUrl(item?.imageUrl, "TVShow", adminDepth);
    return imageUrl(item?.imageUrl, adminDepth);
  }

  function searchResultPlaceholderKind(item) {
    const resultType = String(item?.resultType || "").toLowerCase();
    if (["actor", "director", "writer", "user", "profile", "admin", "superadmin", "super admin"].includes(resultType)) return "people";
    if (["movie", "tvshow", "tv show", "episode"].includes(resultType)) return "movie";
    return "default";
  }

  function youtubeEmbed(url) {
    const safeUrl = MV.ui.optionalText ? MV.ui.optionalText(url) : url;
    if (!safeUrl) return null;
    try {
      const u = new URL(safeUrl);
      let id = null;
      if (u.hostname.includes("youtu.be")) id = u.pathname.split("/").filter(Boolean)[0];
      else if (u.hostname.includes("youtube.com")) {
        if (u.pathname === "/watch") id = u.searchParams.get("v");
        else if (u.pathname.startsWith("/embed/")) id = u.pathname.split("/")[2];
        else if (u.pathname.startsWith("/shorts/")) id = u.pathname.split("/")[2];
      }
      return id ? `https://www.youtube.com/embed/${encodeURIComponent(id)}` : null;
    } catch {
      return null;
    }
  }

  function trailerHtml(url, title = "Trailer") {
    const safeUrl = MV.ui.optionalText ? MV.ui.optionalText(url) : url;
    const embed = youtubeEmbed(safeUrl);
    if (embed) {
      return `<iframe src="${MV.ui.escapeHtml(embed)}" title="${MV.ui.escapeHtml(title)}" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen loading="lazy"></iframe>`;
    }
    if (safeUrl) {
      return `<div class="text-center p-4"><i class="fa-solid fa-circle-play fa-3x text-secondary mb-3"></i><p class="text-secondary">This trailer cannot be embedded here.</p><a class="btn btn-outline-primary" href="${MV.ui.escapeHtml(MV.ui.safeHref(safeUrl))}" target="_blank" rel="noopener">Open Trailer</a></div>`;
    }
    return `<div class="text-center p-4 text-secondary"><i class="fa-solid fa-video-slash fa-3x mb-3"></i><p class="mb-0">No trailer available.</p></div>`;
  }

  async function getActor(actorId) {
    if (!actorId) return null;
    if (actorCache.has(actorId)) return actorCache.get(actorId);
    try {
      const stored = sessionStorage.getItem(personSessionPrefix + actorId);
      if (stored) {
        const data = JSON.parse(stored);
        actorCache.set(actorId, data);
        return data;
      }
    } catch { }

    const promise = MV.api.get(`actors/${actorId}`).then(data => {
      actorCache.set(actorId, data);
      try { sessionStorage.setItem(personSessionPrefix + actorId, JSON.stringify(data)); } catch { }
      return data;
    }).catch(err => {
      actorCache.delete(actorId);
      throw err;
    });

    actorCache.set(actorId, promise);
    return promise;
  }

  function mediaCard(item, type, { showWatched = true, adminDepth = false, className = "" } = {}) {
    const e = MV.ui.escapeHtml;
    const typeLabel = type === "Movie" ? "Movie" : "TV Show";
    const href = adminDepth
      ? `../${type === "Movie" ? "movie-details.html" : "tvshow-details.html"}?id=${encodeURIComponent(item.id)}`
      : detailsUrl(type, item.id);
    const genres = (item.genres || []).map(g => g.name).join(" • ");
    const isAuthed = MV.auth.isAuthenticated();
    const saved = isAuthed && MV.library?.isInWatchlist?.(type, item.id);
    const watched = isAuthed && MV.library?.isWatched?.(type, item.id);
    const hoverParts = [];
    const runtimeLabel = runtime(item.runtimeMinutes);

    if (runtimeLabel) hoverParts.push(`<span><i class="fa-regular fa-clock me-1"></i>${e(runtimeLabel)}</span>`);
    if (item.reviewCount !== undefined && item.reviewCount !== null) {
      hoverParts.push(`<span>${Number(item.reviewCount)} rating${Number(item.reviewCount) === 1 ? "" : "s"}</span>`);
    }

    const hoverHtml = hoverParts.length ? `<div class="mv-card-hover">${hoverParts.join("")}</div>` : "";
    const poster = titleImageUrl(item.posterUrl, type, adminDepth);

    return `<article class="mv-media-card ${e(className)}" data-content-type="${e(type)}" data-content-id="${e(item.id)}">
      <div class="mv-poster-wrap">
        <a href="${href}" aria-label="Open ${e(item.title)}"><img class="mv-poster" src="${e(poster)}" ${imageFallbackAttributes("movie", adminDepth)} alt="${e(item.title)} poster" loading="lazy"></a>
        <span class="mv-content-badge">${typeLabel}</span>
        <div class="mv-card-actions">
          <button class="mv-icon-btn js-watchlist ${saved ? "is-active" : ""}" type="button" aria-label="${saved ? "Remove from" : "Add to"} watchlist"><i class="${saved ? "fa-solid" : "fa-regular"} fa-bookmark"></i></button>
          ${showWatched ? `<button class="mv-icon-btn js-watched ${watched ? "is-active" : ""}" type="button" aria-label="${watched ? "Mark unwatched" : "Mark watched"}"><i class="${watched ? "fa-solid" : "fa-regular"} fa-circle-check"></i></button>` : ""}
        </div>
        ${hoverHtml}
      </div>
      <div class="mv-card-body">
        <a class="mv-card-title d-block" href="${href}" title="${e(item.title)}">${e(item.title)}</a>
        <div class="mv-card-meta"><span>${e(year(item.releaseDate))}</span><span class="mv-rating-inline"><i class="fa-solid fa-star"></i> ${rating(item.averageRating)}</span></div>
        <div class="mv-card-genres" title="${e(genres)}">${e(genres)}</div>
      </div>
    </article>`;
  }

  async function castCards(cast = []) {
    const sorted = [...cast].sort((a, b) => (a.castOrder ?? 0) - (b.castOrder ?? 0));
    const cards = await Promise.all(sorted.map(async member => {
      let actor = null;
      try { actor = await getActor(member.actorId); } catch { }
      const img = personImageUrl(actor?.profileImageUrl, "actor");
      const characterName = MV.ui.optionalText(member.characterName);

      return `<a class="mv-person-card" href="${personUrl("actor", member.actorId)}"><img src="${MV.ui.escapeHtml(img)}" ${imageFallbackAttributes("people")} alt="${MV.ui.escapeHtml(member.fullName)}" loading="lazy"><strong>${MV.ui.escapeHtml(member.fullName)}</strong>${characterName ? `<small>${MV.ui.escapeHtml(characterName)}</small>` : ""}</a>`;
    }));
    return cards.join("");
  }

  function bindCardActions(root = document) {
    if (boundCardRoots.has(root)) return;
    boundCardRoots.add(root);
    root.addEventListener("click", async event => {
      const watchBtn = event.target.closest(".js-watchlist");
      const watchedBtn = event.target.closest(".js-watched");
      if (!watchBtn && !watchedBtn) return;

      event.preventDefault();
      event.stopPropagation();
      const card = event.target.closest("[data-content-type][data-content-id]");
      if (!card) return;

      const type = card.dataset.contentType;
      const id = card.dataset.contentId;
      if (!MV.auth.requireAuth()) return;

      try {
        if (watchBtn) {
          await MV.library.toggleWatchlist(type, id);
          updateLibraryButton(watchBtn, MV.library.isInWatchlist(type, id), "watchlist");
        }
        if (watchedBtn) {
          await MV.library.toggleWatched(type, id);
          updateLibraryButton(watchedBtn, MV.library.isWatched(type, id), "watched");
        }
      } catch (err) {
        MV.ui.showError(err);
      }
    });
  }

  function updateLibraryButton(button, active, kind) {
    if (!button) return;
    button.classList.toggle("is-active", active);
    const icon = button.querySelector("i");
    icon.className = `${active ? "fa-solid" : "fa-regular"} ${kind === "watchlist" ? "fa-bookmark" : "fa-circle-check"}`;
    button.setAttribute("aria-label", kind === "watchlist"
      ? `${active ? "Remove from" : "Add to"} watchlist`
      : `${active ? "Mark unwatched" : "Mark watched"}`);
  }

  return {
    imageUrl,
    titleImageUrl,
    movieImageUrl,
    tvShowImageUrl,
    episodeImageUrl,
    personImageUrl,
    profileImageUrl,
    searchResultImageUrl,
    searchResultPlaceholderKind,
    placeholderUrl,
    imageFallbackAttributes,
    year,
    date,
    runtime,
    rating,
    money,
    detailsUrl,
    personUrl,
    youtubeEmbed,
    trailerHtml,
    getActor,
    mediaCard,
    castCards,
    bindCardActions,
    updateLibraryButton
  };
})();
