using NCrontab;

namespace DaisyFx.NCrontab
{
    public class NCrontabSourceConfiguration
    {
        // https://crontab.guru
        public string CronExpression { get; set; } = string.Empty;
        public bool IncludeSeconds { get; set; }

        public CrontabSchedule GetCurrentCron()
        {
            return CrontabSchedule.Parse(CronExpression,
                new CrontabSchedule.ParseOptions
                {
                    IncludingSeconds = IncludeSeconds
                });
        }
    }
}