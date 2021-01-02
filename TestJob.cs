using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;


public class TestJob : IJob
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public TestJob(
        ILogger logger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task Execute(
        IJobExecutionContext context)
    {

        for (int i = 0; i < 10; i++)
        {
            await Task.Run(() =>
            {
                _logger.LogInformation("I AM SLEEPING");
                Thread.Sleep(1000 * 5);
                _logger.LogInformation("WAKED uP");
            });

            if (context.CancellationToken.IsCancellationRequested)
                return;
        }

        _logger.LogInformation("ENDED");
    }

}
