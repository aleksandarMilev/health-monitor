/** Escape user/API text before putting it into innerHTML (prevents XSS). */
export function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

/** "2026-11-14T08:00:00Z" -> local, human-readable date + time. */
export function formatDateTime(iso) {
  if (!iso) {
    return "";
  }

  return new Date(iso).toLocaleString(undefined, {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/** "2026-11-14T08:00:00Z" -> short label for chart axes, e.g. "Nov 14, 08:00". */
export function formatShortDateTime(iso) {
  if (!iso) {
    return "";
  }

  return new Date(iso).toLocaleString(undefined, {
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/** Show a message in an element with class "alert". type: 'error' | 'success' | 'info'. */
export function showMessage(element, text, type = "info") {
  element.textContent = text;
  element.className = `alert alert-${type}`;
  element.classList.remove("hidden");
}

export function hideMessage(element) {
  element.classList.add("hidden");
}
