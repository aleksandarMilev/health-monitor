import { api } from "../../api.js";
import { showMessage, hideMessage } from "../../utils.js";

export function init(root) {
  const form = root.querySelector("#external-form");
  const message = root.querySelector("#message");
  const output = root.querySelector("#output");

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    hideMessage(message);

    const path = new FormData(form).get("path").trim();
    output.textContent = "Loading...";

    try {
      const data = await api.get(
        `/api/external?path=${encodeURIComponent(path)}`,
      );

      // textContent (not innerHTML) - external data is never trusted.
      output.textContent =
        typeof data === "string" ? data : JSON.stringify(data, null, 2);
    } catch (error) {
      output.textContent = "";
      showMessage(message, error.message, "error");
    }
  });
}
