using System;
using System.Threading;
using System.Diagnostics;

namespace P7;

public class Container
{
    public Container(string name, string payload, string image = "busybox", string checkpoint = "")
    {
        Name = name;
        Payload = payload;
        Image = image;
        CheckpointName = checkpoint;
    }

    #region Variables

    string Name { get; }
    string Image { get; }
    string Payload { get; }
    string CheckpointName { get; set; }
    string ContainerID = "";
    string Path = "/";

    #endregion Variables

    #region Methods


    public void Create()
    {
        Path = $@"/var/lib/docker/containers/{ContainerID}";
        string strCmdText = $@"docker create -d --name {Name} {Image}
            {ContainerID} = sudo docker ps -aqf {Name}
            docker cp {Payload} {Name}:{Payload}";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();
    }


    public void Start()
    {
        string strCmdText = $"docker start {ContainerID}";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();
    }


    public void Stop()
    {
        string strCmdText = $"docker stop {ContainerID}";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();
    }


    public void Execute()
    {
        string strCmdText = $"docker run -d --name {Name} /bin/sh -c ./payload";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();

        while (ContainerIsRunning(Name))
        {
            Thread.Sleep(500);

            Checkpoint(Name);
        }
    }

    public void Checkpoint(string Name)
    {
        string strCmdText = $@"docker checkpoint create {Name} checkpoint";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();
    }

    public void Restore()
    {
        string CheckpointDir = $@"/var/lib/docker/containers/{ContainerID}/checkpoints/{Checkpoint}";

        string strCmdText = $@"docker create --name {Name} --security-opt seccomp:unconfined {Image}
            docker start --checkpoint {CheckpointDir} {Name}";

        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();
    }

    public bool ContainerIsRunning(string Name)
    {
        string strCmdText = $"docker ps | grep {Name}";
        Process P = Process.Start("bash", strCmdText);
        P.WaitForExit();

        if (P.HasExited && P.StandardOutput.ToString() == "")
        {
            return false;
        }
        return true;
    }

    #endregion Methods
}
