using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Options;

public class Program
{
    public static void Main(string[] args)
    {
        HostStarter.StartHost(args, ConfigureService);
    }

    public static void ConfigureService(
        HostBuilderContext context,
        IServiceCollection services)
    {

        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(5));
        services.Configure<TestJobOption>(context.Configuration.GetSection("TestJob"));

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + $"\\logs\\{TestJobOption.TestJob}_{DateTime.Today:yyyyMMdd}.log", LogEventLevel.Information)
            .CreateLogger();

        services.AddScoped<Microsoft.Extensions.Logging.ILogger>(provider => provider.GetService<ILogger<TestJob>>());

        services.AddQuartz(q =>
        {            
            q.UseMicrosoftDependencyInjectionScopedJobFactory();
            q.AddSchedulerListener<ServiceCollectionSchedulerListener>();

            services.Configure<TestJobOption>(context.Configuration.GetSection(TestJobOption.TestJob));
            services.AddOptions<QuartzOptions>().Configure<IOptions<TestJobOption>>((options, dep) =>
            {
                if (!string.IsNullOrWhiteSpace(dep.Value.Cron))
                {
                    q.ScheduleJob<TestJob>(trigger => trigger.StartNow().WithCronSchedule(dep.Value.Cron)                    );
                } // ELSE LOG?
            });

        });

        services.AddTransient<TestJob>();
        services.AddTransient<ServiceCollectionSchedulerListener>();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    }
}

