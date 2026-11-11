// Entry point - loaded by index.html as <script type="module">.
import { startRouter } from "./router.js";

startRouter(document.getElementById("app"));
