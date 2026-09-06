document.addEventListener("DOMContentLoaded", async () => {
  const id = MV.ui.requireParam("id");
  if (!id) return;

  const root = document.getElementById("moviePage");
  if (!root) return;

  root.innerHTML = `<div class="mv-detail-hero"><div class="mv-container mv-detail-hero-inner">${MV.ui.skeletonLines(5)}<div class="mv-skeleton" style="height:430px;margin-top:1rem"></div></div></div>`;

  try { await MV.library.load(); } catch { }

  let movie;
  try {
    movie = await MV.api.get(`movies/${id}`, null, { auth:false });
  } catch (err) {
    root.innerHTML = `<main class="mv-main"><div class="mv-container">${MV.ui.emptyState({ icon:"fa-film", title:"Movie not found", text:err.detail || "This movie could not be loaded.", actionText:"Browse titles", actionHref:"search.html" })}</div></main>`;
    return;
  }

  document.title = `${movie.title} • MovieVerse`;
  await render(movie);

  async function render(m) {
    const e = MV.ui.escapeHtml;
    const optional = MV.ui.optionalText;
    const year = MV.media.year(m.releaseDate);
    const runtime = MV.media.runtime(m.runtimeMinutes);
    const contentRating = optional(m.contentRating);
    const synopsis = optional(m.synopsis);
    const trailerUrl = optional(m.trailerUrl);
    const storyline = optional(m.storyline);
    const poster = MV.media.movieImageUrl(m.posterUrl);
    const castHtml = m.cast?.length ? await MV.media.castCards(m.cast) : "";
    const genreHtml = (m.genres || []).map(g => `<a class="mv-genre-pill" href="search.html?genreId=${g.id}&type=Movie">${e(g.name)}</a>`).join("");
    const directors = (m.directors || []).map(x => `<a href="person-details.html?type=director&id=${x.id}">${e(x.fullName)}</a>`).join(", ");
    const writers = (m.writers || []).map(x => `<a href="person-details.html?type=writer&id=${x.id}">${e(x.fullName)}</a>`).join(", ");

    root.innerHTML = `
      <section class="mv-detail-hero">
        <div class="mv-detail-hero-bg" style="background-image:url('${e(poster)}')"></div>
        <div class="mv-container mv-detail-hero-inner">
          <h1 class="mv-detail-title">${e(m.title)}</h1>
          <div class="mv-title-meta"><span>${year}</span>${contentRating ? `<span>•</span><span>${e(contentRating)}</span>` : ""}${runtime ? `<span>•</span><span>${e(runtime)}</span>` : ""}</div>
          <div class="mv-detail-layout">
            <img class="mv-detail-poster" src="${e(poster)}" ${MV.media.imageFallbackAttributes("movie")} alt="${e(m.title)} poster">
            <div>
              <div class="mv-trailer-shell">${MV.media.trailerHtml(trailerUrl, `${m.title} trailer`)}</div>
              <div class="mv-genre-row">${genreHtml}</div>
              ${synopsis ? `<p class="fs-5 mb-3">${e(synopsis)}</p>` : ""}
              ${directors ? `<div class="mv-crew-line"><strong>Director</strong> ${directors}</div>` : ""}
              ${writers ? `<div class="mv-crew-line"><strong>Writers</strong> ${writers}</div>` : ""}
            </div>
            <aside class="mv-action-panel">
              <div class="mv-score"><i class="fa-solid fa-star fa-xl" style="color:#f3ce55"></i><div><strong id="movieAvgRating">${MV.media.rating(m.averageRating)}</strong><div class="mv-score-label">MovieVerse rating · ${m.reviewCount} ratings</div></div></div>
              <div class="mv-score"><i class="fa-regular fa-star fa-xl" style="color:var(--accent-hover)"></i><div><strong id="movieYourRating">—</strong><div class="mv-score-label">Your rating</div></div></div>
              <button class="btn btn-primary w-100 mb-2" id="rateMovie"><i class="fa-regular fa-star me-2"></i>Rate</button>
              <button class="btn btn-secondary w-100 mb-2" id="watchlistMovie"><i class="fa-regular fa-bookmark me-2"></i><span>Watchlist</span></button>
              <button class="btn btn-secondary w-100" id="watchedMovie"><i class="fa-regular fa-circle-check me-2"></i><span>Watched</span></button>
            </aside>
          </div>
        </div>
      </section>
      <main class="mv-main"><div class="mv-container">
        ${storyline ? `<section class="mv-section"><h2 class="mv-section-title">Storyline</h2><div class="mv-surface p-3"><p class="mb-0">${e(storyline)}</p></div></section>` : ""}
        <section class="mv-section"><h2 class="mv-section-title">Cast</h2>${castHtml ? `<div class="mv-cast-row">${castHtml}</div>` : MV.ui.emptyState({ icon:"fa-users", title:"No cast listed yet" })}</section>
        <section class="mv-section"><div class="d-flex justify-content-between align-items-center gap-2 mb-3"><h2 class="mv-section-title mb-0">User Reviews</h2><button class="btn btn-outline-primary" id="writeMovieReview"><i class="fa-regular fa-pen-to-square me-2"></i>Write review</button></div><div id="movieReviews">${MV.ui.skeletonLines(4)}</div></section>
        <section class="mv-section"><h2 class="mv-section-title">More Like This</h2><div id="movieSimilar" class="mv-card-row">${MV.ui.skeletonCards(6)}</div></section>
        <section class="mv-section" id="movieDetailsSection"></section>
      </div></main>`;

    bindActions();
    renderAdditional(m);
    await Promise.all([refreshMyRating(), loadReviews(), loadSimilar()]);
  }

  function bindActions() {
    document.getElementById("rateMovie").addEventListener("click", () => MV.rating.open({ type:"movie", id, title:movie.title, onSaved:refreshAfterUserReview }));
    document.getElementById("writeMovieReview").addEventListener("click", () => MV.reviews.openEditor({ type:"movie", id, title:movie.title, onSaved:refreshAfterUserReview }));
    document.getElementById("watchlistMovie").addEventListener("click", async () => {
      if (!MV.auth.requireAuth()) return;
      try { await MV.library.toggleWatchlist("Movie", id); updateLibraryButtons(); } catch (err) { MV.ui.showError(err); }
    });
    document.getElementById("watchedMovie").addEventListener("click", async () => {
      if (!MV.auth.requireAuth()) return;
      try { await MV.library.toggleWatched("Movie", id); updateLibraryButtons(); } catch (err) { MV.ui.showError(err); }
    });
    updateLibraryButtons();
  }

  function updateLibraryButtons() {
    const w = document.getElementById("watchlistMovie");
    const h = document.getElementById("watchedMovie");
    if (w) {
      const active = MV.library.isInWatchlist("Movie", id);
      w.classList.toggle("btn-primary", active);
      w.classList.toggle("btn-secondary", !active);
      w.querySelector("i").className = `${active ? "fa-solid" : "fa-regular"} fa-bookmark me-2`;
      w.querySelector("span").textContent = active ? "In watchlist" : "Watchlist";
    }
    if (h) {
      const active = MV.library.isWatched("Movie", id);
      h.classList.toggle("btn-primary", active);
      h.classList.toggle("btn-secondary", !active);
      h.querySelector("i").className = `${active ? "fa-solid" : "fa-regular"} fa-circle-check me-2`;
      h.querySelector("span").textContent = active ? "Watched" : "Mark watched";
    }
  }

  async function refreshMyRating() {
    const el = document.getElementById("movieYourRating");
    if (!el) return;
    if (!MV.auth.isAuthenticated()) { el.textContent = "—"; return; }
    try {
      const mine = await MV.reviews.getMine("movie", id);
      el.textContent = mine ? Number(mine.rating).toFixed(1) : "—";
    } catch {
      el.textContent = "—";
    }
  }

  async function refreshAfterUserReview() {
    await Promise.all([refreshMyRating(), loadReviews()]);
    try {
      movie = await MV.api.get(`movies/${id}`, null, { auth:false });
      document.getElementById("movieAvgRating").textContent = MV.media.rating(movie.averageRating);
    } catch { }
  }

  async function loadReviews() {
    const host = document.getElementById("movieReviews");
    if (!host) return;
    try {
      host.innerHTML = MV.reviews.cards(await MV.reviews.getAll("movie", id));
    } catch (err) {
      host.innerHTML = MV.ui.emptyState({ icon:"fa-message", title:"Reviews unavailable", text:err.detail || "Try again later." });
    }
  }

  async function loadSimilar() {
    const host = document.getElementById("movieSimilar");
    if (!host) return;
    try {
      const rows = await MV.api.get(`movies/${id}/similar`, { limit:6 }, { auth:false });
      const items = (rows || []).map(x => x.movie).filter(Boolean);
      host.className = items.length ? "mv-card-row" : "";
      host.innerHTML = items.length
        ? items.map(x => MV.media.mediaCard(x, "Movie", { showWatched:false })).join("")
        : MV.ui.emptyState({ icon:"fa-wand-magic-sparkles", title:"No similar movies yet" });
      MV.media.bindCardActions(host);
    } catch (err) {
      host.className = "";
      host.innerHTML = MV.ui.emptyState({ icon:"fa-wand-magic-sparkles", title:"Recommendations unavailable", text:err.detail || "Try again later." });
    }
  }

  function renderAdditional(m) {
    const optional = MV.ui.optionalText;
    const rows = [
      ["Release date", MV.media.date(m.releaseDate)],
      ["Tagline", optional(m.tagline)],
      ["Original language", optional(m.originalLanguage)],
      ["Country of origin", optional(m.countryOfOrigin)],
      ["Filming location", optional(m.filmingLocation)],
      ["Production company", optional(m.productionCompany)],
      ["Budget", MV.media.money(m.budget)],
      ["Gross worldwide", MV.media.money(m.grossWorldwide)],
      ["Color", optional(m.color)],
      ["Sound mix", optional(m.soundMix)]
    ].filter(([, value]) => value !== null && value !== undefined && String(value).trim() !== "");

    const trivia = optional(m.trivia);
    const host = document.getElementById("movieDetailsSection");
    if (!host) return;
    if (!rows.length && !trivia) {
      host.innerHTML = "";
      return;
    }

    host.innerHTML = `<h2 class="mv-section-title">Details</h2><dl class="mv-details-list mv-surface px-3">${rows.map(([k, v]) => `<div class="row"><dt class="col-sm-4">${MV.ui.escapeHtml(k)}</dt><dd class="col-sm-8">${MV.ui.escapeHtml(v)}</dd></div>`).join("")}</dl>${trivia ? `<h2 class="mv-section-title mt-4">Did you know?</h2><div class="mv-surface p-3"><strong>Trivia</strong><p class="text-secondary mt-2 mb-0">${MV.ui.escapeHtml(trivia)}</p></div>` : ""}`;
  }
});
