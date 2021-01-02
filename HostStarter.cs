using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

/// https://github.com/serilog/serilog-extensions-hosting
public static class HostStarter
{

    public static void StartHost(
        string[] args,
        Action<HostBuilderContext, IServiceCollection> configure)
    {
        StartHost(args, strings =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(configure)
            );
    }

    public static void StartHost(
        Func<IHostBuilder> hostCreator)
    {
        StartHost(new string[0], strings => hostCreator());
    }

    public static void StartHost(string[] args,
        Func<string[], IHostBuilder> hostCreator)
    {

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Logger.Information("Creating host");
            var hostBuilder = hostCreator(args);
            var host = hostBuilder.UseSerilog().UseWindowsService().Build();
            Log.Logger.Information("Host created");
            Log.Logger.Information("Starting host");
            host.Run();
            Log.Logger.Information("Host ended");
        }
        catch (OperationCanceledException)
        {
            Log.Logger.Information("Host cancelled");
        }
        catch (Exception x)
        {
            Log.Logger.Error(x, $"Host error:");
            Environment.ExitCode = 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }

}

