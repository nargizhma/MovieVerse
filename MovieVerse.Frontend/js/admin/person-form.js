document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const params = new URLSearchParams(location.search);
  const type = params.get("type");
  const id = params.get("id");

  if (!["actor", "director", "writer"].includes(type)) {
    location.replace("actors.html");
    return;
  }

  const form = document.getElementById("personForm");
  const saveBtn = document.getElementById("savePerson");
  const label = type.charAt(0).toUpperCase() + type.slice(1);

  document.getElementById("personFormTitle").textContent = id
    ? `Edit ${label}`
    : `Create ${label}`;

  document.querySelectorAll(".js-person-label").forEach(element => {
    element.textContent = label;
  });

  document.getElementById("personBackLink").href = `${type}s.html`;

  MV.admin.common.bindImagePreview(
    document.getElementById("ProfileImage"),
    document.getElementById("personPreview"),
    { label: "Profile image", kind: "people" }
  );

  if (id) {
    try {
      fill(await MV.api.get(`${type}s/${id}`));
    } catch (err) {
      MV.ui.showFormError(
        form,
        err,
        `${label} editor could not be loaded.`
      );

      form
        .querySelectorAll("input, textarea, button, select")
        .forEach(element => {
          element.disabled = true;
        });

      return;
    }
  }

  $.validator.addMethod(
    "notFuture",
    value => !value || new Date(value) <= new Date(),
    "Date cannot be in the future."
  );

  $.validator.addMethod(
    "deathAfterBirth",
    value =>
      !value ||
      !form.elements.BirthDate.value ||
      new Date(value) > new Date(form.elements.BirthDate.value),
    "Death date must be after birth date."
  );

  $(form).validate({
    rules: {
      FullName: {
        required: true,
        maxlength: 200
      },

      BirthPlace: {
        maxlength: 200
      },

      DeathPlace: {
        maxlength: 200
      },

      AlternativeName: {
        maxlength: 200
      },

      HeightInMeters: {
        min: 0.01
      },

      BirthDate: {
        notFuture: true
      },

      DeathDate: {
        notFuture: true,
        deathAfterBirth: true
      }
    },

    messages: {
      FullName: {
        required: `${label} name is required.`,
        maxlength: `${label} name cannot exceed 200 characters.`
      },

      BirthPlace: {
        maxlength: "Birth place cannot exceed 200 characters."
      },

      DeathPlace: {
        maxlength: "Death place cannot exceed 200 characters."
      },

      AlternativeName: {
        maxlength: "Alternative name cannot exceed 200 characters."
      },

      HeightInMeters: {
        min: "Height must be greater than 0."
      }
    },

    invalidHandler: function (_, validator) {
      if (!validator.errorList.length) return;

      const errors = {};

      validator.errorList.forEach(item => {
        const key = item.element?.name || item.element?.id || "Form";

        if (!errors[key]) {
          errors[key] = [];
        }

        errors[key].push(item.message);
      });

      MV.ui.showFormError(
        form,
        {
          data: { errors },
          detail: validator.errorList
            .map(item => item.message)
            .join(" ")
        },
        id
          ? `${label} could not be updated.`
          : `${label} could not be created.`
      );
    },

    submitHandler: submit
  });

  // If the user starts correcting a field, remove the old server-side
  // red state/summary so the next validation result is clear.
  form.addEventListener("input", event => {
    const target = event.target;

    if (
      target instanceof HTMLElement &&
      target.matches("input, textarea, select") &&
      target.dataset.mvServerInvalid === "true"
    ) {
      target.classList.remove("is-invalid");
      delete target.dataset.mvServerInvalid;
    }
  });

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
        name.charAt(0).toLowerCase() + name.slice(1);

      form.elements[name].value =
        MV.ui.optionalText(person[propertyName]);
    });

    form.elements.BirthDate.value =
      MV.admin.common.dateInput(person.birthDate);

    form.elements.DeathDate.value =
      MV.admin.common.dateInput(person.deathDate);

    const preview =
      document.getElementById("personPreview");

    preview.src =
      MV.media.personImageUrl(
        person.profileImageUrl,
        type,
        true
      );

    preview.dataset.mvPlaceholderKind = "people";
    preview.dataset.mvAdminDepth = "true";
  }

  async function submit() {
    MV.ui.clearFormError(form);

    const file =
      document.getElementById("ProfileImage")
        .files?.[0];

    if (
      file &&
      !MV.admin.common.validateImage(
        file,
        "Profile image"
      )
    ) {
      return;
    }

    const formData = new FormData(form);

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

      location.href = `${type}s.html`;
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
