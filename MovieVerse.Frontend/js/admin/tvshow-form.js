document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const id = new URLSearchParams(location.search).get("id");
  const form = document.getElementById("tvshowForm");
  const saveBtn = document.getElementById("saveTvShow");

  document.getElementById("tvshowFormTitle").textContent = id ? "Edit TV Show" : "Create TV Show";

  let genres = [];
  let actors = [];
  let show = null;
  let genrePicker;
  let actorPicker;

  try {
    const requests = [
      MV.api.get("genres"),
      MV.api.get("actors")
    ];

    if (id) requests.push(MV.api.get(`tvshows/${id}`));

    const data = await Promise.all(requests);
    [genres, actors] = data;
    show = id ? data[2] : null;

    setupPickers();
    if (show) fill(show);
    setupValidation();

    if (id) {
      document.getElementById("seasonManagement").classList.remove("d-none");
      bindSeasonAdd();
      await loadSeasons();
    }
  } catch (err) {
    MV.ui.showFormError(
      form,
      err,
      id ? "TV show editor could not be loaded." : "TV show creation form could not be loaded."
    );

    form.querySelectorAll("input, textarea, button, select").forEach(element => {
      element.disabled = true;
    });
  }

  function setupPickers() {
    genrePicker = new MV.admin.common.RelationshipPicker("tvGenres", genres, {
      placeholder: "Search genres…"
    });

    actorPicker = new MV.admin.common.RelationshipPicker("tvActors", actors, {
      cast: true,
      placeholder: "Search actors…"
    });

    MV.admin.common.bindImagePreview(
      document.getElementById("PosterImage"),
      document.getElementById("posterPreview"),
      { label: "Poster", kind: "movie" }
    );
  }

  function fill(showData) {
    const set = (name, value) => {
      if (!form.elements[name]) return;

      form.elements[name].value = typeof value === "string"
        ? MV.ui.optionalText(value)
        : (value ?? "");
    };

    set("Title", showData.title);
    set("OriginalTitle", showData.originalTitle);
    set("TrailerUrl", showData.trailerUrl);
    set("Synopsis", showData.synopsis);
    set("ReleaseDate", MV.admin.common.dateInput(showData.releaseDate));
    set("EndDate", MV.admin.common.dateInput(showData.endDate));
    set("ContentRating", showData.contentRating);
    set("RuntimeMinutes", showData.runtimeMinutes);
    set("Storyline", showData.storyline);
    set("OriginalLanguage", showData.originalLanguage);
    set("CountryOfOrigin", showData.countryOfOrigin);
    set("ProductionCompany", showData.productionCompany);
    set("Trivia", showData.trivia);
    set("Color", showData.color);

    const preview = document.getElementById("posterPreview");
    preview.src = MV.media.tvShowImageUrl(showData.posterUrl, true);
    preview.dataset.mvPlaceholderKind = "movie";
    preview.dataset.mvAdminDepth = "true";

    genrePicker.setSelected((showData.genres || []).map(genre => genre.id));
    actorPicker.setSelected(showData.cast || []);
  }

  function setupValidation() {
    $.validator.addMethod(
      "validYearDate",
      value => !value || (
        new Date(value).getFullYear() >= 1888 &&
        new Date(value).getFullYear() <= 2100
      ),
      "Release year must be between 1888 and 2100."
    );

    $.validator.addMethod(
      "absoluteUrl",
      value => {
        if (!value) return true;
        try {
          return Boolean(new URL(value).protocol);
        } catch {
          return false;
        }
      },
      "Trailer URL must be valid."
    );

    $.validator.addMethod(
      "endAfterStart",
      value => !value || !form.elements.ReleaseDate.value ||
        new Date(value) >= new Date(form.elements.ReleaseDate.value),
      "End date cannot be earlier than release date."
    );

    $(form).validate({
      rules: {
        Title: { required: true, maxlength: 300 },
        OriginalTitle: { maxlength: 300 },
        Synopsis: { required: true },
        ReleaseDate: { required: true, validYearDate: true },
        EndDate: { endAfterStart: true },
        RuntimeMinutes: { min: 1 },
        TrailerUrl: { absoluteUrl: true }
      },
      messages: {
        Title: {
          required: "TV show title is required.",
          maxlength: "TV show title cannot exceed 300 characters."
        },
        Synopsis: {
          required: "TV show synopsis is required."
        },
        RuntimeMinutes: {
          min: "Runtime must be greater than 0."
        }
      },
      submitHandler: submit
    });
  }

  async function submit() {
    MV.ui.clearFormError(form);

    if (!genrePicker.ids().length) {
      MV.ui.showFormError(
        form,
        new MV.ApiError({ detail: "TV show must have at least one genre." }),
        id ? "TV show could not be updated." : "TV show could not be created."
      );
      document.getElementById("tvGenres")?.scrollIntoView({ behavior: "smooth", block: "center" });
      return;
    }

    const cast = actorPicker.castValues();
    if (!MV.admin.common.validateCast(cast)) return;

    const file = document.getElementById("PosterImage").files?.[0];
    if (file && !MV.admin.common.validateImage(file, "Poster")) return;

    const formData = new FormData(form);
    MV.admin.common.addList(formData, "GenreIds", genrePicker.ids());
    MV.admin.common.addCast(formData, "Actors", cast);

    MV.ui.buttonBusy(saveBtn, true, "Saving…");

    try {
      if (id) {
        await MV.api.put(`tvshows/${id}`, formData);
        MV.ui.setFlash("TV show updated", "success");
      } else {
        await MV.api.post("tvshows", formData);
        MV.ui.setFlash("TV show created", "success");
      }

      location.href = "tvshows.html";
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        id ? "TV show could not be updated." : "TV show could not be created."
      );
    } finally {
      MV.ui.buttonBusy(saveBtn, false);
    }
  }

  function bindSeasonAdd() {
    const seasonForm = document.getElementById("addSeasonForm");

    seasonForm.addEventListener("submit", async event => {
      event.preventDefault();
      MV.ui.clearFormError(seasonForm);

      const input = event.currentTarget.elements.SeasonNumber;
      const number = Number(input.value);

      if (number <= 0) {
        MV.ui.showFormError(
          seasonForm,
          new MV.ApiError({ detail: "Season number must be greater than 0." }),
          "Season could not be added."
        );
        return;
      }

      const button = event.currentTarget.querySelector("button");
      MV.ui.buttonBusy(button, true, "Adding…");

      try {
        await MV.api.post(`tvshows/${id}/seasons`, { seasonNumber: number });
        MV.ui.toast("Season added", "success");
        input.value = "";
        MV.ui.clearFormError(seasonForm);
        await loadSeasons();
      } catch (err) {
        MV.ui.showFormError(seasonForm, err, "Season could not be added.");
      } finally {
        MV.ui.buttonBusy(button, false);
      }
    });
  }

  async function loadSeasons() {
    const host = document.getElementById("seasonList");
    host.innerHTML = MV.ui.skeletonLines(4);

    try {
      const seasons = (await MV.api.get(`tvshows/${id}/seasons`))
        .sort((a, b) => a.seasonNumber - b.seasonNumber);

      if (!seasons.length) {
        host.innerHTML = MV.ui.emptyState({
          icon: "fa-layer-group",
          title: "No seasons yet"
        });
        return;
      }

      host.innerHTML = seasons.map(season => `
        <section class="mv-season-admin" data-season-id="${season.id}">
          <div class="mv-season-admin-head">
            <div class="d-flex align-items-center gap-2">
              <strong>Season</strong>
              <input class="form-control form-control-sm js-season-number" type="number" min="1" value="${season.seasonNumber}" style="width:90px">
              <span class="text-secondary small">${season.episodeCount} episodes</span>
            </div>
            <div class="d-flex flex-wrap gap-2">
              <button class="btn btn-sm btn-outline-primary js-save-season">Save number</button>
              <button class="btn btn-sm btn-secondary js-toggle-episodes">Manage Episodes</button>
              <button class="btn btn-sm btn-outline-danger js-delete-season">Delete</button>
            </div>
          </div>
          <div class="mv-season-admin-body d-none js-episodes-body"></div>
        </section>`).join("");

      host.querySelectorAll(".mv-season-admin").forEach(card => bindSeasonCard(card));
    } catch (err) {
      host.innerHTML = MV.ui.emptyState({
        icon: "fa-circle-exclamation",
        title: "Seasons unavailable",
        text: err.detail || "Try again."
      });
    }
  }

  function bindSeasonCard(card) {
    const seasonId = card.dataset.seasonId;

    card.querySelector(".js-save-season").addEventListener("click", async () => {
      const number = Number(card.querySelector(".js-season-number").value);

      if (number <= 0) {
        MV.ui.toast("Season number must be greater than 0.", "warning");
        return;
      }

      try {
        await MV.api.put(`seasons/${seasonId}`, { seasonNumber: number });
        MV.ui.toast("Season updated", "success");
        await loadSeasons();
      } catch (err) {
        MV.ui.showError(err);
      }
    });

    card.querySelector(".js-delete-season").addEventListener("click", async () => {
      const ok = await MV.ui.confirm({
        title: "Delete season?",
        message: "Deleting a season can affect its episodes. Continue?",
        confirmText: "Delete season"
      });

      if (!ok) return;

      try {
        await MV.api.delete(`seasons/${seasonId}`);
        MV.ui.toast("Season deleted", "success");
        await loadSeasons();
      } catch (err) {
        MV.ui.showError(err);
      }
    });

    card.querySelector(".js-toggle-episodes").addEventListener("click", async () => {
      const body = card.querySelector(".js-episodes-body");
      body.classList.toggle("d-none");

      if (!body.classList.contains("d-none") && !body.dataset.loaded) {
        await loadEpisodes(seasonId, body);
      }
    });
  }

  async function loadEpisodes(seasonId, body) {
    body.innerHTML = MV.ui.skeletonLines(4);

    try {
      const episodes = (await MV.api.get(`seasons/${seasonId}/episodes`))
        .sort((a, b) => a.episodeNumber - b.episodeNumber);

      body.dataset.loaded = "true";
      body.innerHTML = `
        <div class="d-flex justify-content-between align-items-center gap-2 mb-3">
          <strong>Episodes</strong>
          <a class="btn btn-sm btn-primary" href="episode-form.html?seasonId=${seasonId}">
            <i class="fa-solid fa-plus me-1"></i>Add Episode
          </a>
        </div>
        ${episodes.length
          ? `<div class="table-responsive">
              <table class="table align-middle">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Image</th>
                    <th>Title</th>
                    <th>Rating</th>
                    <th class="text-end">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  ${episodes.map(episode => `
                    <tr>
                      <td>${episode.episodeNumber}</td>
                      <td>
                        <img
                          src="${MV.media.episodeImageUrl(episode.imageUrl, true)}"
                          ${MV.media.imageFallbackAttributes("movie", true)}
                          alt=""
                          style="width:75px;height:45px;object-fit:cover;border-radius:.3rem">
                      </td>
                      <td>${MV.ui.escapeHtml(episode.title)}</td>
                      <td>${MV.media.rating(episode.averageRating)}</td>
                      <td class="text-end">
                        <a class="btn btn-sm btn-outline-primary" href="episode-form.html?seasonId=${seasonId}&id=${episode.id}">Edit</a>
                        <button
                          class="btn btn-sm btn-outline-danger js-delete-episode"
                          data-id="${episode.id}"
                          data-title="${MV.ui.escapeHtml(episode.title)}">
                          Delete
                        </button>
                      </td>
                    </tr>`).join("")}
                </tbody>
              </table>
            </div>`
          : MV.ui.emptyState({
              icon: "fa-list-ol",
              title: "No episodes yet",
              actionText: "Add Episode",
              actionHref: `episode-form.html?seasonId=${seasonId}`
            })}`;

      body.querySelectorAll(".js-delete-episode").forEach(button => {
        button.addEventListener("click", () => deleteEpisode(button, seasonId, body));
      });
    } catch (err) {
      body.innerHTML = MV.ui.emptyState({
        icon: "fa-circle-exclamation",
        title: "Episodes unavailable",
        text: err.detail || "Try again."
      });
    }
  }

  async function deleteEpisode(button, seasonId, body) {
    const ok = await MV.ui.confirm({
      title: "Delete episode?",
      message: `Delete “${button.dataset.title}”?`,
      confirmText: "Delete episode"
    });

    if (!ok) return;

    try {
      await MV.api.delete(`episodes/${button.dataset.id}`);
      MV.ui.toast("Episode deleted", "success");
      body.dataset.loaded = "";
      await loadEpisodes(seasonId, body);
    } catch (err) {
      MV.ui.showError(err);
    }
  }
});
