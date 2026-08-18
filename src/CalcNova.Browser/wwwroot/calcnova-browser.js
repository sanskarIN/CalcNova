export function getItem(key) {
  return globalThis.localStorage.getItem(key);
}

export function setItem(key, value) {
  globalThis.localStorage.setItem(key, value);
}

export function removeItem(key) {
  globalThis.localStorage.removeItem(key);
}

export function openExternal(url) {
  const parsed = new URL(url, globalThis.location.href);
  if (parsed.protocol === "http:" || parsed.protocol === "https:") {
    globalThis.open(parsed.href, "_blank", "noopener,noreferrer");
    return;
  }

  if (parsed.protocol === "mailto:") {
    globalThis.location.href = parsed.href;
    return;
  }

  throw new Error("Unsupported external link scheme.");
}
