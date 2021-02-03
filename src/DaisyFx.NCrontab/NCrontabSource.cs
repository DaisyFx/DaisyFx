using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace DaisyFx.NCrontab
{
    public class NCrontabSource : Source<Signal>
    {
        private readonly ILogger<NCrontabSource> _logger;
        private readonly CrontabSchedule _cron;

        public NCrontabSource(ILogger<NCrontabSource> logger)
        {
            _logger = logger;
            _cron = ReadConfiguration<NCrontabSourceConfiguration>().GetCurrentCron();
        }

        public override async Task ExecuteAsync(SourceNextDelegate<Signal> next, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var nextOccurrence = _cron.GetNextOccurrence(DateTime.UtcNow);
                var timeUntilNextOccurrence = nextOccurrence - DateTime.UtcNow;
                var waitUntilNextOccurrenceTask = Task.Delay(timeUntilNextOccurrence, cancellationToken);

                _logger.Waiting(timeUntilNextOccurrence);
                await waitUntilNextOccurrenceTask;

                await next(Signal.Static);
            }
        }
    }
}