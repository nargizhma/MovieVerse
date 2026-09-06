document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const params =
    new URLSearchParams(location.search);

  let seasonId =
    params.get("seasonId");

  const id =
    params.get("id");

  const form =
    document.getElementById(
      "episodeForm"
    );

  const saveBtn =
    document.getElementById(
      "saveEpisode"
    );

  MV.ui.useBackendValidation(form);

  let actors = [];
  let directors = [];
  let writers = [];
  let season = null;
  let episode = null;

  let actorPicker;
  let directorPicker;
  let writerPicker;

  try {
    if (id && !seasonId) {
      episode =
        await MV.api.get(
          `episodes/${id}`
        );

      seasonId =
        episode.seasonId;
    }

    if (!seasonId) {
      MV.ui.showFormError(
        form,
        new MV.ApiError({
          detail:
            "Season id is required."
        }),
        "Episode editor could not be loaded."
      );

      return;
    }

    const requests = [
      MV.api.get("actors"),
      MV.api.get("directors"),
      MV.api.get("writers"),
      MV.api.get(
        `seasons/${seasonId}`
      )
    ];

    if (id && !episode) {
      requests.push(
        MV.api.get(
          `episodes/${id}`
        )
      );
    }

    const data =
      await Promise.all(requests);

    [
      actors,
      directors,
      writers,
      season
    ] = data;

    if (id && !episode) {
      episode = data[4];
    }

    document.getElementById(
      "episodeFormTitle"
    ).textContent =
      id
        ? `Edit Episode · Season ${season.seasonNumber}`
        : `Add Episode · Season ${season.seasonNumber}`;

    setupPickers();

    if (episode) fill(episode);

    form.addEventListener(
      "submit",
      event => {
        event.preventDefault();
        submit();
      }
    );
  } catch (err) {
    MV.ui.showFormError(
      form,
      err,
      "Episode editor could not be loaded."
    );
  }

  function setupPickers() {
    actorPicker =
      new MV.admin.common.RelationshipPicker(
        "episodeActors",
        actors,
        {
          cast: true,
          placeholder:
            "Search actors…"
        }
      );

    directorPicker =
      new MV.admin.common.RelationshipPicker(
        "episodeDirectors",
        directors,
        {
          placeholder:
            "Search directors…"
        }
      );

    writerPicker =
      new MV.admin.common.RelationshipPicker(
        "episodeWriters",
        writers,
        {
          placeholder:
            "Search writers…"
        }
      );

    MV.admin.common.bindImagePreview(
      document.getElementById(
        "Image"
      ),
      document.getElementById(
        "episodePreview"
      ),
      {
        kind: "movie"
      }
    );
  }

  function fill(episodeData) {
    const set = (name, value) => {
      if (!form.elements[name]) return;

      form.elements[name].value =
        typeof value === "string"
          ? MV.ui.optionalText(value)
          : (value ?? "");
    };

    set("Title", episodeData.title);
    set(
      "EpisodeNumber",
      episodeData.episodeNumber
    );
    set(
      "Description",
      episodeData.description
    );
    set(
      "ReleaseDate",
      MV.admin.common.dateInput(
        episodeData.releaseDate
      )
    );
    set(
      "RuntimeMinutes",
      episodeData.runtimeMinutes
    );

    const preview =
      document.getElementById(
        "episodePreview"
      );

    preview.src =
      MV.media.episodeImageUrl(
        episodeData.imageUrl,
        true
      );

    preview.dataset.mvPlaceholderKind =
      "movie";

    preview.dataset.mvAdminDepth =
      "true";

    actorPicker.setSelected(
      episodeData.cast || []
    );

    directorPicker.setSelected(
      episodeData.directors || []
    );

    writerPicker.setSelected(
      episodeData.writers || []
    );
  }

  async function submit() {
    MV.ui.clearFormError(form);

    const formData =
      new FormData(form);

    MV.admin.common.addCast(
      formData,
      "Actors",
      actorPicker.castValues()
    );

    MV.admin.common.addList(
      formData,
      "DirectorIds",
      directorPicker.ids()
    );

    MV.admin.common.addList(
      formData,
      "WriterIds",
      writerPicker.ids()
    );

    MV.ui.buttonBusy(
      saveBtn,
      true,
      "Saving…"
    );

    try {
      if (id) {
        await MV.api.put(
          `episodes/${id}`,
          formData
        );

        MV.ui.setFlash(
          "Episode updated",
          "success"
        );
      } else {
        await MV.api.post(
          `seasons/${seasonId}/episodes`,
          formData
        );

        MV.ui.setFlash(
          "Episode created",
          "success"
        );
      }

      location.href =
        season?.tvShowId
          ? `tvshow-form.html?id=${season.tvShowId}`
          : "tvshows.html";
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        id
          ? "Episode could not be updated."
          : "Episode could not be created."
      );
    } finally {
      MV.ui.buttonBusy(
        saveBtn,
        false
      );
    }
  }
});
