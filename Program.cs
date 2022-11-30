using System;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

class Program
{
    static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        DockerClient client = new DockerClientConfiguration(
            new Uri("unix:///var/run/docker.sock"))
            .CreateClient();

        ContainerController cc = new ContainerController(client);

        try
        {
            Console.WriteLine("Starting Program...");
            Log.Information($"Hello, {Environment.UserName}!");
            Log.Information($"Version Info: {client.System.GetVersionAsync()}");
            Log.Information($"System Info: {client.System.GetSystemInfoAsync()}");

            await cc.CreateImageAsync("busybox");
            await cc.CreateContainerAsync("magnustest1", "busybox", $@"/{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/payload.py");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Something went wrong");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
