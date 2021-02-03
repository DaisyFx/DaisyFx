using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Logging
{
    internal static class ChainLogMessages
    {
        private static readonly Action<ILogger, Exception?> RequestingLockMessage;
        private static readonly Action<ILogger, Exception?> ExecutingMessage;

        static ChainLogMessages()
        {
            RequestingLockMessage = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(1, nameof(RequestingLockMessage)),
                "Requesting lock");

            ExecutingMessage = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(2, nameof(ExecutingMessage)),
                "Executing");
        }

        public static void RequestingLock(this ILogger<IChain> logger)
            => RequestingLockMessage(logger, null);

        public static void Executing(this ILogger<IChain> logger)
            => ExecutingMessage(logger, null);
    }
}