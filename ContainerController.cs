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
    #region Variables

    DockerClient client = new DockerClientConfiguration(
            new Uri("unix:///var/run/docker.sock")).CreateClient();

    string PathToContainers = $@"/var/lib/docker/containers/";

    #endregion Variables

    #region Methods

    public async Task CreateImageAsync(string imageName)
    {
        await client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = "latest"
            },
            null,
            new Progress<JSONMessage>(),
            CancellationToken.None);

        Log.Information($"Created image: {imageName}");
    }

    public async Task CreateContainerAsync(string name, string image, string payloadDirectory)
    {
        // Create Container
        await client.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = image,
            Name = name,
            // TODO: Add arbitrary arguments // TODO: Check if necessary
        },
        CancellationToken.None);


        string id = await GetContainerIDByNameAsync(name);

        await client.Containers.ExtractArchiveToContainerAsync(id, new ContainerPathStatParameters
        {
            Path = payloadDirectory
        },
        null,
        CancellationToken.None);

        Log.Information($"Created Container, id: {id}, name: {name}");
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

    public async Task<string> GetContainerIDByNameAsync(string containerName)
    {
        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = 10,
            },
            CancellationToken.None);
        Console.WriteLine(containers[0]);

        string containerID;

        foreach (var container in containers)
        {
            if (container.Names.Contains("containerName"))
            {
                containerID = container.ID;

                Log.Information($"Container {containerName} has id: \n{containerID}");

                return containerID;
            }
        }

        return "";
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

    public async void Execute(string id, int interval)
    {
        await client.Exec.StartContainerExecAsync(
            id,
            CancellationToken.None
        );

        Log.Information($"Container is running: {id}");

        while (await ContainerIsRunningAsync(id) == true)
        {
            Thread.Sleep(interval);

            for (int i = 1; i > 0; i++)
            {
                Checkpoint(id, $"checkpoint-{id}" + i);
            }
        }
    }

    public void Checkpoint(string id, string checkpointName)
    {

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"checkpoint create {id} {checkpointName} --leave-running";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        // Process process = Process.Start(
        //     "bash",
        //     $@"docker checkpoint create {id} {checkpointName} --leave-running");

        // process.WaitForExit();

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

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"start {containerName} --checkpoint {checkpointName}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        // Process process = Process.Start(
        //     "bash",
        //     $@"docker start {containerName} --checkpoint {checkpointName}");

        // process.WaitForExit();

        Log.Information($"Restored container, id: {id}, checkpoint: {checkpointName}");
    }

    public async Task<bool> ContainerIsRunningAsync(string id)
    {
        // Get a list of all the containers on the host
        var containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters()
        );

        // Check the status of each container
        foreach (var container in containers)
        {
            if (container.Status.StartsWith("Up"))
            {
                return true;
            }
        }

        return false;
    }

    #endregion Methods
}
