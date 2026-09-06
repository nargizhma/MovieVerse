document.addEventListener("DOMContentLoaded", async () => {
  if (!MV.auth.isAdmin()) return;

  const host =
    document.getElementById(
      "usersTable"
    );

  const search =
    document.getElementById(
      "userSearch"
    );

  let users = [];

  try {
    users =
      await MV.api.get(
        "admin/users"
      );

    render();
  } catch (err) {
    host.innerHTML =
      MV.ui.emptyState({
        icon: "fa-users",
        title:
          "Could not load users",
        text:
          err.detail ||
          "Try again."
      });
  }

  search.addEventListener(
    "input",
    MV.ui.debounce(
      render,
      180
    )
  );

  function render() {
    const query =
      search.value
        .trim()
        .toLowerCase();

    const rows =
      users.filter(user =>
        !query ||
        [
          user.userName,
          user.email,
          user.displayName
        ].some(value =>
          String(value || "")
            .toLowerCase()
            .includes(query)
        )
      );

    if (!rows.length) {
      host.innerHTML =
        MV.ui.emptyState({
          icon: "fa-users",
          title:
            "No users found"
        });

      return;
    }

    const canEdit =
      MV.auth.isSuperAdmin();

    const me =
      MV.auth.getUser();

    host.innerHTML = `
      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead>
            <tr>
              <th>User</th>
              <th>Email</th>
              <th>Roles</th>
              <th>Reviews</th>
              <th>Watchlist</th>
              <th>Watched</th>
              ${canEdit ? "<th>Role management</th>" : ""}
            </tr>
          </thead>
          <tbody>
            ${rows
              .map(
                user => `
                  <tr data-user-id="${user.id}">
                    <td>
                      <strong>${MV.ui.escapeHtml(user.displayName || user.userName)}</strong>
                      <div class="text-secondary small">@${MV.ui.escapeHtml(user.userName)}</div>
                    </td>
                    <td>${MV.ui.escapeHtml(user.email)}</td>
                    <td>
                      ${(user.roles || [])
                        .map(
                          role =>
                            `<span class="badge mv-badge me-1">${MV.ui.escapeHtml(role)}</span>`
                        )
                        .join("")}
                    </td>
                    <td>${user.reviewCount}</td>
                    <td>${user.watchlistCount}</td>
                    <td>${user.watchHistoryCount}</td>
                    ${
                      canEdit
                        ? `
                          <td>
                            ${
                              String(user.id) === String(me.id)
                                ? '<span class="text-secondary small">Current account</span>'
                                : `
                                  <div class="d-flex gap-2">
                                    <select class="form-select form-select-sm js-role" data-id="${user.id}">
                                      ${["User", "Admin", "SuperAdmin"]
                                        .map(
                                          role =>
                                            `<option ${user.roles?.includes(role) ? "selected" : ""}>${role}</option>`
                                        )
                                        .join("")}
                                    </select>
                                    <button class="btn btn-sm btn-outline-primary js-save-role" data-id="${user.id}">Save</button>
                                  </div>
                                  <div class="js-role-error mt-2"></div>`
                            }
                          </td>`
                        : ""
                    }
                  </tr>`
              )
              .join("")}
          </tbody>
        </table>
      </div>`;

    host
      .querySelectorAll(
        ".js-save-role"
      )
      .forEach(button =>
        button.addEventListener(
          "click",
          () =>
            saveRole(button)
        )
      );
  }

  async function saveRole(button) {
    const row =
      button.closest(
        "tr[data-user-id]"
      );

    const select =
      row?.querySelector(
        ".js-role"
      );

    const errorHost =
      row?.querySelector(
        ".js-role-error"
      );

    if (!select) return;

    MV.ui.clearInlineError(
      errorHost
    );

    MV.ui.buttonBusy(
      button,
      true,
      "Saving…"
    );

    try {
      await MV.api.put(
        `admin/users/${button.dataset.id}/role`,
        {
          role:
            select.value
        }
      );

      MV.ui.toast(
        "User role updated",
        "success"
      );

      users =
        await MV.api.get(
          "admin/users"
        );

      render();
    } catch (err) {
      MV.ui.showInlineError(
        errorHost,
        err,
        "Role could not be updated."
      );
    } finally {
      MV.ui.buttonBusy(
        button,
        false
      );
    }
  }
});
