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

        ContainerController cc = new ContainerController();

        try
        {
            Console.WriteLine("Starting Program...");
            Log.Information($"Hello, {Environment.UserName}!");

            // await cc.CreateContainerAsync("magnustest1", "busybox", $@"/home/magn/payload.sh");
            await cc.ListAvailableContainersAsync();
            // await cc.StartAsync("e1b358378b38");
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
