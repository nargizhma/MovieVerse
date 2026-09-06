document.addEventListener("DOMContentLoaded", async () => {
  const recent = document.getElementById("recentReleases");
  const top = document.getElementById("topMovies");
  if (!recent || !top) return;
  recent.innerHTML = MV.ui.skeletonCards(7); top.innerHTML = MV.ui.skeletonCards(5);
  try { await MV.library.load(); } catch { }
  MV.media.bindCardActions(document);

  try {
    const [movies,tv] = await Promise.all([
      MV.api.get("movies",{SortBy:"year",SortDescending:true,PageNumber:1,PageSize:12},{auth:false}),
      MV.api.get("tvshows",{SortBy:"year",SortDescending:true,PageNumber:1,PageSize:12},{auth:false})
    ]);
    const merged = [
      ...(movies.items||[]).map(x=>({...x,__type:"Movie"})),
      ...(tv.items||[]).map(x=>({...x,__type:"TVShow"}))
    ].sort((a,b)=>new Date(b.releaseDate)-new Date(a.releaseDate)).slice(0,16);
    recent.className = "mv-card-row";
    recent.innerHTML = merged.length ? merged.map(x=>MV.media.mediaCard(x,x.__type)).join("") : MV.ui.emptyState({icon:"fa-clapperboard",title:"No releases yet",text:"New movies and TV shows will appear here when the catalog has content."});
  } catch(err) { recent.className=""; recent.innerHTML=MV.ui.emptyState({icon:"fa-plug-circle-xmark",title:"Could not load releases",text:err.detail||"The API is unavailable."}); }

  const currentYear = new Date().getFullYear();
  document.getElementById("topYear").textContent = currentYear;
  try {
    const data = await MV.api.get("movies",{ReleaseYear:currentYear,SortBy:"rating",SortDescending:true,PageNumber:1,PageSize:5},{auth:false});
    const items = data.items || [];
    top.className = items.length ? "mv-top5" : "";
    top.innerHTML = items.length ? items.map((m,i)=>`<div class="mv-top-item"><span class="mv-rank">${i+1}</span>${MV.media.mediaCard(m,"Movie",{showWatched:false})}</div>`).join("") : MV.ui.emptyState({icon:"fa-ranking-star",title:`No rated movies from ${currentYear} yet`,text:"This ranking will fill automatically as this year's catalog is added and rated."});
  } catch(err) { top.className=""; top.innerHTML=MV.ui.emptyState({icon:"fa-plug-circle-xmark",title:"Could not load Top 5",text:err.detail||"The API is unavailable."}); }
});
