window.MV=window.MV||{};MV.admin=MV.admin||{};

MV.admin.common=(()=>{
  function init(){
    if(!MV.auth.requireAdmin())return false;
    const side=document.getElementById("adminSidebar"),top=document.getElementById("adminTopbar");
    const page=location.pathname.split("/").pop();
    if(side)side.innerHTML=`<div class="brand"><a href="../index.html"><img src="../assets/images/logo-horizontal.png" alt="MovieVerse"></a></div><nav class="mv-admin-nav">
      ${link("dashboard.html","fa-gauge-high","Dashboard",page)}${link("movies.html","fa-film","Movies",page)}${link("tvshows.html","fa-tv","TV Shows",page)}${link("actors.html","fa-masks-theater","Actors",page)}${link("directors.html","fa-video","Directors",page)}${link("writers.html","fa-pen-nib","Writers",page)}${link("genres.html","fa-tags","Genres",page)}${link("reviews.html","fa-comments","Reviews",page)}${link("users.html","fa-users-gear","Users",page)}
      <hr class="border-secondary"><a href="../index.html"><i class="fa-solid fa-arrow-left"></i>Public site</a>
    </nav>`;
    if(top){const u=MV.auth.getUser();top.innerHTML=`<div class="d-flex align-items-center gap-2"><button class="btn btn-secondary btn-sm mv-sidebar-toggle" id="adminSidebarToggle" aria-label="Toggle admin sidebar"><i class="fa-solid fa-bars"></i></button><strong>Administration</strong></div><div class="d-flex align-items-center gap-2"><span class="badge mv-badge">${MV.ui.escapeHtml((u?.roles||[]).join(", "))}</span><span class="text-secondary d-none d-sm-inline">${MV.ui.escapeHtml(u?.userName||"")}</span></div>`;}
    document.getElementById("adminSidebarToggle")?.addEventListener("click",()=>document.getElementById("adminSidebar")?.classList.toggle("open"));
    document.addEventListener("click",e=>{const s=document.getElementById("adminSidebar");if(innerWidth<992&&s?.classList.contains("open")&&!s.contains(e.target)&&!e.target.closest("#adminSidebarToggle"))s.classList.remove("open");});
    return true;
  }
  function link(href,icon,label,page){const is=page===href||(href==="movies.html"&&page==="movie-form.html")||(href==="tvshows.html"&&["tvshow-form.html","episode-form.html"].includes(page))||(href==="actors.html"&&page==="person-form.html"&&new URLSearchParams(location.search).get("type")==="actor")||(href==="directors.html"&&page==="person-form.html"&&new URLSearchParams(location.search).get("type")==="director")||(href==="writers.html"&&page==="person-form.html"&&new URLSearchParams(location.search).get("type")==="writer");return`<a class="${is?"active":""}" href="${href}"><i class="fa-solid ${icon}"></i>${label}</a>`;}

  function validateImage(file,label="Image"){if(!file)return true;if(!file.type.startsWith("image/")){MV.ui.toast(`${label} must be an image file.`,"warning");return false;}if(file.size>5*1024*1024){MV.ui.toast(`${label} cannot exceed 5 MB.`,"warning");return false;}return true;}
  function bindImagePreview(input,preview,{label="Image"}={}){if(!input||!preview)return;input.addEventListener("change",()=>{const f=input.files?.[0];if(!f)return;if(!validateImage(f,label)){input.value="";return;}preview.src=URL.createObjectURL(f);});}
  function addList(fd,name,values){(values||[]).forEach((v,i)=>fd.append(`${name}[${i}]`,v));}
  function addCast(fd,name,values){(values||[]).forEach((v,i)=>{fd.append(`${name}[${i}].ActorId`,v.actorId);fd.append(`${name}[${i}].CharacterName`,v.characterName||"");fd.append(`${name}[${i}].CastOrder`,Number(v.castOrder)||0);});}
  function appendNullable(fd,name,value){if(value!==undefined&&value!==null&&String(value)!=="")fd.append(name,value);}
  function validateCast(values){for(const item of values||[]){if(!Number.isInteger(Number(item.castOrder))||Number(item.castOrder)<0){MV.ui.toast("Cast order must be a non-negative whole number.","warning");return false;}if((item.characterName||"").length>200){MV.ui.toast("Character name cannot exceed 200 characters.","warning");return false;}}return true;}
  function dateInput(value){if(!value)return"";const d=new Date(value);return Number.isNaN(d.getTime())?"":d.toISOString().slice(0,10);}

  class RelationshipPicker{
    constructor(host,items,{cast=false,placeholder="Search…"}={}){this.host=typeof host==="string"?document.getElementById(host):host;this.items=items||[];this.cast=cast;this.selected=[];this.placeholder=placeholder;this.renderBase();}
    renderBase(){this.host.innerHTML=`<div class="mv-picker"><input class="form-control js-picker-search" type="search" placeholder="${MV.ui.escapeHtml(this.placeholder)}" autocomplete="off"><div class="mv-picker-results d-none js-picker-results"></div></div><div class="mv-selected-list js-selected-list"></div>`;this.search=this.host.querySelector(".js-picker-search");this.results=this.host.querySelector(".js-picker-results");this.list=this.host.querySelector(".js-selected-list");this.search.addEventListener("input",()=>this.renderResults());this.search.addEventListener("focus",()=>this.renderResults());this.results.addEventListener("click",e=>{const o=e.target.closest("[data-picker-id]");if(o)this.add(o.dataset.pickerId);});this.list.addEventListener("click",e=>{const b=e.target.closest("[data-remove-id]");if(b)this.remove(b.dataset.removeId);});document.addEventListener("click",e=>{if(!this.host.contains(e.target))this.results.classList.add("d-none");});}
    itemId(item){return String(item.id);}
    itemName(item){return item.fullName||item.name||item.title||"Unknown";}
    renderResults(){const q=this.search.value.trim().toLowerCase();const ids=new Set(this.selected.map(x=>String(x.id)));const hits=this.items.filter(x=>!ids.has(this.itemId(x))&&(!q||this.itemName(x).toLowerCase().includes(q))).slice(0,10);this.results.innerHTML=hits.length?hits.map(x=>`<div class="mv-picker-option" data-picker-id="${this.itemId(x)}">${MV.ui.escapeHtml(this.itemName(x))}</div>`).join(""):'<div class="p-2 text-secondary small">No matches</div>';this.results.classList.remove("d-none");}
    add(id,data={}){if(this.selected.some(x=>String(x.id)===String(id)))return;const item=this.items.find(x=>String(x.id)===String(id));if(!item)return;this.selected.push({id:item.id,name:this.itemName(item),characterName:data.characterName||"",castOrder:data.castOrder??this.selected.length});this.search.value="";this.results.classList.add("d-none");this.renderSelected();}
    remove(id){this.selected=this.selected.filter(x=>String(x.id)!==String(id));this.renderSelected();}
    setSelected(values){this.selected=[];(values||[]).forEach((v,i)=>{const id=typeof v==="string"?v:(v.actorId||v.id);const item=this.items.find(x=>String(x.id)===String(id));if(item)this.selected.push({id:item.id,name:this.itemName(item),characterName:v.characterName||"",castOrder:v.castOrder??i});});this.renderSelected();}
    renderSelected(){if(!this.selected.length){this.list.innerHTML='<div class="text-secondary small">Nothing selected.</div>';return;}this.list.innerHTML=this.selected.map(x=>this.cast?`<div class="mv-selected-item" data-id="${x.id}"><strong>${MV.ui.escapeHtml(x.name)}</strong><input class="form-control form-control-sm js-character" maxlength="200" value="${MV.ui.escapeHtml(x.characterName)}" placeholder="Character name" aria-label="Character name for ${MV.ui.escapeHtml(x.name)}"><input class="form-control form-control-sm js-order" type="number" min="0" value="${Number(x.castOrder)||0}" aria-label="Cast order for ${MV.ui.escapeHtml(x.name)}"><button class="btn btn-sm btn-outline-danger" type="button" data-remove-id="${x.id}" aria-label="Remove ${MV.ui.escapeHtml(x.name)} from selection"><i class="fa-solid fa-xmark"></i></button></div>`:`<div class="mv-selected-chip"><span>${MV.ui.escapeHtml(x.name)}</span><button class="btn btn-sm btn-link text-danger p-0" type="button" data-remove-id="${x.id}" aria-label="Remove ${MV.ui.escapeHtml(x.name)}"><i class="fa-solid fa-xmark"></i></button></div>`).join("");}
    ids(){return this.selected.map(x=>x.id);}
    castValues(){if(!this.cast)return[];return this.selected.map(x=>{const row=this.list.querySelector(`[data-id="${CSS.escape(String(x.id))}"]`);return{actorId:x.id,characterName:row?.querySelector(".js-character")?.value.trim()||null,castOrder:Number(row?.querySelector(".js-order")?.value||0)};});}
  }

  document.addEventListener("DOMContentLoaded",init);
  return{init,validateImage,bindImagePreview,addList,addCast,appendNullable,validateCast,dateInput,RelationshipPicker};
})();
