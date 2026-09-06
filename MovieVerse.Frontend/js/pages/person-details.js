document.addEventListener("DOMContentLoaded", async () => {
  const type = String(MV.ui.getParam("type") || "").toLowerCase();
  const id = MV.ui.getParam("id");
  const valid = ["actor", "director", "writer"];

  if (!valid.includes(type) || !id) {
    location.replace("search.html");
    return;
  }

  const root = document.getElementById("personPage");
  root.innerHTML = `<main class="mv-main"><div class="mv-container">${MV.ui.skeletonLines(6)}</div></main>`;

  try {
    const person = await MV.api.get(`${type}s/${id}`, null, { auth:false });
    document.title = `${person.fullName} • MovieVerse`;
    render(person);
  } catch (err) {
    root.innerHTML = `<main class="mv-main"><div class="mv-container">${MV.ui.emptyState({ icon:"fa-user", title:"Person not found", text:err.detail || "This profile could not be loaded.", actionText:"Search", actionHref:"search.html" })}</div></main>`;
  }

  function render(p) {
    const e = MV.ui.escapeHtml;
    const optional = MV.ui.optionalText;
    const role = type.charAt(0).toUpperCase() + type.slice(1);
    const biography = optional(p.biography);

    const primary = [
      ["Born", formatDatePlace(p.birthDate, p.birthPlace)],
      ["Died", formatDatePlace(p.deathDate, p.deathPlace)],
      ["Height", p.heightInMeters ? `${Number(p.heightInMeters).toFixed(2)} m` : ""],
      ["Alternative name", optional(p.alternativeName)],
      ["Nickname", optional(p.nickname)]
    ].filter(([, value]) => value);

    const secondary = [
      ["Spouse", optional(p.spouse)],
      ["Children", optional(p.children)],
      ["Parents", optional(p.parents)],
      ["Relatives", optional(p.relatives)],
      ["Other works", optional(p.otherWorks)],
      ["Trivia", optional(p.trivia)],
      ["Quote", optional(p.quote)],
      ["Trademark", optional(p.trademark)]
    ].filter(([, value]) => value);

    const films = [...(p.filmography || [])].sort((a, b) => (b.releaseYear || 0) - (a.releaseYear || 0));
    const portrait = MV.media.personImageUrl(p.profileImageUrl, type);

    root.innerHTML = `<main class="mv-main"><div class="mv-container">
      <section class="mv-person-hero">
        <img class="mv-person-portrait" src="${e(portrait)}" ${MV.media.imageFallbackAttributes("people")} alt="${e(p.fullName)}">
        <div>
          <span class="mv-role-badge"><i class="fa-solid ${type === "actor" ? "fa-masks-theater" : type === "director" ? "fa-video" : "fa-pen-nib"}"></i>${role}</span>
          <h1 class="mv-detail-title mt-2 mb-2">${e(p.fullName)}</h1>
          ${biography ? `<p class="fs-5 text-secondary">${e(biography)}</p>` : ""}
          ${primary.length ? `<dl class="mv-details-list mv-surface px-3 mt-3">${primary.map(([k, v]) => `<div class="row"><dt class="col-sm-4">${e(k)}</dt><dd class="col-sm-8">${e(v)}</dd></div>`).join("")}</dl>` : ""}
        </div>
      </section>

      <section class="mv-section">
        <h2 class="mv-section-title">Filmography</h2>
        ${films.length ? `<div class="mv-filmography">${films.map(f => filmItem(f)).join("")}</div>` : MV.ui.emptyState({ icon:"fa-clapperboard", title:"No filmography listed yet" })}
      </section>

      ${secondary.length ? `<section class="mv-section"><h2 class="mv-section-title">Personal details & trivia</h2><dl class="mv-details-list mv-surface px-3">${secondary.map(([k, v]) => `<div class="row"><dt class="col-sm-4">${e(k)}</dt><dd class="col-sm-8">${e(v)}</dd></div>`).join("")}</dl></section>` : ""}
    </div></main>`;
  }

  function formatDatePlace(date, place) {
    return [date ? MV.media.date(date) : "", MV.ui.optionalText(place)].filter(Boolean).join(" · ");
  }

  function filmItem(f) {
    const isMovie = String(f.contentType).toLowerCase() === "movie";
    const contentType = isMovie ? "Movie" : "TVShow";
    const href = isMovie ? `movie-details.html?id=${f.id}` : `tvshow-details.html?id=${f.id}`;
    const extra = [];
    const characterName = MV.ui.optionalText(f.characterName);

    if (type === "actor" && characterName) extra.push(`as ${characterName}`);
    if (f.episodeCount) extra.push(`${f.episodeCount} episode${f.episodeCount === 1 ? "" : "s"}`);

    return `<a class="mv-filmography-item" href="${href}">
      <img src="${MV.ui.escapeHtml(MV.media.titleImageUrl(f.posterUrl, contentType))}" ${MV.media.imageFallbackAttributes("movie")} alt="">
      <div><strong>${MV.ui.escapeHtml(f.title)}</strong><div class="text-secondary small">${MV.ui.escapeHtml(extra.join(" · "))}</div></div>
      <div class="text-end"><span class="badge mv-badge">${isMovie ? "Movie" : "TV Show"}</span><div class="text-secondary small mt-1">${f.releaseYear || ""}</div></div>
    </a>`;
  }
});
