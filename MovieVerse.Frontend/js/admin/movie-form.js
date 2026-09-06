document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const id =
    new URLSearchParams(location.search)
      .get("id");

  const form =
    document.getElementById("movieForm");

  const saveBtn =
    document.getElementById("saveMovie");

  MV.ui.useBackendValidation(form);

  document.getElementById(
    "movieFormTitle"
  ).textContent =
    id ? "Edit Movie" : "Create Movie";

  let genres = [];
  let actors = [];
  let directors = [];
  let writers = [];
  let movie = null;

  let genrePicker;
  let actorPicker;
  let directorPicker;
  let writerPicker;

  try {
    const requests = [
      MV.api.get("genres"),
      MV.api.get("actors"),
      MV.api.get("directors"),
      MV.api.get("writers")
    ];

    if (id) {
      requests.push(
        MV.api.get(`movies/${id}`)
      );
    }

    const data =
      await Promise.all(requests);

    [
      genres,
      actors,
      directors,
      writers
    ] = data;

    movie =
      id ? data[4] : null;

    setupPickers();

    if (movie) fill(movie);

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
      "Movie editor could not be loaded."
    );

    form
      .querySelectorAll(
        "input, textarea, button, select"
      )
      .forEach(element => {
        element.disabled = true;
      });
  }

  function setupPickers() {
    genrePicker =
      new MV.admin.common.RelationshipPicker(
        "movieGenres",
        genres,
        {
          placeholder:
            "Search genres…"
        }
      );

    actorPicker =
      new MV.admin.common.RelationshipPicker(
        "movieActors",
        actors,
        {
          cast: true,
          placeholder:
            "Search actors…"
        }
      );

    directorPicker =
      new MV.admin.common.RelationshipPicker(
        "movieDirectors",
        directors,
        {
          placeholder:
            "Search directors…"
        }
      );

    writerPicker =
      new MV.admin.common.RelationshipPicker(
        "movieWriters",
        writers,
        {
          placeholder:
            "Search writers…"
        }
      );

    MV.admin.common.bindImagePreview(
      document.getElementById(
        "PosterImage"
      ),
      document.getElementById(
        "posterPreview"
      ),
      {
        kind: "movie"
      }
    );
  }

  function fill(movieData) {
    const set = (name, value) => {
      const element =
        form.elements[name];

      if (!element) return;

      element.value =
        typeof value === "string"
          ? MV.ui.optionalText(value)
          : (value ?? "");
    };

    set("Title", movieData.title);
    set("TrailerUrl", movieData.trailerUrl);
    set(
      "ReleaseDate",
      MV.admin.common.dateInput(
        movieData.releaseDate
      )
    );
    set(
      "ContentRating",
      movieData.contentRating
    );
    set(
      "RuntimeMinutes",
      movieData.runtimeMinutes
    );
    set(
      "Synopsis",
      movieData.synopsis
    );
    set(
      "Storyline",
      movieData.storyline
    );
    set("Tagline", movieData.tagline);
    set(
      "OriginalLanguage",
      movieData.originalLanguage
    );
    set(
      "CountryOfOrigin",
      movieData.countryOfOrigin
    );
    set(
      "FilmingLocation",
      movieData.filmingLocation
    );
    set(
      "ProductionCompany",
      movieData.productionCompany
    );
    set("Budget", movieData.budget);
    set(
      "GrossWorldwide",
      movieData.grossWorldwide
    );
    set("Color", movieData.color);
    set(
      "SoundMix",
      movieData.soundMix
    );
    set("Trivia", movieData.trivia);

    const preview =
      document.getElementById(
        "posterPreview"
      );

    preview.src =
      MV.media.movieImageUrl(
        movieData.posterUrl,
        true
      );

    preview.dataset.mvPlaceholderKind =
      "movie";

    preview.dataset.mvAdminDepth =
      "true";

    genrePicker.setSelected(
      (movieData.genres || [])
        .map(genre => genre.id)
    );

    actorPicker.setSelected(
      movieData.cast || []
    );

    directorPicker.setSelected(
      movieData.directors || []
    );

    writerPicker.setSelected(
      movieData.writers || []
    );
  }

  async function submit() {
    MV.ui.clearFormError(form);

    const formData =
      new FormData(form);

    MV.admin.common.addList(
      formData,
      "GenreIds",
      genrePicker.ids()
    );

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
          `movies/${id}`,
          formData
        );

        MV.ui.setFlash(
          "Movie updated",
          "success"
        );
      } else {
        await MV.api.post(
          "movies",
          formData
        );

        MV.ui.setFlash(
          "Movie created",
          "success"
        );
      }

      location.href =
        "movies.html";
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        id
          ? "Movie could not be updated."
          : "Movie could not be created."
      );
    } finally {
      MV.ui.buttonBusy(
        saveBtn,
        false
      );
    }
  }
});
