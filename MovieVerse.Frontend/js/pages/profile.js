document.addEventListener("DOMContentLoaded", async () => {
  if(!MV.auth.requireAuth("profile.html"))return;
  const root=document.getElementById("profilePage"); root.innerHTML=`<main class="mv-main"><div class="mv-container">${MV.ui.skeletonLines(7)}</div></main>`;
  let profile,activity=[];
  try{
    [profile,activity]=await Promise.all([MV.api.get("profiles/me"),MV.api.get("profiles/me/activity"),MV.library.load().then(()=>null)]).then(x=>[x[0],x[1]]);
    renderShell(); renderAll(); activateHash();
  }catch(err){root.innerHTML=`<main class="mv-main"><div class="mv-container">${MV.ui.emptyState({icon:"fa-user",title:"Could not load profile",text:err.detail||"Try signing in again."})}</div></main>`;}

  function renderShell(){
    const e=MV.ui.escapeHtml;
    root.innerHTML=`<main class="mv-main"><div class="mv-container">
      <div class="mv-profile-head mb-4"><img class="mv-profile-photo" id="profilePhoto" src="${MV.media.imageUrl(profile.profileImageUrl)}" alt="Profile photo"><div><div class="mv-kicker">Your profile</div><h1 class="mv-page-title mb-1">${e(profile.displayName||profile.userName)}</h1><div class="text-secondary">@${e(profile.userName)} · ${e(profile.email)}</div>${profile.bio?`<p class="mt-2 mb-0">${e(profile.bio)}</p>`:""}</div></div>
      <ul class="nav nav-tabs flex-nowrap overflow-auto" id="profileTabs" role="tablist">
        ${tab("overview","Overview",true)}${tab("watchlist","Watchlist")}${tab("watched","Watched")}${tab("ratings","Ratings")}${tab("reviews","Reviews")}${tab("edit-profile","Edit Profile")}
      </ul>
      <div class="tab-content mv-surface p-3 border-top-0 rounded-top-0" id="profileTabContent">
        <div class="tab-pane fade show active" id="overview-pane"><div id="profileOverview"></div></div>
        <div class="tab-pane fade" id="watchlist-pane"><div id="profileWatchlist"></div></div>
        <div class="tab-pane fade" id="watched-pane"><div id="profileWatched"></div></div>
        <div class="tab-pane fade" id="ratings-pane"><div id="profileRatings"></div></div>
        <div class="tab-pane fade" id="reviews-pane"><div id="profileReviews"></div></div>
        <div class="tab-pane fade" id="edit-profile-pane"><div id="profileEdit"></div></div>
      </div>
    </div></main>`;
    document.getElementById("profileTabs").addEventListener("shown.bs.tab",event=>{history.replaceState(null,"",`#${event.target.dataset.tab}`);});
  }
  function tab(name,label,active=false){return `<li class="nav-item" role="presentation"><button class="nav-link ${active?"active":""}" data-bs-toggle="tab" data-bs-target="#${name}-pane" data-tab="${name}" type="button">${label}</button></li>`;}
  function renderAll(){renderOverview();renderLibrary("watchlist");renderLibrary("watched");renderActivity("ratings");renderActivity("reviews");renderEdit();}
  function renderOverview(){document.getElementById("profileOverview").innerHTML=`<div class="mv-stat-grid">${stat(profile.totalReviewCount,"Rating records")}${stat(profile.movieReviewCount,"Movie ratings")}${stat(profile.tvShowReviewCount,"TV ratings")}${stat(profile.episodeReviewCount,"Episode ratings")}${stat(profile.watchlistCount,"Watchlist")}${stat(profile.watchHistoryCount,"Watched")}</div>${profile.bio?`<div class="mv-section"><h2 class="mv-section-title">About</h2><p class="mb-0">${MV.ui.escapeHtml(profile.bio)}</p></div>`:""}`;}
  function stat(v,l){return `<div class="mv-stat"><strong>${Number(v||0)}</strong><span>${MV.ui.escapeHtml(l)}</span></div>`;}

  function renderLibrary(kind){
    const host=document.getElementById(kind==="watchlist"?"profileWatchlist":"profileWatched"); const items=kind==="watchlist"?MV.library.watchlistItems():MV.library.historyItems();
    if(!items.length){host.innerHTML=MV.ui.emptyState({icon:kind==="watchlist"?"fa-bookmark":"fa-circle-check",title:kind==="watchlist"?"Your watchlist is empty":"No watched titles yet",text:"Browse MovieVerse to add titles.",actionText:"Browse titles",actionHref:"search.html"});return;}
    host.innerHTML=`<div class="mv-media-grid">${items.map(item=>MV.media.mediaCard({...item,id:item.contentId,genres:[]},item.contentType,{showWatched:true})).join("")}</div>`;
    MV.media.bindCardActions(host);
    if(!host.dataset.libraryRefreshBound){
      host.dataset.libraryRefreshBound="true";
      host.addEventListener("click",event=>{
        const btn=event.target.closest(kind==="watchlist"?".js-watchlist":".js-watched");if(!btn)return;
        setTimeout(()=>{renderLibrary(kind);refreshCounts();},80);
      });
    }
  }

  function renderActivity(kind){
    const host=document.getElementById(kind==="ratings"?"profileRatings":"profileReviews"); const items=kind==="reviews"?activity.filter(a=>a.content&&String(a.content).trim()):activity;
    if(!items.length){host.innerHTML=MV.ui.emptyState({icon:kind==="ratings"?"fa-star":"fa-message",title:kind==="ratings"?"No ratings yet":"No written reviews yet",text:kind==="ratings"?"Rate a movie, TV show, or episode to see it here.":"Written reviews you post will appear here."});return;}
    host.innerHTML=`<div class="mv-activity-list">${items.map(a=>activityItem(a,kind)).join("")}</div>`;
    host.querySelectorAll(".js-edit-activity").forEach(btn=>btn.addEventListener("click",()=>{
      const item=activity.find(x=>x.reviewId===btn.dataset.reviewId);if(item)MV.reviews.openEditor({type:item.contentType,id:item.contentId,title:item.title,onSaved:reloadActivity});
    }));
    host.querySelectorAll(".js-delete-written").forEach(btn=>btn.addEventListener("click",async()=>{
      const item=activity.find(x=>x.reviewId===btn.dataset.reviewId);if(!item)return;const ok=await MV.ui.confirm({title:"Delete written review?",message:"Your numeric rating will stay saved.",confirmText:"Delete review"});if(!ok)return;
      try{await MV.reviews.deleteWrittenOnly(item.contentType,item.contentId,{rating:item.rating,content:item.content});MV.ui.toast("Written review deleted; rating kept","success");await reloadActivity();}catch(err){MV.ui.showError(err);}
    }));
  }
  function activityItem(a,kind){const href=activityHref(a);return `<article class="mv-activity-item"><a href="${href}"><img src="${MV.media.imageUrl(a.imageUrl)}" alt=""></a><div><a href="${href}" class="fw-bold text-light">${MV.ui.escapeHtml(a.title)}</a>${a.parentTitle?`<div class="text-secondary small">${MV.ui.escapeHtml(a.parentTitle)}${a.seasonNumber?` · S${a.seasonNumber}E${a.episodeNumber}`:""}</div>`:""}<div class="mv-review-rating mt-1"><i class="fa-solid fa-star"></i> ${Number(a.rating).toFixed(1)}/10</div>${kind==="reviews"?`<p class="text-secondary mt-2 mb-0">${MV.ui.escapeHtml(a.content)}</p>`:""}</div><div class="d-flex flex-column gap-2"><button class="btn btn-sm btn-outline-primary js-edit-activity" data-review-id="${a.reviewId}">${kind==="reviews"?"Edit":"Change rating"}</button>${kind==="reviews"?`<button class="btn btn-sm btn-outline-danger js-delete-written" data-review-id="${a.reviewId}">Delete review</button>`:""}</div></article>`;}
  function activityHref(a){const t=String(a.contentType).toLowerCase();if(t==="movie")return`movie-details.html?id=${a.contentId}`;if(t==="tvshow")return`tvshow-details.html?id=${a.contentId}`;return`tvshow-details.html?id=${a.tvShowId}&season=${a.seasonNumber||""}&episode=${a.contentId}`;}
  async function reloadActivity(){try{activity=await MV.api.get("profiles/me/activity");renderActivity("ratings");renderActivity("reviews");profile=await MV.api.get("profiles/me");renderOverview();}catch(err){MV.ui.showError(err);}}
  async function refreshCounts(){try{profile=await MV.api.get("profiles/me");renderOverview();}catch{}}

  function renderEdit(){
    const host=document.getElementById("profileEdit");host.innerHTML=`<form id="profileEditForm" class="row g-3" enctype="multipart/form-data"><div class="col-md-6"><label class="form-label" for="profileDisplayName">Display name</label><input class="form-control" id="profileDisplayName" name="DisplayName" maxlength="50" value="${MV.ui.escapeHtml(profile.displayName||"")}"></div><div class="col-12"><label class="form-label" for="profileBio">Bio</label><textarea class="form-control" id="profileBio" name="Bio" maxlength="500" rows="5">${MV.ui.escapeHtml(profile.bio||"")}</textarea></div><div class="col-md-7"><label class="form-label" for="profileImageInput">Profile image</label><input class="form-control" id="profileImageInput" name="ProfileImage" type="file" accept="image/*"><div class="form-text text-secondary">Image files only, maximum 5 MB.</div></div><div class="col-md-5"><img id="profileEditPreview" class="mv-image-preview" src="${MV.media.imageUrl(profile.profileImageUrl)}" alt="Profile image preview"></div><div class="col-12 d-flex flex-wrap gap-2"><button class="btn btn-primary" id="saveProfile" type="submit">Save profile</button>${profile.profileImageUrl?'<button class="btn btn-outline-danger" id="deleteProfileImage" type="button">Remove profile image</button>':""}</div></form>`;
    const input=document.getElementById("profileImageInput");input.addEventListener("change",()=>{const f=input.files?.[0];if(f){if(!validateImage(f)){input.value="";return;}document.getElementById("profileEditPreview").src=URL.createObjectURL(f);}});
    const editForm=document.getElementById("profileEditForm");
    if(window.jQuery?.fn?.validate){
      $(editForm).validate({rules:{DisplayName:{maxlength:50},Bio:{maxlength:500}},messages:{DisplayName:{maxlength:"Display name cannot exceed 50 characters."},Bio:{maxlength:"Bio cannot exceed 500 characters."}},submitHandler:formEl=>saveProfile(formEl)});
    }else{ editForm.addEventListener("submit",saveProfile); }
    document.getElementById("deleteProfileImage")?.addEventListener("click",deleteImage);
  }
  function validateImage(file){if(!file.type.startsWith("image/")){MV.ui.toast("Please choose an image file.","warning");return false;}if(file.size>5*1024*1024){MV.ui.toast("Profile image cannot exceed 5 MB.","warning");return false;}return true;}
  async function saveProfile(eventOrForm){eventOrForm.preventDefault?.();const btn=document.getElementById("saveProfile"),form=eventOrForm.currentTarget||eventOrForm,fd=new FormData(form);MV.ui.buttonBusy(btn,true,"Saving…");try{await MV.api.put("profiles/me",fd);MV.ui.toast("Profile updated","success");profile=await MV.api.get("profiles/me");renderShell();renderAll();activateHash();}catch(err){MV.ui.showError(err);}finally{MV.ui.buttonBusy(btn,false);}}
  async function deleteImage(){const ok=await MV.ui.confirm({title:"Remove profile image?",message:"Your current profile image will be deleted.",confirmText:"Remove image"});if(!ok)return;try{await MV.api.delete("profiles/me/image");MV.ui.toast("Profile image removed","success");profile=await MV.api.get("profiles/me");renderShell();renderAll();activateHash();}catch(err){MV.ui.showError(err);}}
  function activateHash(){const target=(location.hash||"#overview").slice(1);const button=document.querySelector(`[data-tab="${CSS.escape(target)}"]`);if(button)bootstrap.Tab.getOrCreateInstance(button).show();}
});
