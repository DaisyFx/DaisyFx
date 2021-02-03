using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.NCrontab
{
    public static class NCronTabSourceLogMessages
    {
        private static readonly Action<ILogger, TimeSpan, Exception?> WaitingMessage;

        static NCronTabSourceLogMessages()
        {
            WaitingMessage = LoggerMessage.Define<TimeSpan>(
                LogLevel.Information,
                new EventId(1, nameof(WaitingMessage)),
                "Waiting: {TimeSpan}");
        }

        public static void Waiting(this ILogger<NCrontabSource> logger, TimeSpan timeSpan)
            => WaitingMessage(logger, timeSpan, null);
    }
}