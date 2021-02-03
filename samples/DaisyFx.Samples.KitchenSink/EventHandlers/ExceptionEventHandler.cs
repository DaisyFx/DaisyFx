using System.Threading.Tasks;
using DaisyFx.Events;
using DaisyFx.Events.Chain;
using DaisyFx.Events.Source;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Samples.KitchenSink.EventHandlers
{
    public class ExceptionEventHandler : IDaisyExceptionEventHandler
    {
        private readonly ILogger<ExceptionEventHandler> _logger;

        public ExceptionEventHandler(ILogger<ExceptionEventHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask HandleAsync(ChainExceptionEvent daisyEvent)
        {
            _logger.LogInformation($"Event: {nameof(ChainExceptionEvent)}");
            return new ValueTask();
        }

        public ValueTask HandleAsync(SourceExceptionEvent daisyEvent)
        {
            _logger.LogInformation($"Event: {nameof(SourceExceptionEvent)}");
            return new ValueTask();
        }
    }
}