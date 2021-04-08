using System;
using DaisyFx.Sources;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Logging
{
    public static class SourceConnectorLogMessages
    {
        private static readonly Action<ILogger, string, Exception?> SourceCompletedMessage;
        private static readonly Action<ILogger, string, Exception?> SourceCanceledMessage;
        private static readonly Action<ILogger, string, Exception> SourceFatalErrorMessage;

        static SourceConnectorLogMessages()
        {
            SourceCompletedMessage = LoggerMessage.Define<string>(
                LogLevel.Information, EventIds.SourceCompletedMessage,
                "Source {SourceName} completed");

            SourceCanceledMessage = LoggerMessage.Define<string>(
                LogLevel.Warning, EventIds.SourceCanceledMessage,
                "Source {SourceName} canceled");

            SourceFatalErrorMessage = LoggerMessage.Define<string>(
                LogLevel.Critical, EventIds.SourceFatalErrorMessage,
                "Source {SourceName} crashed");
        }

        public static void SourceCompleted(this ILogger<ISourceConnector> logger, string sourceName)
            => SourceCompletedMessage(logger, sourceName, null);

        public static void SourceCanceled(this ILogger<ISourceConnector> logger,
            string sourceName,
            Exception? exception = null)
            => SourceCanceledMessage(logger, sourceName, exception);

        public static void SourceFatalError(this ILogger<ISourceConnector> logger,
            string sourceName,
            Exception exception)
            => SourceFatalErrorMessage(logger, sourceName, exception);
    }
}