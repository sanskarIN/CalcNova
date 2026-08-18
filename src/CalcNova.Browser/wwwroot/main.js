import { dotnet } from "./_framework/dotnet.js";

const runtime = await dotnet
  .withDiagnosticTracing(false)
  .withApplicationArgumentsFromQuery()
  .create();

await runtime.runMain();

if ("serviceWorker" in navigator) {
  globalThis.addEventListener("load", () => {
    navigator.serviceWorker.register("./service-worker.js").catch((error) => {
      console.warn("CalcNova service worker registration failed.", error);
    });
  });
}
