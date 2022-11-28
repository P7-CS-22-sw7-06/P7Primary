using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

public class ContainerController
{
    public ContainerController(DockerClient Client)
    {
        if (Client is null)
        {
            throw new ArgumentNullException(nameof(Client));
        }

        Client = client;
    }

    #region Variables

    DockerClient client { get; set; } = default!;

    string PathToContainers = $@"/var/lib/docker/containers/";

    #endregion Variables

    #region Methods

    public async Task CreateImageAsync(string image)
    {
        // // Initialize Container StreamReader
        // Stream stream = await client.System.MonitorEventsAsync(
        //     new ContainerEventsParameters(),
        //     new Progress<JSONMessage>(),
        //     CancellationToken.None);

        await client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = image
            },
            // stream,
            null,
            new Progress<JSONMessage>(),
            CancellationToken.None);

        Log.Information($"Created image: {image}");
    }

    public async Task CreateContainerAsync(string name, string image, string payloadDirectory)
    {
        // Read payload file
        IList<string> payloads = new List<string>
            { File.ReadAllText(payloadDirectory) };

        // Create Container
        await client.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = image,
            Name = name,
            Cmd = payloads
            // TODO: Add arbitrary arguments // TODO: Check if necessary
        },
        CancellationToken.None);

        // TODO: Get Container ID

        // TODO: Log.Information($"Created Container, id: {id}, name: {name}");
    }

    public async Task DeleteContainerAsync(string id)
    {
        await client.Containers.RemoveContainerAsync(
            id,
            new ContainerRemoveParameters
            {
                Force = true,
            },
            CancellationToken.None);

        Log.Information($"Deleted Container: {id}");
    }

    public async Task ListAvailableContainersAsync()
    {
        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = 10,
            },
            CancellationToken.None);

        Log.Information($"Listing Containers: \n{containers}");
    }

    public async Task StartAsync(string id)
    {
        await client.Containers.StartContainerAsync(
            id,
            new ContainerStartParameters(),
            CancellationToken.None);

        Log.Information($"Started container: {id}");
    }

    public async Task StopContainer(string id)
    {
        await client.Containers.StopContainerAsync(
            id,
            new ContainerStopParameters
            {
                WaitBeforeKillSeconds = 30
            },
            CancellationToken.None);

        Log.Information($"Stopped container: {id}");

    }

    public async void Execute(string id)
    {
        await client.Exec.StartContainerExecAsync(
            id,
            CancellationToken.None
        );

        Log.Information($"Container is running: {id}");

        while (await ContainerIsRunningAsync(id) == true)
        {
            Thread.Sleep(500);

            for (int i = 1; i > 0; i++)
            {
                Checkpoint(id, $"checkpoint-{id}" + i);
            }
        }
    }

    public void Checkpoint(string id, string checkpointName)
    {
        Process p = Process.Start(
            "bash",
            $@"docker checkpoint create {id} {checkpointName} --leave-running");

        p.WaitForExit();

        Log.Information($"Stopped container: {id}");
    }

    public async Task RestoreAsync(
        string id,
        string checkpointName,
        string containerName,
        string payload,
        string image)
    {
        await CreateContainerAsync(containerName, image, payload);
        // TODO: Arguments --security-opt seccomp:unconfined // TODO: Check if necessary

        Process p = Process.Start(
            "bash",
            $@"docker start {containerName} --checkpoint {checkpointName}");

        p.WaitForExit();

        Log.Information($"Restored container, id: {id}, checkpoint: {checkpointName}");
    }

    public async Task<bool> ContainerIsRunningAsync(string id)
    {
        var isRunning = await client.Tasks.InspectAsync(id, CancellationToken.None);
        // TODO: Get working

        // string strCmdText = $"docker ps | grep {Name}";
        // Process p = Process.Start("bash", strCmdText);
        // p.WaitForExit();

        // if (p.HasExited && p.StandardOutput.ToString() == "")
        // {
        //     return false;
        // }
        // return true;

        return true;
    }

    #endregion Methods
}
