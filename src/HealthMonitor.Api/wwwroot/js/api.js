export class ApiError extends Error {
  constructor(status, message, details = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

async function request(method, url, body) {
  const options = {
    method,
    headers: { Accept: "application/json" },
  };

  if (body !== undefined) {
    options.headers["Content-Type"] = "application/json";
    options.body = JSON.stringify(body);
  }

  let response;

  try {
    response = await fetch(url, options);
  } catch (networkError) {
    throw new ApiError(
      0,
      "Cannot reach the server. Is it running?",
      networkError,
    );
  }

  if (response.status === 204) {
    return null;
  }

  const isJson = response.headers.get("content-type")?.includes("json");
  const data = isJson ? await response.json() : await response.text();

  if (!response.ok) {
    throw new ApiError(response.status, extractMessage(data, response), data);
  }

  return data;
}

function extractMessage(data, response) {
  if (data && typeof data === "object") {
    // Validation errors: { errors: { Name: ["The Name field is required."] } }
    if (data.errors) {
      return Object.values(data.errors).flat().join(" ");
    }

    return data.detail || data.title || `Request failed (${response.status})`;
  }

  return data || `Request failed (${response.status})`;
}

export const api = {
  get: (url) => request("GET", url),
  post: (url, body) => request("POST", url, body),
  put: (url, body) => request("PUT", url, body),
  delete: (url) => request("DELETE", url),
};
