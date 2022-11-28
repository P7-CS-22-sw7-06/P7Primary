using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

public class Misc
{
    public Misc() { }

    #region Variables

    #endregion Variables

    #region Methods

    public void MovePayloadIntoContainer(string payloadName, string containerID)
    {
        string pathToContainers = $@"/var/lib/docker/containers";
        string pathToPayload = $@"/{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}";
        string pathToCheckpoints = $@"/{pathToContainers}/{containerID}/checkpoints";

        // Get permission to access container
        Process p = Process.Start("chmod", $"-R 755 {pathToContainers}");
        p.WaitForExit();

        string sourceFile = System.IO.Path.Combine(payloadName, payloadName);
        string destFile = System.IO.Path.Combine(pathToCheckpoints, payloadName);

        System.IO.File.Copy(sourceFile, destFile, true);
    }

    #endregion Methods
}
