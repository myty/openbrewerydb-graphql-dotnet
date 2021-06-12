import { Docker, Options } from "docker-cli-js";
import path from "path";
import { program } from "commander";
import { grep } from "shelljs";

var currentWorkingDirectory = path.join(__dirname, "..");

const dockerOptions = new Options(
    /* machineName */ undefined,
    /* currentWorkingDirectory */ currentWorkingDirectory,
    /* echo*/ true,
);

const docker = new Docker(dockerOptions);

const DEFAULT_CONTAINER_NAME = "openbrewerydb-sql-db";

const findDockerContainer = async (name: string) => {
    const data = await docker.command("ps");
    const containerList: any[] = data.containerList ?? [];
    return containerList.find((c) => c.names === name);
};

const runSqlDockerContainer = async (containerName: string): Promise<void> => {
    const dockerContainer = await findDockerContainer(containerName);
    if (dockerContainer == null) {
        await docker.command(
            `run --name ${containerName} -e ACCEPT_EULA=Y -e SA_PASSWORD=passw0rd! -p 9433:1433 -v sqlvolume:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2019-latest`,
        );

        // TODO - DUPLICATE THE FOLLOWING
        /*
            #!/bin/bash

            while ! docker logs container | grep -q "[services.d] done.";
            do
                sleep 1
                echo "working..."
            done
        */

        // const logs = await docker.command(`logs ${containerName}`);
        // grep
    }
};

const shutdownSqlDockerContainer = async (containerName: string): Promise<void> => {
    const foundContainer = await findDockerContainer(containerName);

    if (foundContainer != null) {
        await docker.command(`stop ${containerName}`);
        await docker.command(`rm -f ${containerName}`);
    }
};

program
    .command("db")
    .option(
        "-n, --container-name",
        `Container name (defaults to ${DEFAULT_CONTAINER_NAME})`,
        DEFAULT_CONTAINER_NAME,
    )
    .option("-s, --start", "Starts the database container")
    .option("-t, --stop", "Stops the database container")
    .action(async (cmdObj) => {
        if (cmdObj.start) {
            console.log("Starting docker container...");
            await runSqlDockerContainer(cmdObj.containerName);
            return;
        }

        if (cmdObj.stop) {
            console.log("Shutting down docker container...");
            await shutdownSqlDockerContainer(cmdObj.containerName);
        }
    });

const run = async () => {
    await program.parseAsync(process.argv);
};

run();
