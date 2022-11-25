using System;
using Serilog;

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

        try
        {
            Console.WriteLine("Hello World!");
            Log.Information("Hello, {Name}!", Environment.UserName);
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
