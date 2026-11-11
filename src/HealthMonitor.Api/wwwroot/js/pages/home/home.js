import { api } from "../../api.js";
import { escapeHtml, formatDateTime } from "../../utils.js";

export async function init(root) {
  const statusText = root.querySelector("#status-text");

  try {
    const status = await api.get("/api/status");
    statusText.innerHTML =
      `<span class="badge status-ok">Online</span> ` +
      `environment: <strong>${escapeHtml(status.environment)}</strong>, ` +
      `server time: ${formatDateTime(status.serverTimeUtc)}`;
  } catch (error) {
    statusText.innerHTML = `<span class="badge status-critical">Offline</span> ${escapeHtml(error.message)}`;
  }
}
