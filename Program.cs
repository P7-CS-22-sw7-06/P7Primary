using System;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

class Program
{
    static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        DockerClient client = new DockerClientConfiguration(
            new Uri("unix:///var/run/docker.sock"))
            .CreateClient();

        ContainerController containercontroller = new ContainerController(client);

        try
        {
            Console.WriteLine("Starting Program...");
            Log.Information($"Hello, {Environment.UserName}!");
            Log.Information($"Version Info: {client.System.GetVersionAsync()}");
            Log.Information($"System Info: {client.System.GetSystemInfoAsync()}");
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
