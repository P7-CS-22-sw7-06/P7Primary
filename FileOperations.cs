using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

public class FileOperations
{
    public FileOperations(DockerClient client)
    {
        this.client = client;
    }

    #region Variables

    DockerClient client { get; set; }
    string pathToContainers = $@"/var/lib/docker/containers";
    string pathToHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    #endregion Variables

    #region Methods

    public void MovePayloadIntoContainer(string payloadName, string containerID)
    {
        string payload = System.IO.Path.Combine(pathToHome, payloadName);

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"cp {payload} {containerID}:{payloadName}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }

    public void ExtractResultFromContainer(string resultName, string containerID)
    {
        string resultDestination = System.IO.Path.Combine(pathToHome, resultName);

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"cp {containerID}:./{resultName} {resultDestination}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }

    public void MoveCheckpointFromContainer(string checkpoint, string containerID)
    {
        string pathToCheckpoints = $@"/{pathToContainers}/{containerID}/checkpoints";

        // Get permission to access container
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "chmod";
            process.StartInfo.Arguments = $"-R 755 {pathToContainers}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        // Process p = Process.Start("chmod", $"-R 755 {pathToContainers}");
        // p.WaitForExit();

        string sourceFile = System.IO.Path.Combine(pathToCheckpoints, checkpoint);
        string destFile = System.IO.Path.Combine(pathToHome, checkpoint);

        System.IO.File.Copy(sourceFile, destFile, true);
    }

    public void MoveCheckpointIntoContainer(string checkpoint, string containerID)
    {
        string pathToCheckpoints = $@"/{pathToContainers}/{containerID}/checkpoints";

        // Get permission to access container
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "chmod";
            process.StartInfo.Arguments = $"-R 755 {pathToContainers}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        // Process p = Process.Start("chmod", $"-R 755 {pathToContainers}");
        // p.WaitForExit();

        string sourceFile = System.IO.Path.Combine(pathToHome, checkpoint);
        string destFile = System.IO.Path.Combine(pathToCheckpoints, checkpoint);

        System.IO.File.Copy(sourceFile, destFile, true);
    }

    #endregion Methods
}
