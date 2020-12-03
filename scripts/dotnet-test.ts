import spawn from "cross-spawn";
import { Docker, Options } from "docker-cli-js";
import path from "path";

var currentWorkingDirectory = path.join(__dirname, "..");

const dockerOptions = new Options(
    /* machineName */ undefined,
    /* currentWorkingDirectory */ currentWorkingDirectory,
    /* echo*/ false
);

const docker = new Docker(dockerOptions);

const containerName = "sql-test-db";

const runSqlDockerContainer = async (): Promise<any> => {
    await shutdownSqlDockerContainer();

    return await docker.command(
        `run --name ${containerName} -e ACCEPT_EULA=Y -e SA_PASSWORD=passw0rd! -p 9433:1433 -d mcr.microsoft.com/mssql/server:2019-latest`
    );
};

const shutdownSqlDockerContainer = async (): Promise<void> => {
    const data = await docker.command("ps");
    const containerList: any[] = data.containerList ?? [];
    const foundContainer = containerList.find((c) => c.names === containerName);

    if (foundContainer != null) {
        await docker.command(`stop ${containerName}`);
        await docker.command(`rm -f ${containerName}`);
    }
};

const run = async () => {
    console.log("Starting docker container");
    const dockerResult = await runSqlDockerContainer();
    console.log(dockerResult.containerId);

    console.log("Running tests");
    spawn.sync("dotnet", ["test", "./dotnet/"], { stdio: "inherit" });

    console.log("Shutting down docker container");
    await shutdownSqlDockerContainer();
};

run();
