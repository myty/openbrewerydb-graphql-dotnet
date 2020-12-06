import { Docker, Options } from "docker-cli-js";
import path from "path";
import { program } from "commander";

var currentWorkingDirectory = path.join(__dirname, "..");

const dockerOptions = new Options(
    /* machineName */ undefined,
    /* currentWorkingDirectory */ currentWorkingDirectory,
    /* echo*/ false
);

const docker = new Docker(dockerOptions);

const containerName = "sql-test-db";

const findDockerContainer = async (name: string) => {
    const data = await docker.command("ps");
    const containerList: any[] = data.containerList ?? [];
    return containerList.find((c) => c.names === name);
};

const runSqlDockerContainer = async (): Promise<void> => {
    const dockerContainer = await findDockerContainer(containerName);
    if (dockerContainer == null) {
        await docker.command(
            `run --name ${containerName} -e ACCEPT_EULA=Y -e SA_PASSWORD=passw0rd! -p 9433:1433 -d mcr.microsoft.com/mssql/server:2019-latest`
        );
    }
};

const shutdownSqlDockerContainer = async (): Promise<void> => {
    const foundContainer = await findDockerContainer(containerName);

    if (foundContainer != null) {
        await docker.command(`stop ${containerName}`);
        await docker.command(`rm -f ${containerName}`);
    }
};

program
    .command("db")
    .option("-s, --start", "Starts the database container")
    .option("-t, --stop", "Starts the database container")
    .action(async (cmdObj) => {
        if (cmdObj.start) {
            console.log("Starting docker container...");
            await runSqlDockerContainer();
            return;
        }

        if (cmdObj.stop) {
            console.log("Shutting down docker container...");
            await shutdownSqlDockerContainer();
        }
    });

const run = async () => {
    await program.parseAsync(process.argv);
};

run();
