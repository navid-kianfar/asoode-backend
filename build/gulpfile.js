const path = require("path");
const fs = require("fs");
const gulp = require("gulp");
const run = require("gulp-run-command").default;

const applications = ['Api', 'Background'];
const environments = ['Development', 'Staging', 'Production'];
const config_file = "./Dockerfile.json";
const command_file = "./Dockerfile.txt";
const root = path.resolve(__dirname, '..');
const build = fs.readFileSync(command_file).toString();

const increment_version = (current) => {
    const parts = current.split('.');
    let major = +parts[0];
    let minor = +parts[1];
    let patch = +parts[2];
    
    patch++;
    if (patch > 100) {
        patch = 0;
        minor++;
    }
    if (minor > 10) {
        minor = 0;
        major++;
    }
    
    return `${major}.${minor}.${patch}`;
}
const generator = (app, target, must_increment) => {
    const app_lowered = app.toLowerCase();
    environments.forEach(env => {
        const env_lowered = env.toLowerCase();
        gulp.task(`${target}-${app_lowered}-${env_lowered}`, (callback) => {
            const appSetting = require(config_file);
            const version = appSetting[app][env];
            const newVersion = must_increment ? increment_version(version) : version;
            const exec_command = build
                .replaceAll('{APP_LOWERED}', app_lowered)
                .replaceAll('{ENV_LOWERED}', env_lowered)
                .replaceAll('{VERSION}', newVersion)
                .replaceAll('{ENV}', env)
                .replaceAll('{APP}', app);
            run(exec_command, { cwd: root })().then(() => {
                appSetting[app][env] = newVersion;
                fs.writeFileSync(config_file, JSON.stringify(appSetting));
                callback();
            });
        });
    });
}

applications.forEach(app => generator(app, 'build', false));
applications.forEach(app => generator(app, 'release', true));