window.MV = window.MV || {};

MV.filmChat = (() => {
  let history = [];
  let sending = false;

  function storageKey() {
    const userId =
      MV.auth.getUser()?.id || "guest";

    return `movieverse_film_chat_${userId}`;
  }

  function loadHistory() {
    try {
      const value =
        JSON.parse(
          sessionStorage.getItem(
            storageKey()
          ) || "[]"
        );

      if (!Array.isArray(value))
        return [];

      return value
        .filter(item =>
          item &&
          ["user", "model"].includes(
            item.role
          ) &&
          typeof item.text === "string" &&
          item.text.trim()
        )
        .slice(-12);
    }
    catch {
      return [];
    }
  }

  function saveHistory() {
    sessionStorage.setItem(
      storageKey(),
      JSON.stringify(
        history.slice(-12)
      )
    );
  }

  function render() {
    if (!MV.auth.isAuthenticated())
      return;

    if (
      document.getElementById(
        "mvFilmChatButton"
      )
    ) {
      return;
    }

    history = loadHistory();

    document.body.insertAdjacentHTML(
      "beforeend",
      `
        <button
          id="mvFilmChatButton"
          class="mv-film-chat-button"
          type="button"
          aria-label="Open MovieVerse AI">

          <i class="fa-solid fa-wand-magic-sparkles"></i>

        </button>

        <section
          id="mvFilmChatPanel"
          class="mv-film-chat-panel d-none"
          aria-label="MovieVerse AI chat">

          <div class="mv-film-chat-header">

            <div>
              <strong>MovieVerse AI</strong>

              <small>
                Film & TV recommendations only
              </small>
            </div>

            <div class="d-flex gap-1">

              <button
                id="mvFilmChatClear"
                class="mv-film-chat-header-btn"
                type="button"
                title="Clear chat"
                aria-label="Clear chat">

                <i class="fa-solid fa-trash-can"></i>

              </button>

              <button
                id="mvFilmChatClose"
                class="mv-film-chat-header-btn"
                type="button"
                aria-label="Close chat">

                <i class="fa-solid fa-xmark"></i>

              </button>

            </div>

          </div>

          <div
            id="mvFilmChatMessages"
            class="mv-film-chat-messages">
          </div>

          <form
            id="mvFilmChatForm"
            class="mv-film-chat-form">

            <textarea
              id="mvFilmChatInput"
              class="form-control"
              rows="2"
              maxlength="500"
              placeholder="Ask for a movie or TV show recommendation…"
              aria-label="Message MovieVerse AI"></textarea>

            <button
              id="mvFilmChatSend"
              class="btn btn-primary"
              type="submit"
              aria-label="Send message">

              <i class="fa-solid fa-paper-plane"></i>

            </button>

          </form>

        </section>
      `
    );

    bindEvents();
    renderMessages();
  }

  function bindEvents() {
    const button =
      document.getElementById(
        "mvFilmChatButton"
      );

    const panel =
      document.getElementById(
        "mvFilmChatPanel"
      );

    const close =
      document.getElementById(
        "mvFilmChatClose"
      );

    const clear =
      document.getElementById(
        "mvFilmChatClear"
      );

    const form =
      document.getElementById(
        "mvFilmChatForm"
      );

    const input =
      document.getElementById(
        "mvFilmChatInput"
      );

    button?.addEventListener(
      "click",
      () => {
        panel?.classList.toggle(
          "d-none"
        );

        if (
          !panel?.classList.contains(
            "d-none"
          )
        ) {
          input?.focus();
          scrollToBottom();
        }
      }
    );

    close?.addEventListener(
      "click",
      () => {
        panel?.classList.add(
          "d-none"
        );
      }
    );

    clear?.addEventListener(
      "click",
      () => {
        history = [];

        sessionStorage.removeItem(
          storageKey()
        );

        renderMessages();
      }
    );

    form?.addEventListener(
      "submit",
      sendMessage
    );

    input?.addEventListener(
      "keydown",
      event => {
        if (
          event.key === "Enter" &&
          !event.shiftKey
        ) {
          event.preventDefault();
          form?.requestSubmit();
        }
      }
    );
  }

  function renderMessages() {
    const host =
      document.getElementById(
        "mvFilmChatMessages"
      );

    if (!host) return;

    host.innerHTML = "";

    if (!history.length) {
      appendMessage(
        "model",
        "Tell me what you feel like watching. For example: “Recommend a sci-fi movie like Interstellar, but shorter.”"
      );

      return;
    }

    history.forEach(message =>
      appendMessage(
        message.role,
        message.text
      )
    );

    scrollToBottom();
  }

  function appendMessage(
    role,
    text
  ) {
    const host =
      document.getElementById(
        "mvFilmChatMessages"
      );

    if (!host) return;

    const row =
      document.createElement("div");

    row.className =
      `mv-film-chat-message ${
        role === "user"
          ? "is-user"
          : "is-model"
      }`;

    const bubble =
      document.createElement("div");

    bubble.className =
      "mv-film-chat-bubble";

    bubble.textContent = text;

    row.appendChild(bubble);
    host.appendChild(row);
  }

  async function sendMessage(event) {
    event.preventDefault();

    if (sending) return;

    const input =
      document.getElementById(
        "mvFilmChatInput"
      );

    const sendButton =
      document.getElementById(
        "mvFilmChatSend"
      );

    const message =
      input?.value.trim() || "";

    if (!message)
      return;

    const previousHistory =
      history.slice(-12);

    history.push({
      role: "user",
      text: message
    });

    history =
      history.slice(-12);

    saveHistory();
    renderMessages();

    if (input)
      input.value = "";

    sending = true;

    if (input)
      input.disabled = true;

    if (sendButton)
      sendButton.disabled = true;

    appendTyping();
    scrollToBottom();

    try {
      const response =
        await MV.api.post(
          "ai/film-chat",
          {
            message,
            history:
              previousHistory
          }
        );

      removeTyping();

      const reply =
        response?.reply ||
        "I could not generate a recommendation right now.";

      history.push({
        role: "model",
        text: reply
      });

      history =
        history.slice(-12);

      saveHistory();
      renderMessages();
    }
    catch (error) {
      removeTyping();

      appendMessage(
        "model",
        error?.detail ||
        "MovieVerse AI is unavailable right now. Please try again."
      );
    }
    finally {
      sending = false;

      if (input) {
        input.disabled = false;
        input.focus();
      }

      if (sendButton)
        sendButton.disabled = false;

      scrollToBottom();
    }
  }

  function appendTyping() {
    const host =
      document.getElementById(
        "mvFilmChatMessages"
      );

    if (!host) return;

    const row =
      document.createElement("div");

    row.id =
      "mvFilmChatTyping";

    row.className =
      "mv-film-chat-message is-model";

    row.innerHTML = `
      <div class="mv-film-chat-bubble mv-film-chat-typing">
        <span></span>
        <span></span>
        <span></span>
      </div>
    `;

    host.appendChild(row);
  }

  function removeTyping() {
    document
      .getElementById(
        "mvFilmChatTyping"
      )
      ?.remove();
  }

  function scrollToBottom() {
    const host =
      document.getElementById(
        "mvFilmChatMessages"
      );

    if (!host) return;

    host.scrollTop =
      host.scrollHeight;
  }

  document.addEventListener(
    "DOMContentLoaded",
    render
  );

  return {
    render
  };
})();