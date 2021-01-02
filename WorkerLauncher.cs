// using Quartz;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using Quartz.Impl;
// using Microsoft.Extensions.Logging;
// using System.Threading;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;

// namespace edok.Common.FolderCleaner.NtService
// {
//     public class WorkersLauncher<TJob> : IHostedService where TJob : IJob
//     {
//         private readonly ILogger _logger;
//         private readonly ServiceProviderJobFactory _serviceProviderJobFactory;
//         private readonly IHostApplicationLifetime _hostApplicationLifetime;

//         private readonly IBuildableJob<TJob> buildableJob;

//         private IScheduler _scheduler;
//         private IJobDetail _cleaningJob;

//         public WorkersLauncher(
//             ServiceProviderJobFactory serviceProviderJobFactory,
//             IHostApplicationLifetime hostApplicationLifetime,
//             ILogger logger, 
//             IBuildableJob<TJob> buildableJob)
//         {
//             _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
//             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//             _serviceProviderJobFactory = serviceProviderJobFactory ?? throw new ArgumentNullException(nameof(serviceProviderJobFactory));
//             this.buildableJob = buildableJob ?? throw new ArgumentNullException(nameof(buildableJob));
//         }


//         public async Task StartAsync(
//             CancellationToken cancellationToken)
//         {

//             try
//             {
//                 _logger.LogInformation($"{GetType().Name} starting...");

//                 ISchedulerFactory schedFact = new StdSchedulerFactory();

//                 var namePrefix = GetType().Name;
//                 var postFix = Guid.NewGuid().ToString().Split("-").First();

//                 _scheduler = await schedFact.GetScheduler(cancellationToken);
//                 await _scheduler.Start(CancellationToken.None);
//                 _scheduler.JobFactory = _serviceProviderJobFactory;

//                 _cleaningJob = JobBuilder.Create<TJob>().Build();
                
//                 var trigger = buildableJob.Create().Build();

//                 await _scheduler.ScheduleJob(_cleaningJob, trigger, cancellationToken);

//                 _logger.LogInformation($"{GetType().Name} started.");

//             }
//             catch (Exception e)
//             {
//                 _logger.LogCritical(e, "Service abnormal start");
//                 throw;
//             }

//         }

//         public async Task StopAsync(
//             CancellationToken cancellationToken)
//         {

//             try
//             {
//                 _logger.LogInformation($"{GetType().Name} stopping...");

//                 await _scheduler.Interrupt(_cleaningJob.Key, cancellationToken);
//                 await _scheduler.Shutdown(true, cancellationToken);

//                 _logger.LogInformation($"{GetType().Name} stopped");
//             }
//             catch (Exception e)
//             {
//                 _logger.LogCritical(e, "Service abnormal exit");
//                 throw;
//             }

//         }

//     }

// }

// public interface IBuildableJob<TJob> where TJob : IJob  {

//     public JobBuilder Create();

// }

