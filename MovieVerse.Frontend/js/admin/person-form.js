document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const params =
    new URLSearchParams(location.search);

  const type =
    params.get("type");

  const id =
    params.get("id");

  if (
    !["actor", "director", "writer"]
      .includes(type)
  ) {
    location.replace("actors.html");
    return;
  }

  const form =
    document.getElementById(
      "personForm"
    );

  const saveBtn =
    document.getElementById(
      "savePerson"
    );

  MV.ui.useBackendValidation(form);

  const label =
    type.charAt(0).toUpperCase() +
    type.slice(1);

  document.getElementById(
    "personFormTitle"
  ).textContent =
    id
      ? `Edit ${label}`
      : `Create ${label}`;

  document
    .querySelectorAll(
      ".js-person-label"
    )
    .forEach(element => {
      element.textContent = label;
    });

  document.getElementById(
    "personBackLink"
  ).href =
    `${type}s.html`;

  MV.admin.common.bindImagePreview(
    document.getElementById(
      "ProfileImage"
    ),
    document.getElementById(
      "personPreview"
    ),
    {
      kind: "people"
    }
  );

  if (id) {
    try {
      fill(
        await MV.api.get(
          `${type}s/${id}`
        )
      );
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        `${label} editor could not be loaded.`
      );

      form
        .querySelectorAll(
          "input, textarea, button, select"
        )
        .forEach(element => {
          element.disabled = true;
        });

      return;
    }
  }

  form.addEventListener(
    "submit",
    event => {
      event.preventDefault();
      submit();
    }
  );

  function fill(person) {
    const fields = [
      "FullName",
      "Biography",
      "BirthPlace",
      "DeathPlace",
      "HeightInMeters",
      "AlternativeName",
      "Nickname",
      "Spouse",
      "Children",
      "Parents",
      "Relatives",
      "OtherWorks",
      "Trivia",
      "Quote",
      "Trademark"
    ];

    fields.forEach(name => {
      if (!form.elements[name]) return;

      const propertyName =
        name.charAt(0).toLowerCase() +
        name.slice(1);

      form.elements[name].value =
        MV.ui.optionalText(
          person[propertyName]
        );
    });

    form.elements.BirthDate.value =
      MV.admin.common.dateInput(
        person.birthDate
      );

    form.elements.DeathDate.value =
      MV.admin.common.dateInput(
        person.deathDate
      );

    const preview =
      document.getElementById(
        "personPreview"
      );

    preview.src =
      MV.media.personImageUrl(
        person.profileImageUrl,
        type,
        true
      );

    preview.dataset.mvPlaceholderKind =
      "people";

    preview.dataset.mvAdminDepth =
      "true";
  }

  async function submit() {
    MV.ui.clearFormError(form);

    const formData =
      new FormData(form);

    MV.ui.buttonBusy(
      saveBtn,
      true,
      "Saving…"
    );

    try {
      if (id) {
        await MV.api.put(
          `${type}s/${id}`,
          formData
        );

        MV.ui.setFlash(
          `${label} updated`,
          "success"
        );
      } else {
        await MV.api.post(
          `${type}s`,
          formData
        );

        MV.ui.setFlash(
          `${label} created`,
          "success"
        );
      }

      location.href =
        `${type}s.html`;
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        id
          ? `${label} could not be updated.`
          : `${label} could not be created.`
      );
    } finally {
      MV.ui.buttonBusy(
        saveBtn,
        false
      );
    }
  }
});
