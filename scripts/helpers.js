const { spawn } = require('child_process');
const process = require('process');
const fs = require('fs');

function makeDir(path, deleteIfExists) {
  const split = path.split('/');
  let pathBuilder = './';
  for (let index = 0; index < split.length; index++) {
    pathBuilder += split[index] + '/';
    if (deleteIfExists && index == split.length - 1) {
      if (fs.existsSync(pathBuilder))
        fs.rmdirSync(pathBuilder, { recursive: true, force: true });
    }
    if (fs.existsSync(pathBuilder) === false) fs.mkdirSync(pathBuilder);
  }
}

function getArgument(prefix) {
  const args = process.argv;
  prefix = `--${prefix}`;
  let returnNext = false;
  for (let index = 0; index < args.length; index++) {
    const argument = args[index];
    if (argument.startsWith(`${prefix}=`)) {
      return argument.substring(prefix.length + 1);
    } else if (argument == `${prefix}`) {
      returnNext = true;
    } else if (returnNext) {
      return argument;
    }
  }
  return returnNext;
}

function getIntArgument(prefix) {
  const argString = getArgument(prefix);

  if (!argString) {
    console.error(`No version was provided (using --${prefix}=[number])`);
    return { success: false };
  }
  const argInt = parseInt(argString, 10);
  if (Number.isNaN(argInt)) {
    console.error(`Provided version is not a number: ${argString}`);
    return { success: false };
  }
  return { success: true, value: argInt };
}

async function executeAsync(cmd, path) {
  const _options = Object.assign({}, { shell: true, stdio: 'inherit' });
  _options.cwd = path;
  console.log(`${path}> ${cmd}`);
  const child = spawn(cmd, _options);
  return new Promise((res, rej) => {
    child.on('exit', function (err) {
      if (err) rej(err);
      else res();
    });
  });
}

function copyFolder(source, destination, isRecursiveCall) {
  const files = fs.readdirSync(source);
  if (isRecursiveCall !== true) console.log('Copying files...');
  files.forEach((file) => {
    const sourcePath = `${source}/${file}`;
    makeDir(destination, false);
    const destinationPath = `${destination}/${file}`;
    if (fs.lstatSync(sourcePath).isDirectory()) {
      makeDir(destinationPath);
      copyFolder(sourcePath, destinationPath, true);
    } else {
      console.log(sourcePath);
      fs.copyFileSync(sourcePath, destinationPath);
    }
  });
}

module.exports = {
  makeDir,
  getArgument,
  getIntArgument,
  executeAsync,
  copyFolder,
};
