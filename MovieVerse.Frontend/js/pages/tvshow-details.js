document.addEventListener("DOMContentLoaded", async () => {
  const id=MV.ui.requireParam("id");if(!id)return;
  const root=document.getElementById("tvshowPage");if(!root)return;
  root.innerHTML=`<div class="mv-detail-hero"><div class="mv-container mv-detail-hero-inner">${MV.ui.skeletonLines(5)}<div class="mv-skeleton" style="height:430px;margin-top:1rem"></div></div></div>`;
  try{await MV.library.load();}catch{}
  let show;
  try{show=await MV.api.get(`tvshows/${id}`,null,{auth:false});}catch(err){root.innerHTML=`<main class="mv-main"><div class="mv-container">${MV.ui.emptyState({icon:"fa-tv",title:"TV show not found",text:err.detail||"This show could not be loaded.",actionText:"Browse titles",actionHref:"search.html"})}</div></main>`;return;}
  document.title=`${show.title} • MovieVerse`;
  await render(show);

  async function render(s){
    const e=MV.ui.escapeHtml;
    const start=MV.media.year(s.releaseDate),end=s.endDate?MV.media.year(s.endDate):"";
    const years=end&&end!==start?`${start}–${end}`:start;
    const castHtml=s.cast?.length?await MV.media.castCards(s.cast):"";
    root.innerHTML=`
      <section class="mv-detail-hero"><div class="mv-detail-hero-bg" style="background-image:url('${e(MV.media.imageUrl(s.posterUrl))}')"></div><div class="mv-container mv-detail-hero-inner">
        <h1 class="mv-detail-title">${e(s.title)}</h1>${s.originalTitle&&s.originalTitle!==s.title?`<div class="text-secondary mb-1">Original title: ${e(s.originalTitle)}</div>`:""}
        <div class="mv-title-meta"><span>TV Series</span><span>•</span><span>${e(years)}</span>${s.contentRating?`<span>•</span><span>${e(s.contentRating)}</span>`:""}${s.runtimeMinutes?`<span>•</span><span>${e(MV.media.runtime(s.runtimeMinutes))}</span>`:""}</div>
        <div class="mv-detail-layout">
          <img class="mv-detail-poster" src="${e(MV.media.imageUrl(s.posterUrl))}" alt="${e(s.title)} poster">
          <div><div class="mv-trailer-shell">${MV.media.trailerHtml(s.trailerUrl,`${s.title} trailer`)}</div><div class="mv-genre-row">${(s.genres||[]).map(g=>`<a class="mv-genre-pill" href="search.html?genreId=${g.id}&type=TVShow">${e(g.name)}</a>`).join("")}</div><p class="fs-5 mb-0">${e(s.synopsis)}</p></div>
          <aside class="mv-action-panel">
            <div class="mv-score"><i class="fa-solid fa-star fa-xl" style="color:#f3ce55"></i><div><strong id="tvAvgRating">${MV.media.rating(s.averageRating)}</strong><div class="mv-score-label">MovieVerse rating · ${s.reviewCount} ratings</div></div></div>
            <div class="mv-score"><i class="fa-regular fa-star fa-xl" style="color:var(--accent-hover)"></i><div><strong id="tvYourRating">—</strong><div class="mv-score-label">Your rating</div></div></div>
            <button class="btn btn-primary w-100 mb-2" id="rateTv"><i class="fa-regular fa-star me-2"></i>Rate</button>
            <button class="btn btn-secondary w-100 mb-2" id="watchlistTv"><i class="fa-regular fa-bookmark me-2"></i><span>Watchlist</span></button>
            <button class="btn btn-secondary w-100" id="watchedTv"><i class="fa-regular fa-circle-check me-2"></i><span>Watched</span></button>
          </aside>
        </div>
      </div></section>
      <main class="mv-main"><div class="mv-container">
        <section class="mv-section"><h2 class="mv-section-title">Episodes</h2><div id="seasonPills" class="mv-season-pills"></div><div id="episodeList"></div></section>
        <section class="mv-section"><h2 class="mv-section-title">Cast</h2>${castHtml?`<div class="mv-cast-row">${castHtml}</div>`:MV.ui.emptyState({icon:"fa-users",title:"No cast listed yet"})}</section>
        <section class="mv-section"><div class="d-flex justify-content-between align-items-center gap-2 mb-3"><h2 class="mv-section-title mb-0">User Reviews</h2><button class="btn btn-outline-primary" id="writeTvReview"><i class="fa-regular fa-pen-to-square me-2"></i>Write review</button></div><div id="tvReviews">${MV.ui.skeletonLines(4)}</div></section>
        <section class="mv-section"><h2 class="mv-section-title">More Like This</h2><div id="tvSimilar" class="mv-card-row">${MV.ui.skeletonCards(6)}</div></section>
        ${s.storyline?`<section class="mv-section"><h2 class="mv-section-title">Storyline</h2><div class="mv-surface p-3"><p class="mb-0">${e(s.storyline)}</p></div></section>`:""}
        <section class="mv-section" id="tvDetailsSection"></section>
      </div></main>`;
    bindTopActions();renderAdditional(s);await Promise.all([refreshMyRating(),loadReviews(),loadSimilar(),loadSeasons(s.seasons||[])]);
  }

  function bindTopActions(){
    document.getElementById("rateTv").addEventListener("click",()=>MV.rating.open({type:"tvshow",id,title:show.title,onSaved:refreshAfterUserReview}));
    document.getElementById("writeTvReview").addEventListener("click",()=>MV.reviews.openEditor({type:"tvshow",id,title:show.title,onSaved:refreshAfterUserReview}));
    document.getElementById("watchlistTv").addEventListener("click",async()=>{if(!MV.auth.requireAuth())return;try{await MV.library.toggleWatchlist("TVShow",id);updateLibraryButtons();}catch(err){MV.ui.showError(err);}});
    document.getElementById("watchedTv").addEventListener("click",async()=>{if(!MV.auth.requireAuth())return;try{await MV.library.toggleWatched("TVShow",id);updateLibraryButtons();}catch(err){MV.ui.showError(err);}});
    updateLibraryButtons();
  }
  function updateLibraryButtons(){
    const w=document.getElementById("watchlistTv"),h=document.getElementById("watchedTv");
    if(w){const a=MV.library.isInWatchlist("TVShow",id);w.classList.toggle("btn-primary",a);w.classList.toggle("btn-secondary",!a);w.querySelector("i").className=`${a?"fa-solid":"fa-regular"} fa-bookmark me-2`;w.querySelector("span").textContent=a?"In watchlist":"Watchlist";}
    if(h){const a=MV.library.isWatched("TVShow",id);h.classList.toggle("btn-primary",a);h.classList.toggle("btn-secondary",!a);h.querySelector("i").className=`${a?"fa-solid":"fa-regular"} fa-circle-check me-2`;h.querySelector("span").textContent=a?"Watched":"Mark watched";}
  }
  async function refreshMyRating(){const el=document.getElementById("tvYourRating");if(!el)return;if(!MV.auth.isAuthenticated()){el.textContent="—";return;}try{const mine=await MV.reviews.getMine("tvshow",id);el.textContent=mine?Number(mine.rating).toFixed(1):"—";}catch{el.textContent="—";}}
  async function refreshAfterUserReview(){await Promise.all([refreshMyRating(),loadReviews()]);try{show=await MV.api.get(`tvshows/${id}`,null,{auth:false});document.getElementById("tvAvgRating").textContent=MV.media.rating(show.averageRating);}catch{}}
  async function loadReviews(){const host=document.getElementById("tvReviews");try{host.innerHTML=MV.reviews.cards(await MV.reviews.getAll("tvshow",id));}catch(err){host.innerHTML=MV.ui.emptyState({icon:"fa-message",title:"Reviews unavailable",text:err.detail||"Try again later."});}}
  async function loadSimilar(){const host=document.getElementById("tvSimilar");try{const rows=await MV.api.get(`tvshows/${id}/similar`,{limit:6},{auth:false});const items=(rows||[]).map(x=>x.tvShow).filter(Boolean);host.className=items.length?"mv-card-row":"";host.innerHTML=items.length?items.map(x=>MV.media.mediaCard(x,"TVShow",{showWatched:false})).join(""):MV.ui.emptyState({icon:"fa-wand-magic-sparkles",title:"No similar shows yet"});MV.media.bindCardActions(host);}catch(err){host.className="";host.innerHTML=MV.ui.emptyState({icon:"fa-wand-magic-sparkles",title:"Recommendations unavailable",text:err.detail||"Try again later."});}}

  async function loadSeasons(initial){
    const pills=document.getElementById("seasonPills"),list=document.getElementById("episodeList");
    let seasons=initial;
    try{seasons=await MV.api.get(`tvshows/${id}/seasons`,null,{auth:false});}catch{}
    seasons=[...(seasons||[])].sort((a,b)=>a.seasonNumber-b.seasonNumber);
    if(!seasons.length){pills.innerHTML="";list.innerHTML=MV.ui.emptyState({icon:"fa-layer-group",title:"No seasons yet",text:"Episodes will appear here once seasons are added."});return;}
    pills.innerHTML=seasons.map(s=>`<button class="btn btn-secondary mv-season-pill" data-season-id="${s.id}" data-season-number="${s.seasonNumber}">Season ${s.seasonNumber}</button>`).join("");
    pills.addEventListener("click",event=>{const b=event.target.closest("[data-season-id]");if(b)selectSeason(b.dataset.seasonId,Number(b.dataset.seasonNumber));});
    const requestedNumber=Number(MV.ui.getParam("season"));const chosen=seasons.find(s=>s.seasonNumber===requestedNumber)||seasons[0];await selectSeason(chosen.id,chosen.seasonNumber);
  }
  async function selectSeason(seasonId,number){
    document.querySelectorAll(".mv-season-pill").forEach(b=>b.classList.toggle("active",b.dataset.seasonId===seasonId));
    const list=document.getElementById("episodeList");list.innerHTML=`<div class="mt-3">${MV.ui.skeletonLines(5)}</div>`;
    try{const episodes=await MV.api.get(`seasons/${seasonId}/episodes`,null,{auth:false});renderEpisodes(episodes||[],number);const requested=MV.ui.getParam("episode");if(requested&&episodes.some(e=>e.id===requested))await openEpisode(requested,number,true);}catch(err){list.innerHTML=MV.ui.emptyState({icon:"fa-circle-exclamation",title:"Could not load episodes",text:err.detail||"Try again."});}
  }
  function renderEpisodes(episodes,seasonNumber){
    const list=document.getElementById("episodeList");
    if(!episodes.length){list.innerHTML=MV.ui.emptyState({icon:"fa-list-ol",title:"No episodes yet"});return;}
    list.innerHTML=`<div class="mv-episode-list">${[...episodes].sort((a,b)=>a.episodeNumber-b.episodeNumber).map(ep=>`<article class="mv-episode-card" id="episode-${ep.id}"><img src="${MV.media.imageUrl(ep.imageUrl)}" alt="${MV.ui.escapeHtml(ep.title)}"><div><div class="text-secondary small">S${seasonNumber} · E${ep.episodeNumber}</div><h4>${MV.ui.escapeHtml(ep.title)}</h4><div class="text-secondary small mb-1">${ep.releaseDate?MV.media.date(ep.releaseDate):""}${ep.runtimeMinutes?` · ${MV.media.runtime(ep.runtimeMinutes)}`:""} · <span class="mv-rating-inline"><i class="fa-solid fa-star"></i> ${MV.media.rating(ep.averageRating)}</span></div>${ep.description?`<p class="text-secondary mb-0">${MV.ui.escapeHtml(ep.description)}</p>`:""}</div><button class="btn btn-outline-primary js-expand-episode" data-episode-id="${ep.id}" data-season-number="${seasonNumber}">Details</button><div class="mv-episode-detail d-none" id="episode-detail-${ep.id}" style="grid-column:1/-1"></div></article>`).join("")}</div>`;
    list.querySelectorAll(".js-expand-episode").forEach(btn=>btn.addEventListener("click",()=>openEpisode(btn.dataset.episodeId,Number(btn.dataset.seasonNumber),false)));
  }
  async function openEpisode(episodeId,seasonNumber,deepLink){
    const panel=document.getElementById(`episode-detail-${episodeId}`);if(!panel)return;
    if(panel.dataset.loaded==="true"){panel.classList.toggle("d-none");if(!panel.classList.contains("d-none"))panel.scrollIntoView({behavior:deepLink?"auto":"smooth",block:"center"});return;}
    panel.classList.remove("d-none");panel.innerHTML=MV.ui.skeletonLines(5);
    try{
      const ep=await MV.api.get(`episodes/${episodeId}`,null,{auth:false});const cast=ep.cast?.length?await MV.media.castCards(ep.cast):"";
      panel.innerHTML=`<div class="d-flex flex-wrap justify-content-between gap-2 align-items-start"><div><div class="mv-kicker">S${ep.seasonNumber} · E${ep.episodeNumber}</div><h3 class="h5 mb-1">${MV.ui.escapeHtml(ep.title)}</h3><div class="text-secondary">${ep.releaseDate?MV.media.date(ep.releaseDate):""}${ep.runtimeMinutes?` · ${MV.media.runtime(ep.runtimeMinutes)}`:""} · <span class="mv-rating-inline"><i class="fa-solid fa-star"></i> <span id="epAvg-${ep.id}">${MV.media.rating(ep.averageRating)}</span></span> · ${ep.reviewCount} ratings</div></div><div class="d-flex gap-2"><button class="btn btn-sm btn-primary js-ep-rate"><i class="fa-regular fa-star me-1"></i>Rate</button><button class="btn btn-sm btn-outline-primary js-ep-review"><i class="fa-regular fa-pen-to-square me-1"></i>Review</button></div></div>${ep.description?`<p class="mt-3">${MV.ui.escapeHtml(ep.description)}</p>`:""}${ep.directors?.length?`<div class="mv-crew-line"><strong>Directors</strong> ${ep.directors.map(x=>`<a href="person-details.html?type=director&id=${x.id}">${MV.ui.escapeHtml(x.fullName)}</a>`).join(", ")}</div>`:""}${ep.writers?.length?`<div class="mv-crew-line"><strong>Writers</strong> ${ep.writers.map(x=>`<a href="person-details.html?type=writer&id=${x.id}">${MV.ui.escapeHtml(x.fullName)}</a>`).join(", ")}</div>`:""}${cast?`<h4 class="h6 mt-3">Cast</h4><div class="mv-cast-row">${cast}</div>`:""}<div class="mt-3"><h4 class="h6">Written reviews</h4><div id="epReviews-${ep.id}">${MV.ui.skeletonLines(2)}</div></div>`;
      panel.dataset.loaded="true";
      panel.querySelector(".js-ep-rate").addEventListener("click",()=>MV.rating.open({type:"episode",id:ep.id,title:`${show.title}: ${ep.title}`,onSaved:()=>refreshEpisode(ep.id)}));
      panel.querySelector(".js-ep-review").addEventListener("click",()=>MV.reviews.openEditor({type:"episode",id:ep.id,title:`${show.title}: ${ep.title}`,onSaved:()=>refreshEpisode(ep.id)}));
      await loadEpisodeReviews(ep.id); if(deepLink) panel.scrollIntoView({block:"center"});
    }catch(err){panel.innerHTML=MV.ui.emptyState({icon:"fa-circle-exclamation",title:"Episode details unavailable",text:err.detail||"Try again."});}
  }
  async function loadEpisodeReviews(epId){const host=document.getElementById(`epReviews-${epId}`);if(!host)return;try{host.innerHTML=MV.reviews.cards(await MV.reviews.getAll("episode",epId));}catch(err){host.innerHTML=`<span class="text-secondary">${MV.ui.escapeHtml(err.detail||"Reviews unavailable")}</span>`;}}
  async function refreshEpisode(epId){await loadEpisodeReviews(epId);try{const ep=await MV.api.get(`episodes/${epId}`,null,{auth:false});const avg=document.getElementById(`epAvg-${epId}`);if(avg)avg.textContent=MV.media.rating(ep.averageRating);}catch{}}

  function renderAdditional(s){
    const rows=[["Release date",MV.media.date(s.releaseDate)],["End date",MV.media.date(s.endDate)],["Original language",s.originalLanguage],["Country of origin",s.countryOfOrigin],["Production company",s.productionCompany],["Color",s.color]].filter(([,v])=>v&&String(v).trim());
    const host=document.getElementById("tvDetailsSection");if(!rows.length&&!s.trivia)return;
    host.innerHTML=`<h2 class="mv-section-title">Details</h2><dl class="mv-details-list mv-surface px-3">${rows.map(([k,v])=>`<div class="row"><dt class="col-sm-4">${MV.ui.escapeHtml(k)}</dt><dd class="col-sm-8">${MV.ui.escapeHtml(v)}</dd></div>`).join("")}</dl>${s.trivia?`<h2 class="mv-section-title mt-4">Did you know?</h2><div class="mv-surface p-3"><strong>Trivia</strong><p class="text-secondary mt-2 mb-0">${MV.ui.escapeHtml(s.trivia)}</p></div>`:""}`;
  }
});
