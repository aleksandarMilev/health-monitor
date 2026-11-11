// Reference page: shows every common pattern in one place.
//   - load data from the API
//   - render a table (with escaping!)
//   - draw a Chart.js line chart
//   - submit a form (POST) and delete rows (DELETE)
//   - clean up in destroy()

import { api } from "../../api.js";
import { createLineChart } from "../../charts.js";
import {
  escapeHtml,
  formatDateTime,
  formatShortDateTime,
  showMessage,
  hideMessage,
} from "../../utils.js";

let chart = null;
let elements = {};

export async function init(root) {
  elements = {
    message: root.querySelector("#message"),
    form: root.querySelector("#sample-form"),
    tableBody: root.querySelector("#samples-body"),
    canvas: root.querySelector("#samples-chart"),
  };

  elements.form.addEventListener("submit", onSubmit);
  elements.tableBody.addEventListener("click", onTableClick);

  await loadSamples();
}

export function destroy() {
  chart?.destroy();
  chart = null;
}

// Data
async function loadSamples() {
  try {
    const samples = await api.get("/api/samples");
    renderTable(samples);
    renderChart(samples);
  } catch (error) {
    showMessage(elements.message, error.message, "error");
  }
}

// Rendering
function renderTable(samples) {
  if (samples.length === 0) {
    elements.tableBody.innerHTML =
      '<tr><td colspan="5" class="muted">No samples yet.</td></tr>';
    return;
  }

  // Newest first in the table.
  const rows = [...samples].reverse().map(
    (s) => `
    <tr>
      <td>${escapeHtml(s.name)}</td>
      <td>${escapeHtml(s.value)}</td>
      <td>${escapeHtml(s.unit)}</td>
      <td>${formatDateTime(s.recordedAt)}</td>
      <td><button class="btn btn-danger btn-sm" data-action="delete" data-id="${s.id}">Delete</button></td>
    </tr>`,
  );

  elements.tableBody.innerHTML = rows.join("");
}

function renderChart(samples) {
  // x-axis: every distinct timestamp, in order (the API returns them sorted).
  const timestamps = [...new Set(samples.map((s) => s.recordedAt))];
  const labels = timestamps.map(formatShortDateTime);

  // One line per sample name. Missing points are null (Chart.js skips them).
  const names = [...new Set(samples.map((s) => s.name))];
  const datasets = names.map((name) => ({
    label: name,
    data: timestamps.map(
      (t) =>
        samples.find((s) => s.name === name && s.recordedAt === t)?.value ??
        null,
    ),
  }));

  chart?.destroy();
  chart = createLineChart(elements.canvas, labels, datasets);
}

// Events
async function onSubmit(event) {
  event.preventDefault();
  hideMessage(elements.message);

  const formData = new FormData(elements.form);
  const recordedAt = formData.get("recordedAt");

  const payload = {
    name: formData.get("name")?.trim(),
    value: formData.get("value") === "" ? null : Number(formData.get("value")),
    unit: formData.get("unit")?.trim() || null,
    // datetime-local gives local time without a zone -> convert to ISO (UTC).
    recordedAt: recordedAt ? new Date(recordedAt).toISOString() : null,
  };

  try {
    await api.post("/api/samples", payload);
    elements.form.reset();
    showMessage(elements.message, "Sample added.", "success");
    await loadSamples();
  } catch (error) {
    // Server-side validation messages (e.g. "The Name field is required.") end up here.
    showMessage(elements.message, error.message, "error");
  }
}

async function onTableClick(event) {
  const button = event.target.closest('[data-action="delete"]');
  if (!button) {
    return;
  }

  if (!confirm("Delete this sample?")) {
    return;
  }

  try {
    await api.delete(`/api/samples/${button.dataset.id}`);
    await loadSamples();
  } catch (error) {
    showMessage(elements.message, error.message, "error");
  }
}
