window.MV = window.MV || {};

MV.rating = (() => {
  let state = null;

  function ensureModal() {
    if (document.getElementById("mvRatingModal")) return;
    document.body.insertAdjacentHTML("beforeend", `
      <div class="modal fade" id="mvRatingModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg"><div class="modal-content">
          <div class="modal-header"><div><div class="mv-kicker">Rate this title</div><h5 class="modal-title" id="mvRatingTitle">Your rating</h5></div><button class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
          <div class="modal-body">
            <div class="mv-rating-value"><span id="mvRatingNumber">8.0</span> <span class="text-secondary fs-5">/ 10</span></div>
            <div class="mv-rating-stars-scroll"><div id="mvRatingStars" class="mv-rating-stars" tabindex="0" role="slider" aria-label="Rating from 1 to 10" aria-valuemin="1" aria-valuemax="10" aria-valuenow="8.0"></div></div>
            <p class="text-center text-secondary mb-0">Move across the stars for one-decimal precision, then click or tap to choose.</p>
          </div>
          <div class="modal-footer"><button class="btn btn-outline-danger me-auto d-none" id="mvRemoveRating">Remove rating</button><button class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button><button class="btn btn-primary" id="mvSaveRating">Save rating</button></div>
        </div></div>
      </div>`);
    const stars = document.getElementById("mvRatingStars");
    stars.innerHTML = Array.from({length:10},()=>'<span class="mv-rating-star" aria-hidden="true"><i class="fa-regular fa-star"></i><span class="filled"><i class="fa-solid fa-star"></i></span></span>').join("");
    stars.addEventListener("pointermove",pointerMove);
    stars.addEventListener("pointerdown",pointerChoose);
    stars.addEventListener("keydown",keyChoose);
    stars.addEventListener("mouseleave",()=>{ if(state) renderValue(state.value); });
  }

  function valueFromPointer(event) {
    const stars = document.getElementById("mvRatingStars");
    const rect = stars.getBoundingClientRect();
    const x = Math.max(0, Math.min(rect.width, event.clientX - rect.left));
    return Math.max(1, Math.min(10, Math.round((x / rect.width) * 100) / 10));
  }
  function pointerMove(event) { if (!state) return; renderValue(valueFromPointer(event),true); }
  function pointerChoose(event) { if (!state) return; event.preventDefault(); state.value = valueFromPointer(event); renderValue(state.value); document.getElementById("mvRatingStars").setPointerCapture?.(event.pointerId); }
  function keyChoose(event) {
    if (!state) return;
    let next = state.value;
    if (["ArrowRight","ArrowUp"].includes(event.key)) next += .1;
    else if (["ArrowLeft","ArrowDown"].includes(event.key)) next -= .1;
    else if (event.key === "Home") next = 1;
    else if (event.key === "End") next = 10;
    else return;
    event.preventDefault(); state.value = Math.max(1,Math.min(10,Math.round(next*10)/10)); renderValue(state.value);
  }
  function renderValue(value, preview = false) {
    const v = Math.max(1,Math.min(10,Number(value || 1)));
    document.getElementById("mvRatingNumber").textContent = v.toFixed(1);
    const stars = [...document.querySelectorAll("#mvRatingStars .mv-rating-star")];
    stars.forEach((star,index)=>{
      const fill = Math.max(0,Math.min(1,v-index))*100;
      star.querySelector(".filled").style.setProperty("--fill",`${fill}%`);
      star.querySelector(".filled").style.width = `${fill}%`;
    });
    const control = document.getElementById("mvRatingStars");
    control.setAttribute("aria-valuenow",v.toFixed(1));
    control.setAttribute("aria-valuetext",`${v.toFixed(1)} out of 10${preview ? " preview" : ""}`);
  }

  async function open({type,id,title,onSaved}={}) {
    if (!MV.auth.requireAuth()) return;
    ensureModal();
    let existing = null;
    try { existing = await MV.reviews.getMine(type,id); } catch(err){ MV.ui.showError(err); return; }
    state = { type,id,title,onSaved,existing,value:Number(existing?.rating ?? 8) };
    document.getElementById("mvRatingTitle").textContent = title || "Your rating";
    document.getElementById("mvRemoveRating").classList.toggle("d-none",!existing);
    renderValue(state.value);
    const modalEl = document.getElementById("mvRatingModal");
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const saveBtn = document.getElementById("mvSaveRating");
    const removeBtn = document.getElementById("mvRemoveRating");

    const save = async () => {
      MV.ui.buttonBusy(saveBtn,true,"Saving…");
      try {
        await MV.reviews.save(type,id,state.value,existing?.content ?? null,existing);
        MV.ui.toast("Rating saved","success"); modal.hide(); if(onSaved) await onSaved(state.value);
      } catch(err){ MV.ui.showError(err); } finally { MV.ui.buttonBusy(saveBtn,false); }
    };
    const remove = async () => {
      const suffix = existing?.content ? " This will also remove your written review." : "";
      const ok = await MV.ui.confirm({title:"Remove rating?",message:`This removes your entire rating record.${suffix}`,confirmText:"Remove rating"});
      if (!ok) return;
      MV.ui.buttonBusy(removeBtn,true,"Removing…");
      try { await MV.reviews.removeEntire(type,id); MV.ui.toast("Rating removed","success"); modal.hide(); if(onSaved) await onSaved(null); }
      catch(err){ MV.ui.showError(err); } finally { MV.ui.buttonBusy(removeBtn,false); }
    };
    saveBtn.addEventListener("click",save);
    removeBtn.addEventListener("click",remove);
    modalEl.addEventListener("hidden.bs.modal",()=>{ saveBtn.removeEventListener("click",save); removeBtn.removeEventListener("click",remove); state=null; },{once:true});
    modal.show(); setTimeout(()=>document.getElementById("mvRatingStars")?.focus(),250);
  }

  return { open };
})();
