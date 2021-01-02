using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;
using System.Threading;

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
            q.UseInMemoryStore(a=>{

            }); 
            var testJobOption = context.Configuration.GetSection(TestJobOption.TestJob).Get<TestJobOption>();
            q.UseMicrosoftDependencyInjectionScopedJobFactory();
            q.ScheduleJob<TestJob>(trigger => trigger.StartNow()
                .WithCronSchedule(testJobOption.Cron)
                .WithSimpleSchedule(s=> s.WithIntervalInHours(1))
            );

            q.AddSchedulerListener<ServiceCollectionSchedulerListener>();

        });

        services.AddTransient<TestJob>();
        services.AddTransient<ServiceCollectionSchedulerListener>();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    }
}

public class ServiceCollectionSchedulerListener : ISchedulerListener
{

    ISchedulerFactory schedulerFactory;

    public ServiceCollectionSchedulerListener(ISchedulerFactory schedulerFactory)
    {
        this.schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
    }

    public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public  Task SchedulerShutdown(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
    {

        var schedulers = await schedulerFactory.GetAllSchedulers(cancellationToken);
      
        foreach(var scheduler in schedulers) {

            var jobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);

            foreach (var job in jobs) {

                await scheduler.Interrupt(job.JobDetail.Key, cancellationToken);

            }

        }

        return;

        
    }

    public Task SchedulerStarted(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SchedulerStarting(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

