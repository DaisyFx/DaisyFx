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
                _logger.Waiting(nextOccurrence - DateTime.UtcNow);

                // Task.Delay is unreliable, ensure we wait until next occurrence
                do
                {
                    var delay = nextOccurrence - DateTime.UtcNow;
                    var millisecondsDelay = (int)Math.Ceiling(delay.TotalMilliseconds);
                    if (millisecondsDelay > 0)
                    {
                        await Task.Delay(millisecondsDelay, cancellationToken);
                    }
                } while (DateTime.UtcNow < nextOccurrence);

                await next(Signal.Static);
            }
        }
    }
}