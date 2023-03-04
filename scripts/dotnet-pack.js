const { executeAsync } = require("./helpers");
const fs = require("fs");

(async function () {
  fs.copyFileSync("../LICENSE", "../Flaeng.Productivity/LICENSE");
  fs.copyFileSync("../README.md", "../Flaeng.Productivity/README.md");
  executeAsync(
    `dotnet pack --include-symbols --include-source --configuration Release`,
    `../Flaeng.Productivity`
  );
})();
