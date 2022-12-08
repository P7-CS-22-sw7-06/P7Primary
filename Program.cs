using System;
using System.IO;
using System.Threading;
using Serilog;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace P7;

class Program
{
    static async Task Main()
    {
        // Change these values to suit your needs
        string container = "magnustest1";
        string image = "busybox";
        string payloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/p7log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        ContainerController cc = new ContainerController();

        try
        {
            Console.WriteLine("Starting Program...");
            Log.Information($"Hello, {Environment.UserName}!");

            while (true)
            {
                string filePath = System.IO.Path.Combine(payloadLocation, "payload.zip");

                if (System.IO.File.Exists(filePath))
                {
                    await cc.CreateContainerAsync(container, image, payloadLocation);
                    string containerID = await cc.GetContainerIDByNameAsync(container);
                    await cc.StartAsync(containerID);
                }

                Thread.Sleep(500);
            }
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
