const routes = {
  "/": "home",
  "/samples": "samples",
  "/external": "external",
  // '/dashboard': 'dashboard',
};

const NOT_FOUND_PAGE = "not-found";

const templateCache = new Map();

let currentPage = null;
let appRoot = null;

export function startRouter(root) {
  appRoot = root;
  window.addEventListener("hashchange", render);
  render();
}

/** Navigate from code, e.g. navigate('/samples'). */
export function navigate(path) {
  window.location.hash = `#${path}`;
}

async function render() {
  const path = getPath();
  const pageName = routes[path] ?? NOT_FOUND_PAGE;

  try {
    currentPage?.destroy?.();
    currentPage = null;

    const html = await loadTemplate(pageName);
    appRoot.innerHTML = html;

    const module = await import(`./pages/${pageName}/${pageName}.js`);
    currentPage = module;
    await module.init?.(appRoot);

    highlightNav(path);
    window.scrollTo(0, 0);
  } catch (err) {
    console.error(`Failed to load page "${pageName}"`, err);
    appRoot.innerHTML = `<div class="alert alert-error">Failed to load page "${pageName}". Check the browser console.</div>`;
  }
}

function getPath() {
  // "#/samples?x=1" -> "/samples"
  const hash = window.location.hash.slice(1) || "/";
  return hash.split("?")[0] || "/";
}

async function loadTemplate(pageName) {
  if (templateCache.has(pageName)) {
    return templateCache.get(pageName);
  }

  const response = await fetch(`/js/pages/${pageName}/${pageName}.html`);
  if (!response.ok) {
    throw new Error(
      `Template not found for page "${pageName}" (HTTP ${response.status})`,
    );
  }

  const html = await response.text();
  templateCache.set(pageName, html);
  return html;
}

function highlightNav(path) {
  document.querySelectorAll("[data-route]").forEach((link) => {
    link.classList.toggle("active", link.dataset.route === path);
  });
}
