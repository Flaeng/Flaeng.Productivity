const { executeAsync } = require("./helpers");

(async function () {
  await executeAsync("dotnet format");
  await executeAsync("dotnet test");
})();
