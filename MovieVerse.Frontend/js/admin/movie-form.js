document.addEventListener("DOMContentLoaded",async()=>{
  if(!MV.auth.isAdmin())return;
  const id=new URLSearchParams(location.search).get("id"),form=document.getElementById("movieForm"),saveBtn=document.getElementById("saveMovie");
  const titleEl=document.getElementById("movieFormTitle");titleEl.textContent=id?"Edit Movie":"Create Movie";
  let genres=[],actors=[],directors=[],writers=[],movie=null,genrePicker,actorPicker,directorPicker,writerPicker;
  try{
    const req=[MV.api.get("genres"),MV.api.get("actors"),MV.api.get("directors"),MV.api.get("writers")];if(id)req.push(MV.api.get(`movies/${id}`));
    const data=await Promise.all(req);[genres,actors,directors,writers]=data;movie=id?data[4]:null;setupPickers();if(movie)fill(movie);setupValidation();
  }catch(err){MV.ui.showError(err);form.querySelectorAll("input,textarea,button,select").forEach(x=>x.disabled=true);}

  function setupPickers(){genrePicker=new MV.admin.common.RelationshipPicker("movieGenres",genres,{placeholder:"Search genres…"});actorPicker=new MV.admin.common.RelationshipPicker("movieActors",actors,{cast:true,placeholder:"Search actors…"});directorPicker=new MV.admin.common.RelationshipPicker("movieDirectors",directors,{placeholder:"Search directors…"});writerPicker=new MV.admin.common.RelationshipPicker("movieWriters",writers,{placeholder:"Search writers…"});MV.admin.common.bindImagePreview(document.getElementById("PosterImage"),document.getElementById("posterPreview"),{label:"Poster",kind:"movie"});}
  function fill(m){
    const set=(name,value)=>{const el=form.elements[name];if(el)el.value=typeof value==="string"?MV.ui.optionalText(value):(value??"");};
    set("Title",m.title);set("TrailerUrl",m.trailerUrl);set("ReleaseDate",MV.admin.common.dateInput(m.releaseDate));set("ContentRating",m.contentRating);set("RuntimeMinutes",m.runtimeMinutes);set("Synopsis",m.synopsis);set("Storyline",m.storyline);set("Tagline",m.tagline);set("OriginalLanguage",m.originalLanguage);set("CountryOfOrigin",m.countryOfOrigin);set("FilmingLocation",m.filmingLocation);set("ProductionCompany",m.productionCompany);set("Budget",m.budget);set("GrossWorldwide",m.grossWorldwide);set("Color",m.color);set("SoundMix",m.soundMix);set("Trivia",m.trivia);
    const preview=document.getElementById("posterPreview");preview.src=MV.media.movieImageUrl(m.posterUrl,true);preview.dataset.mvPlaceholderKind="movie";preview.dataset.mvAdminDepth="true";genrePicker.setSelected((m.genres||[]).map(g=>g.id));actorPicker.setSelected(m.cast||[]);directorPicker.setSelected(m.directors||[]);writerPicker.setSelected(m.writers||[]);
  }
  function setupValidation(){
    $.validator.addMethod("validYearDate",v=>!v||(new Date(v).getFullYear()>=1888&&new Date(v).getFullYear()<=2100),"Release year must be between 1888 and 2100.");
    $.validator.addMethod("absoluteUrl",v=>{if(!v)return true;try{return Boolean(new URL(v).protocol);}catch{return false;}},"Trailer URL must be valid.");
    $(form).validate({rules:{Title:{required:true,maxlength:300},Synopsis:{required:true},ReleaseDate:{required:true,validYearDate:true},RuntimeMinutes:{required:true,min:1},TrailerUrl:{absoluteUrl:true},Budget:{min:0},GrossWorldwide:{min:0}},messages:{Title:{required:"Movie title is required.",maxlength:"Movie title cannot exceed 300 characters."},Synopsis:{required:"Movie synopsis is required."},RuntimeMinutes:{required:"Runtime is required.",min:"Runtime must be greater than 0."}},submitHandler:submit});
  }
  async function submit(){
    if(!genrePicker.ids().length){MV.ui.toast("Movie must have at least one genre.","warning");document.getElementById("movieGenres").scrollIntoView({behavior:"smooth"});return;}
    const cast=actorPicker.castValues();if(!MV.admin.common.validateCast(cast))return;const file=document.getElementById("PosterImage").files?.[0];if(file&&!MV.admin.common.validateImage(file,"Poster"))return;
    const fd=new FormData(form);MV.admin.common.addList(fd,"GenreIds",genrePicker.ids());MV.admin.common.addCast(fd,"Actors",cast);MV.admin.common.addList(fd,"DirectorIds",directorPicker.ids());MV.admin.common.addList(fd,"WriterIds",writerPicker.ids());
    MV.ui.buttonBusy(saveBtn,true,"Saving…");try{if(id){await MV.api.put(`movies/${id}`,fd);MV.ui.setFlash("Movie updated","success");}else{await MV.api.post("movies",fd);MV.ui.setFlash("Movie created","success");}location.href="movies.html";}catch(err){MV.ui.showError(err);}finally{MV.ui.buttonBusy(saveBtn,false);}
  }
});
