document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const host =
    document.getElementById(
      "genresList"
    );

  const form =
    document.getElementById(
      "genreForm"
    );

  const input =
    form.elements.Name;

  const addButton =
    document.getElementById(
      "addGenre"
    );

  MV.ui.useBackendValidation(form);

  let genres = [];

  try {
    genres =
      await MV.api.get("genres");

    render();
  } catch (err) {
    host.innerHTML =
      MV.ui.emptyState({
        icon: "fa-tags",
        title:
          "Could not load genres",
        text:
          err.detail ||
          "Try again."
      });
  }

  form.addEventListener(
    "submit",
    async event => {
      event.preventDefault();

      MV.ui.clearFormError(form);

      MV.ui.buttonBusy(
        addButton,
        true,
        "Adding…"
      );

      try {
        await MV.api.post(
          "genres",
          {
            name:
              input.value.trim()
          }
        );

        MV.ui.toast(
          "Genre created",
          "success"
        );

        input.value = "";

        genres =
          await MV.api.get(
            "genres"
          );

        render();
      } catch (err) {
        MV.ui.showFormError(
          form,
          err,
          "Genre could not be created."
        );
      } finally {
        MV.ui.buttonBusy(
          addButton,
          false
        );
      }
    }
  );

  function render() {
    if (!genres.length) {
      host.innerHTML =
        MV.ui.emptyState({
          icon: "fa-tags",
          title:
            "No genres yet"
        });

      return;
    }

    host.innerHTML = `
      <div class="table-responsive">
        <table class="table align-middle">
          <thead>
            <tr>
              <th>Name</th>
              <th class="text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            ${genres
              .map(
                genre => `
                  <tr data-id="${genre.id}">
                    <td>
                      <input class="form-control form-control-sm js-name" value="${MV.ui.escapeHtml(genre.name)}">
                      <div class="js-row-error mt-2"></div>
                    </td>
                    <td class="text-end">
                      <button class="btn btn-sm btn-outline-primary js-save">Save</button>
                      <button class="btn btn-sm btn-outline-danger js-delete">Delete</button>
                    </td>
                  </tr>`
              )
              .join("")}
          </tbody>
        </table>
      </div>`;

    host
      .querySelectorAll("tr[data-id]")
      .forEach(row => {
        row
          .querySelector(".js-save")
          .addEventListener(
            "click",
            () => save(row)
          );

        row
          .querySelector(".js-delete")
          .addEventListener(
            "click",
            () => del(row)
          );
      });
  }

  async function save(row) {
    const nameInput =
      row.querySelector(
        ".js-name"
      );

    const errorHost =
      row.querySelector(
        ".js-row-error"
      );

    MV.ui.clearInlineError(
      errorHost
    );

    try {
      await MV.api.put(
        `genres/${row.dataset.id}`,
        {
          name:
            nameInput.value.trim()
        }
      );

      MV.ui.toast(
        "Genre updated",
        "success"
      );

      genres =
        await MV.api.get(
          "genres"
        );

      render();
    } catch (err) {
      MV.ui.showInlineError(
        errorHost,
        err,
        "Genre could not be updated."
      );
    }
  }

  async function del(row) {
    const name =
      row.querySelector(
        ".js-name"
      ).value;

    const ok =
      await MV.ui.confirm({
        title:
          "Delete genre?",
        message:
          `Delete “${name}”?`,
        confirmText:
          "Delete genre"
      });

    if (!ok) return;

    try {
      await MV.api.delete(
        `genres/${row.dataset.id}`
      );

      MV.ui.toast(
        "Genre deleted",
        "success"
      );

      genres =
        await MV.api.get(
          "genres"
        );

      render();
    } catch (err) {
      MV.ui.showError(err);
    }
  }
});
