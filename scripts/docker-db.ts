import { Docker, Options } from "docker-cli-js";
import path from "path";
import { program } from "commander";
import { exec, execSync, spawn } from "child_process";

var currentWorkingDirectory = path.join(__dirname, "..");

const dockerOptions = new Options(
    /* machineName */ undefined,
    /* currentWorkingDirectory */ currentWorkingDirectory,
    /* echo*/ true,
);

const docker = new Docker(dockerOptions);

const DEFAULT_CONTAINER_NAME = "openbrewerydb-sql-db";

type ContainerState = "exited" | "running";

interface ContainerListItem {
    Command: string;
    CreatedAt: string;
    ID: string;
    Image: string;
    Labels: string;
    LocalVolumes: string;
    Mounts: string;
    Names: string;
    Networks: string;
    Ports: string;
    RunningFor: string;
    Size: string;
    State: ContainerState;
    Status: string;
}

const findDockerContainer = async (
    name: string,
): Promise<ContainerListItem | undefined> => {
    const containerList = await getContainerList();
    return containerList.find((c) => c.Names === name);
};

const getContainerList = async (): Promise<Array<ContainerListItem>> =>
    new Promise((resolve) => {
        const parsedContainerList: Array<ContainerListItem> = [];

        spawn("docker", ["container", "ls", "-a", "--no-trunc", "--format='{{json .}}'"])
            .stdout?.on("data", (chunk: Buffer) => {
                const chunkedString = chunk.toString();
                const jsonString = chunkedString.substring(1, chunkedString.length - 2);

                parsedContainerList.push(JSON.parse(jsonString));
            })
            .on("close", () => {
                resolve(parsedContainerList);
            });
    });

const runSqlDockerContainer = async (containerName: string): Promise<void> => {
    const dockerContainer = await findDockerContainer(containerName);

    if (dockerContainer?.State === "running") {
        return;
    }

    if (dockerContainer == null) {
        await docker.command(
            `run --name ${containerName} -e ACCEPT_EULA=Y -e SA_PASSWORD=passw0rd! -p 9433:1433 -v sqlvolume:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2019-latest`,
        );
    }

    if (dockerContainer?.State === "exited") {
        await docker.command(`start ${containerName}`);
    }

    await waitForLogs(containerName, /Server is listening on(.*)/);
};

const waitForLogs = async (containerName: string, logPattern: RegExp): Promise<void> => {
    try {
        const containerLogs = spawn(
            "docker",
            ["logs", containerName, "-f", "--since=10s"],
            {
                shell: false,
            },
        );

        await new Promise<void>((resolve) => {
            containerLogs.stdout.on("data", (chunk: string) => {
                const matches = logPattern.exec(chunk) ?? [];
                if (matches.length > 0) {
                    resolve();
                }
            });
        });

        killPID(containerLogs.pid);
    } catch (error) {
        console.error(error);
    }
};

const killPID = (pid: number) => {
    var isWin = /^win/.test(process.platform);
    if (!isWin) {
        process.kill(pid);
    } else {
        exec("taskkill /PID " + pid + " /T /F");
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
