document.addEventListener("DOMContentLoaded", async () => {
  const results = document.getElementById("searchResults");
  if (!results) return;
  try { await MV.library.load(); } catch { }
  MV.media.bindCardActions(document);

  const params = new URLSearchParams(location.search);
  const q = params.get("q") || "";
  const type = params.get("type") || "All";
  document.getElementById("searchHeading").textContent = q ? `Results for “${q}”` : "Browse MovieVerse";
  document.getElementById("searchSubheading").textContent =
  type === "People"
    ? (q
        ? "Matching actors, directors and writers"
        : "Browse actors, directors and writers.")
    : q
      ? "Movies, TV shows and matching people"
      : "Use the filters to explore the catalog.";

  let genres=[],actors=[],directors=[];
  try { [genres,actors,directors]=await Promise.all([MV.api.get("genres",null,{auth:false}),MV.api.get("actors",null,{auth:false}),MV.api.get("directors",null,{auth:false})]); } catch(err){ MV.ui.showError(err); }
  renderFilters("filterBarDesktop",false); renderFilters("filterBarMobile",true); syncControls();
  document.querySelectorAll(".js-filter-form").forEach(form=>{MV.ui.useBackendValidation(form);form.addEventListener("submit",applyFilters);});
  document.querySelectorAll(".js-clear-filters").forEach(btn=>btn.addEventListener("click",()=>location.href="search.html"));
  await loadResults();

function renderFilters(hostId, mobile) {
  const host = document.getElementById(hostId);
  if (!host) return;

  const titleFilters = type === "People"
    ? ""
    : `
      <div>
        <label class="form-label">Genre</label>
        <select class="form-select form-select-sm" name="genreId">
          <option value="">All genres</option>
          ${genres.map(g => `
            <option value="${g.id}">
              ${MV.ui.escapeHtml(g.name)}
            </option>
          `).join("")}
        </select>
      </div>

      <div>
        <label class="form-label">Year</label>
        <input
          class="form-control form-control-sm"
          name="year"
          type="number"
          min="1888"
          max="2100"
          placeholder="Any">
      </div>

      <div>
        <label class="form-label">Min rating</label>
        <input
          class="form-control form-control-sm"
          name="min"
          type="number"
          min="1"
          max="10"
          step="0.1"
          placeholder="1">
      </div>

      <div>
        <label class="form-label">Max rating</label>
        <input
          class="form-control form-control-sm"
          name="max"
          type="number"
          min="1"
          max="10"
          step="0.1"
          placeholder="10">
      </div>

      <div>
        <label class="form-label">Actor</label>
        <select class="form-select form-select-sm" name="actorId">
          <option value="">Any actor</option>
          ${actors.map(x => `
            <option value="${x.id}">
              ${MV.ui.escapeHtml(x.fullName)}
            </option>
          `).join("")}
        </select>
      </div>

      <div>
        <label class="form-label">Director</label>
        <select class="form-select form-select-sm" name="directorId">
          <option value="">Any director</option>
          ${directors.map(x => `
            <option value="${x.id}">
              ${MV.ui.escapeHtml(x.fullName)}
            </option>
          `).join("")}
        </select>
      </div>

      <div>
        <label class="form-label">Sort</label>
        <select class="form-select form-select-sm" name="sortBy">
          <option value="title">Title</option>
          <option value="year">Year</option>
          <option value="rating">Rating</option>
        </select>
      </div>

      <div>
        <label class="form-label">Direction</label>
        <select class="form-select form-select-sm" name="desc">
          <option value="false">Ascending</option>
          <option value="true">Descending</option>
        </select>
      </div>
    `;

  host.innerHTML = `
    <form
      class="js-filter-form ${mobile ? "d-grid gap-3" : "mv-filter-row"}"
      novalidate>

      <div>
        <label class="form-label">Content</label>

        <select
          class="form-select form-select-sm"
          name="type">

          <option>All</option>
          <option value="Movie">Movies</option>
          <option value="TVShow">TV Shows</option>
          <option value="People">People</option>

        </select>
      </div>

      ${titleFilters}

      <div class="d-flex gap-2">

        <button
          class="btn btn-primary btn-sm flex-grow-1"
          type="submit">

          <i class="fa-solid fa-filter me-1"></i>
          Apply

        </button>

        <button
          class="btn btn-secondary btn-sm js-clear-filters"
          type="button"
          aria-label="Clear filters">

          <i class="fa-solid fa-xmark"></i>

        </button>

      </div>

    </form>
  `;
}

  function syncControls(){
    document.querySelectorAll(".js-filter-form").forEach(form=>{
      ["type","genreId","year","min","max","actorId","directorId","sortBy","desc"].forEach(name=>{
        const el=form.elements[name]; if(!el)return;
        const key={year:"year",min:"min",max:"max"}[name]||name;
        const fallback=name==="type"?"All":name==="sortBy"?"title":name==="desc"?"false":"";
        el.value=params.get(key)??fallback;
      });
    });
  }

  function applyFilters(event){
    event.preventDefault(); const fd=new FormData(event.currentTarget); const next=new URLSearchParams();
    if(q)next.set("q",q);
    for(const [key,value] of fd.entries()){ if(value && !(key==="type"&&value==="All") && !(key==="sortBy"&&value==="title") && !(key==="desc"&&value==="false")) next.set(key,value); }
    location.href=`search.html${next.toString()?`?${next}`:""}`;
  }

  function catalogQuery(page){ return { Search:q||undefined, GenreId:params.get("genreId")||undefined, ReleaseYear:params.get("year")||undefined, MinRating:params.get("min")||undefined, MaxRating:params.get("max")||undefined, ActorId:params.get("actorId")||undefined, DirectorId:params.get("directorId")||undefined, SortBy:params.get("sortBy")||"title", SortDescending:params.get("desc")==="true", PageNumber:page, PageSize:type==="All"?8:12 }; }

async function loadResults() {
  const page = Math.max(1, Number(params.get("page") || 1));

if (type === "People") {
  await loadPeoplePage(q, page);
  return;
}

  results.innerHTML = `
    <div class="mv-media-grid">
      ${MV.ui.skeletonCards(type === "All" ? 8 : 12)}
    </div>
  `;

  document.getElementById("peopleResults").innerHTML = "";

  try {
    let movieData = null;
    let tvData = null;

    if (type === "Movie") {
      movieData = await MV.api.get(
        "movies",
        catalogQuery(page),
        { auth: false }
      );
    }
    else if (type === "TVShow") {
      tvData = await MV.api.get(
        "tvshows",
        catalogQuery(page),
        { auth: false }
      );
    }
    else {
      [movieData, tvData] = await Promise.all([
        MV.api.get("movies", catalogQuery(page), { auth: false }),
        MV.api.get("tvshows", catalogQuery(page), { auth: false })
      ]);
    }

    let html = "";
    let maxPages = 1;

    if (movieData) {
      maxPages = Math.max(maxPages, movieData.totalPages || 1);
      html += section(
        "Movies",
        movieData.items || [],
        "Movie"
      );
    }

    if (tvData) {
      maxPages = Math.max(maxPages, tvData.totalPages || 1);
      html += section(
        "TV Shows",
        tvData.items || [],
        "TVShow"
      );
    }

    if (
      !(
        (movieData?.items?.length || 0) +
        (tvData?.items?.length || 0)
      )
    ) {
      html = MV.ui.emptyState({
        icon: "fa-magnifying-glass",
        title: "No results found",
        text: "Try changing the search or clearing some filters.",
        actionText: "Clear filters",
        actionHref: q
          ? `search.html?q=${encodeURIComponent(q)}`
          : "search.html"
      });
    }

    results.innerHTML = html;

    document.getElementById("paginationHost").innerHTML =
      maxPages > 1
        ? pagination(page, maxPages)
        : "";

    // Only show the extra People section when browsing ALL results.
    if (q && type === "All") {
      await loadPeople(q);
    }
  }
  catch (err) {
    results.innerHTML = MV.ui.emptyState({
      icon: "fa-circle-exclamation",
      title: "Could not load results",
      text: err.detail || "Try again."
    });
  }
}

  function section(title,items,kind){ if(!items.length)return ""; return `<section class="mb-4"><div class="mv-results-head"><h2 class="mv-section-title mb-0">${title}</h2><span class="text-secondary">${items.length} on this page</span></div><div class="mv-media-grid">${items.map(x=>MV.media.mediaCard(x,kind)).join("")}</div></section>`; }
  function pagination(page,total){ const pages=[]; const start=Math.max(1,page-2),end=Math.min(total,page+2); if(start>1)pages.push(1); for(let i=start;i<=end;i++)pages.push(i); if(end<total)pages.push(total); return `<nav aria-label="Catalog pages"><ul class="pagination justify-content-center flex-wrap">${pages.map((p,i)=>`${i&&p-pages[i-1]>1?'<li class="page-item disabled"><span class="page-link">…</span></li>':""}<li class="page-item ${p===page?"active":""}"><a class="page-link" href="${pageUrl(p)}">${p}</a></li>`).join("")}</ul></nav>`; }
  function pageUrl(p){ const x=new URLSearchParams(params); x.set("page",p); return `search.html?${x}`; }
  async function loadPeople(query){
    const host=document.getElementById("peopleResults"); if(!host)return; host.innerHTML='<div class="mv-skeleton mv-skeleton-line"></div>';
    try { const data=await MV.api.get("search",{query,limit:10},{auth:false}); const people=[...(data.actors||[]),...(data.directors||[]),...(data.writers||[])]; host.innerHTML=people.length?`<div class="mv-section" id="people"><h2 class="mv-section-title">People</h2><div class="mv-people-results">${people.map(p=>{const personType=String(p.resultType).toLowerCase();return `<a class="mv-search-person" href="person-details.html?type=${personType}&id=${p.id}"><img src="${MV.media.personImageUrl(p.imageUrl,personType)}" ${MV.media.imageFallbackAttributes("people")} alt=""><div><strong>${MV.ui.escapeHtml(p.title)}</strong><div class="text-secondary small">${MV.ui.escapeHtml(MV.ui.optionalText(p.subtitle)||p.resultType)}</div></div></a>`;}).join("")}</div></div>`:""; } catch { host.innerHTML=""; }
  }
});

async function loadPeoplePage(query, page = 1) {
  const results = document.getElementById("searchResults");
  const paginationHost = document.getElementById("paginationHost");
  const peopleResults = document.getElementById("peopleResults");

  if (peopleResults) {
    peopleResults.innerHTML = "";
  }

  paginationHost.innerHTML = "";

  results.innerHTML = `
    <div class="mv-skeleton mv-skeleton-line"></div>
  `;

  try {
    const [
      actorRows,
      directorRows,
      writerRows
    ] = await Promise.all([
      MV.api.get("actors", null, { auth: false }),
      MV.api.get("directors", null, { auth: false }),
      MV.api.get("writers", null, { auth: false })
    ]);

    // Put actors, directors and writers into one People list.
    let people = [
      ...(actorRows || []).map(person =>
        normalizePerson(person, "Actor")
      ),

      ...(directorRows || []).map(person =>
        normalizePerson(person, "Director")
      ),

      ...(writerRows || []).map(person =>
        normalizePerson(person, "Writer")
      )
    ];

    // If the user searched for a name, filter the People list.
    const cleanQuery = String(query || "")
      .trim()
      .toLowerCase();

    if (cleanQuery) {
      people = people.filter(person =>
        person.title
          .toLowerCase()
          .includes(cleanQuery)
      );
    }

    // Always show People alphabetically.
    people.sort((a, b) =>
      a.title.localeCompare(
        b.title,
        undefined,
        { sensitivity: "base" }
      )
    );

    const PAGE_SIZE = 12;

    const totalPeople = people.length;

    if (!totalPeople) {
      results.innerHTML = MV.ui.emptyState({
        icon: "fa-user",
        title: "No people found",
        text: query
          ? `No actors, directors or writers match “${query}”.`
          : "No people have been added yet."
      });

      return;
    }

    const totalPages = Math.ceil(
      totalPeople / PAGE_SIZE
    );

    const currentPage = Math.min(
      Math.max(1, Number(page) || 1),
      totalPages
    );

    const startIndex =
      (currentPage - 1) * PAGE_SIZE;

    const peopleOnPage = people.slice(
      startIndex,
      startIndex + PAGE_SIZE
    );

    const firstNumber = startIndex + 1;
    const lastNumber =
      startIndex + peopleOnPage.length;

    results.innerHTML = `
      <section class="mv-section">

        <div class="mv-results-head">

          <h2 class="mv-section-title mb-0">
            People
          </h2>

          <span class="text-secondary">
            ${firstNumber}–${lastNumber} of ${totalPeople}
          </span>

        </div>

        <div class="mv-people-results">

          ${peopleOnPage.map(person =>
            peopleCard(person)
          ).join("")}

        </div>

      </section>
    `;

    paginationHost.innerHTML =
      totalPages > 1
        ? peoplePagination(
            currentPage,
            totalPages
          )
        : "";
  }
  catch (err) {
    results.innerHTML = MV.ui.emptyState({
      icon: "fa-circle-exclamation",
      title: "Could not load people",
      text: err.detail || "Try again."
    });
  }
}


function normalizePerson(person, resultType) {
  return {
    id: person.id,
    resultType: resultType,
    title: person.fullName,
    imageUrl: person.profileImageUrl
  };
}


function peopleCard(person) {
  const personType =
    String(person.resultType).toLowerCase();

  return `
    <a
      class="mv-search-person"
      href="person-details.html?type=${encodeURIComponent(personType)}&id=${encodeURIComponent(person.id)}">

      <img
        src="${MV.ui.escapeHtml(
          MV.media.personImageUrl(
            person.imageUrl,
            personType
          )
        )}"
        ${MV.media.imageFallbackAttributes("people")}
        alt="">

      <div>

        <strong>
          ${MV.ui.escapeHtml(person.title)}
        </strong>

        <div class="text-secondary small">
          ${MV.ui.escapeHtml(person.resultType)}
        </div>

      </div>

    </a>
  `;
}


function peoplePagination(page, totalPages) {
  const pages = [];

  const start = Math.max(
    1,
    page - 2
  );

  const end = Math.min(
    totalPages,
    page + 2
  );

  if (start > 1) {
    pages.push(1);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  if (end < totalPages) {
    pages.push(totalPages);
  }

  let html = `
    <nav aria-label="People pages">
      <ul class="pagination justify-content-center flex-wrap">
  `;

  pages.forEach((pageNumber, index) => {

    if (
      index > 0 &&
      pageNumber - pages[index - 1] > 1
    ) {
      html += `
        <li class="page-item disabled">
          <span class="page-link">…</span>
        </li>
      `;
    }

    const params =
      new URLSearchParams(location.search);

    params.set(
      "page",
      pageNumber
    );

    html += `
      <li
        class="page-item ${
          pageNumber === page
            ? "active"
            : ""
        }">

        <a
          class="page-link"
          href="search.html?${params.toString()}">

          ${pageNumber}

        </a>

      </li>
    `;
  });

  html += `
      </ul>
    </nav>
  `;

  return html;
}