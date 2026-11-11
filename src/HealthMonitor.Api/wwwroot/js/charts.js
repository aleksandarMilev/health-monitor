const palette = [
  "#4c8dff",
  "#2fbf71",
  "#f2b632",
  "#ef4f4f",
  "#b07cff",
  "#36c5d6",
];

function readCssVar(name, fallback) {
  const value = getComputedStyle(document.documentElement)
    .getPropertyValue(name)
    .trim();

  return value || fallback;
}

/**
 * Line chart for values over time.
 * @param {HTMLCanvasElement} canvas
 * @param {string[]} labels     x-axis labels
 * @param {{label: string, data: (number|null)[]}[]} datasets
 * @param {object} [extraOptions] merged into Chart.js options
 */
export function createLineChart(canvas, labels, datasets, extraOptions = {}) {
  if (typeof window.Chart === "undefined") {
    throw new Error(
      "Chart.js is not loaded. Check the <script> tag in index.html.",
    );
  }

  const textColor = readCssVar("--color-text-muted", "#9aa6bd");
  const gridColor = readCssVar("--color-border", "#2c3852");

  return new window.Chart(canvas, {
    type: "line",
    data: {
      labels,
      datasets: datasets.map((ds, i) => ({
        borderColor: palette[i % palette.length],
        backgroundColor: palette[i % palette.length],
        tension: 0.3,
        spanGaps: true,
        pointRadius: 3,
        ...ds,
      })),
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      interaction: { mode: "index", intersect: false },
      plugins: {
        legend: { labels: { color: textColor } },
      },
      scales: {
        x: { ticks: { color: textColor }, grid: { color: gridColor } },
        y: { ticks: { color: textColor }, grid: { color: gridColor } },
      },
      ...extraOptions,
    },
  });
}
