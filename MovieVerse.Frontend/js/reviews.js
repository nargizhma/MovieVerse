window.MV = window.MV || {};

MV.reviews = (() => {
  const contextPath = (type, id) =>
    `${String(type).toLowerCase() === "tvshow"
      ? "tvshows"
      : String(type).toLowerCase() === "episode"
        ? "episodes"
        : "movies"}/${id}/reviews`;

  async function getMine(
    type,
    id,
    { silent404 = true } = {}
  ) {
    if (!MV.auth.isAuthenticated()) {
      return null;
    }

    try {
      return await MV.api.get(
        `${contextPath(type, id)}/me`,
        null,
        { handle401: false }
      );
    } catch (err) {
      if (
        silent404 &&
        err.status === 404
      ) {
        return null;
      }

      throw err;
    }
  }

  async function getAll(type, id) {
    return MV.api.get(
      contextPath(type, id)
    );
  }

  async function save(
    type,
    id,
    rating,
    content,
    existing
  ) {
    const body = {
      rating: Number(rating),
      content:
        content === ""
          ? null
          : content
    };

    if (existing) {
      await MV.api.put(
        `${contextPath(type, id)}/me`,
        body
      );

      return "updated";
    }

    await MV.api.post(
      contextPath(type, id),
      body
    );

    return "created";
  }

  async function removeEntire(
    type,
    id
  ) {
    return MV.api.delete(
      `${contextPath(type, id)}/me`
    );
  }

  async function deleteWrittenOnly(
    type,
    id,
    existing
  ) {
    if (!existing) return;

    return MV.api.put(
      `${contextPath(type, id)}/me`,
      {
        rating:
          Number(existing.rating),
        content: null
      }
    );
  }

  function ensureReviewModal() {
    if (
      document.getElementById(
        "mvReviewModal"
      )
    ) {
      return;
    }

    document.body.insertAdjacentHTML(
      "beforeend",
      `
      <div class="modal fade" id="mvReviewModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
          <div class="modal-content">
            <form id="mvReviewForm" novalidate>
              <div class="modal-header">
                <div>
                  <div class="mv-kicker">Your review</div>
                  <h5 class="modal-title" id="mvReviewTitle">Write a review</h5>
                </div>
                <button class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body">
                <div class="row g-3">
                  <div class="col-sm-4">
                    <label class="form-label" for="mvReviewRating">Rating</label>
                    <div class="input-group">
                      <input id="mvReviewRating" name="rating" class="form-control" type="number">
                      <span class="input-group-text">/ 10</span>
                    </div>
                    <div class="form-text text-secondary">The backend accepts 1 to 10 with at most one decimal place.</div>
                  </div>
                  <div class="col-12">
                    <label class="form-label" for="mvReviewContent">Written review</label>
                    <textarea id="mvReviewContent" name="content" class="form-control" rows="7" placeholder="Share your thoughts…"></textarea>
                    <div class="d-flex justify-content-between mt-1">
                      <small class="text-secondary">Optional</small>
                      <small id="mvReviewCount" class="text-secondary">0 / 5000</small>
                    </div>
                  </div>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-outline-danger me-auto d-none" id="mvDeleteWrittenReview">Delete written review</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                <button type="submit" class="btn btn-primary" id="mvSaveReview">Save review</button>
              </div>
            </form>
          </div>
        </div>
      </div>`
    );

    const textarea =
      document.getElementById(
        "mvReviewContent"
      );

    textarea.addEventListener(
      "input",
      () => {
        document.getElementById(
          "mvReviewCount"
        ).textContent =
          `${textarea.value.length} / 5000`;
      }
    );
  }

  async function openEditor({
    type,
    id,
    title,
    onSaved
  } = {}) {
    if (!MV.auth.requireAuth()) {
      return;
    }

    ensureReviewModal();

    const modalElement =
      document.getElementById(
        "mvReviewModal"
      );

    const modal =
      bootstrap.Modal
        .getOrCreateInstance(
          modalElement
        );

    const form =
      document.getElementById(
        "mvReviewForm"
      );

    MV.ui.useBackendValidation(form);

    const ratingInput =
      document.getElementById(
        "mvReviewRating"
      );

    const contentInput =
      document.getElementById(
        "mvReviewContent"
      );

    const deleteBtn =
      document.getElementById(
        "mvDeleteWrittenReview"
      );

    const saveBtn =
      document.getElementById(
        "mvSaveReview"
      );

    document.getElementById(
      "mvReviewTitle"
    ).textContent =
      title ||
      "Write a review";

    let existing = null;

    try {
      existing =
        await getMine(type, id);
    } catch (err) {
      MV.ui.showError(err);
      return;
    }

    MV.ui.clearFormError(form);

    ratingInput.value =
      existing?.rating ?? "";

    contentInput.value =
      existing?.content ?? "";

    document.getElementById(
      "mvReviewCount"
    ).textContent =
      `${contentInput.value.length} / 5000`;

    deleteBtn.classList.toggle(
      "d-none",
      !existing?.content
    );

    const submitHandler =
      async event => {
        event.preventDefault();

        MV.ui.clearFormError(form);

        MV.ui.buttonBusy(
          saveBtn,
          true,
          "Saving…"
        );

        try {
          await save(
            type,
            id,
            ratingInput.value,
            contentInput.value.trim() ||
              null,
            existing
          );

          MV.ui.toast(
            "Review saved",
            "success"
          );

          modal.hide();

          if (onSaved) {
            await onSaved();
          }
        } catch (err) {
          MV.ui.showFormError(
            form,
            err,
            "Review could not be saved."
          );
        } finally {
          MV.ui.buttonBusy(
            saveBtn,
            false
          );
        }
      };

    const deleteHandler =
      async () => {
        if (!existing) return;

        const ok =
          await MV.ui.confirm({
            title:
              "Delete written review?",
            message:
              "Your rating will be kept. Only the written review text will be removed.",
            confirmText:
              "Delete review"
          });

        if (!ok) return;

        MV.ui.buttonBusy(
          deleteBtn,
          true,
          "Deleting…"
        );

        try {
          await deleteWrittenOnly(
            type,
            id,
            existing
          );

          MV.ui.toast(
            "Written review deleted; rating kept",
            "success"
          );

          modal.hide();

          if (onSaved) {
            await onSaved();
          }
        } catch (err) {
          MV.ui.showFormError(
            form,
            err,
            "Review could not be deleted."
          );
        } finally {
          MV.ui.buttonBusy(
            deleteBtn,
            false
          );
        }
      };

    form.addEventListener(
      "submit",
      submitHandler
    );

    deleteBtn.addEventListener(
      "click",
      deleteHandler
    );

    modalElement.addEventListener(
      "hidden.bs.modal",
      () => {
        form.removeEventListener(
          "submit",
          submitHandler
        );

        deleteBtn.removeEventListener(
          "click",
          deleteHandler
        );

        MV.ui.clearFormError(form);
      },
      { once: true }
    );

    modal.show();
  }

  function cards(reviews = []) {
    const written =
      (reviews || []).filter(
        review =>
          review.content &&
          String(
            review.content
          ).trim()
      );

    if (!written.length) {
      return MV.ui.emptyState({
        icon: "fa-message",
        title:
          "No written reviews yet",
        text:
          "Be the first to share a written review."
      });
    }

    return `
      <div class="d-grid gap-3">
        ${written
          .map(review => {
            const name =
              review.displayName ||
              review.userName;

            return `
              <article class="mv-review-card">
                <div class="mv-review-head">
                  <a class="mv-review-user" href="user-profile.html?username=${encodeURIComponent(review.userName)}">${MV.ui.escapeHtml(name)}</a>
                  <span class="mv-review-rating">
                    <i class="fa-solid fa-star"></i>
                    ${Number(review.rating).toFixed(1)}/10
                  </span>
                </div>
                <div class="mv-review-content">${MV.ui.escapeHtml(review.content)}</div>
              </article>`;
          })
          .join("")}
      </div>`;
  }

  return {
    getMine,
    getAll,
    save,
    removeEntire,
    deleteWrittenOnly,
    openEditor,
    cards,
    contextPath
  };
})();
