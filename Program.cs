using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Events;

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
            .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + $"\\logs\\Common.FolderCleaner_{DateTime.Today:yyyyMMdd}.log", LogEventLevel.Information)
            .CreateLogger();

        services.AddScoped<Microsoft.Extensions.Logging.ILogger>(provider => provider.GetService<ILogger<TestJob>>());

        services.AddQuartz(q =>
        {

                // var xxx = context.Configuration.GetSection("TestJob");

                // var bs = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
                // bs.AddConfiguration(context.Configuration);

                // var ssss = bs.Build();
                // var xxxxx= ssss.GetValue<TestJobOption>("TestJob");
                var r = context.Configuration.GetValue<string>("TestJobOption:Cron");


                // var b = services.BuildServiceProvider();
                // var r = b.GetService<IOptions<TestJobOption>>();

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                // q.AddJobAndTrigger<TestJob>(configuration);
                q.ScheduleJob<TestJob>(trigger => trigger
               .StartNow()
               .WithCronSchedule(r)
               );

        });

        services.AddTransient<TestJob>();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    }
}
