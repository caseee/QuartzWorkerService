// using System;
// using Microsoft.Extensions.DependencyInjection;
// using Quartz;
// using Quartz.Spi;

// public class ServiceProviderJobFactory : IJobFactory
// {
//     private readonly IServiceProvider _rootServiceProvider;

//     public ServiceProviderJobFactory(IServiceProvider rootServiceProvider)
//     {
//         _rootServiceProvider = rootServiceProvider ?? throw new ArgumentNullException(nameof(rootServiceProvider));
//     }

//     public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
//     {
//         var jobType = bundle.JobDetail.JobType;
//         return (IJob)_rootServiceProvider.GetRequiredService(jobType);
//     }

//     public void ReturnJob(IJob job)
//     {
// 		(job as IDisposable)?.Dispose();
//     }
// }