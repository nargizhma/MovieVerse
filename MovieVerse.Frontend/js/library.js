window.MV = window.MV || {};
MV.library = (() => {
  let loaded = false, loading = null;
  let watchlist = new Map(), history = new Map();
  const key = (type,id) => `${String(type).toLowerCase()}:${id}`;
  const apiSegment = type => String(type).toLowerCase() === "movie" ? "movies" : "tvshows";

  async function load(force = false) {
    if (!MV.auth.isAuthenticated()) { reset(); return; }
    if (loaded && !force) return;
    if (loading && !force) return loading;
    loading = Promise.all([MV.api.get("watchlist"), MV.api.get("watch-history")]).then(([w,h]) => {
      watchlist = new Map((w || []).map(item => [key(item.contentType,item.contentId),item]));
      history = new Map((h || []).map(item => [key(item.contentType,item.contentId),item]));
      loaded = true;
      document.dispatchEvent(new CustomEvent("mv:library-loaded"));
    }).finally(() => { loading = null; });
    return loading;
  }
  function reset() { loaded=false; loading=null; watchlist=new Map(); history=new Map(); }
  function isInWatchlist(type,id) { return watchlist.has(key(type,id)); }
  function isWatched(type,id) { return history.has(key(type,id)); }
  function watchlistItems() { return [...watchlist.values()]; }
  function historyItems() { return [...history.values()]; }

  async function toggleWatchlist(type,id) {
    await load(); const active=isInWatchlist(type,id), path=`watchlist/${apiSegment(type)}/${id}`;
    if (active) { await MV.api.delete(path); watchlist.delete(key(type,id)); MV.ui.toast("Removed from watchlist","success"); }
    else { await MV.api.post(path); watchlist.set(key(type,id),{contentType:type,contentId:id}); MV.ui.toast("Added to watchlist","success"); }
    document.dispatchEvent(new CustomEvent("mv:library-change",{detail:{kind:"watchlist",type,id,active:!active}})); return !active;
  }
  async function toggleWatched(type,id) {
    await load(); const active=isWatched(type,id), path=`watch-history/${apiSegment(type)}/${id}`;
    if (active) { await MV.api.delete(path); history.delete(key(type,id)); MV.ui.toast("Removed from watched","success"); }
    else { await MV.api.post(path); history.set(key(type,id),{contentType:type,contentId:id}); MV.ui.toast("Marked as watched","success"); }
    document.dispatchEvent(new CustomEvent("mv:library-change",{detail:{kind:"history",type,id,active:!active}})); return !active;
  }

  return { load, reset, isInWatchlist, isWatched, watchlistItems, historyItems, toggleWatchlist, toggleWatched };
})();
