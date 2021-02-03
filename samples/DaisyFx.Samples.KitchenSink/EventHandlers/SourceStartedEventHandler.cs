using DaisyFx.Events;
using DaisyFx.Events.Source;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Samples.KitchenSink.EventHandlers
{
    public class SourceStartedEventHandler : IDaisyEventHandler<SourceStartedEvent>
    {
        private readonly ILogger<SourceStartedEventHandler> _logger;

        public SourceStartedEventHandler(ILogger<SourceStartedEventHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(in SourceStartedEvent daisyEvent)
        {
            _logger.LogInformation($"Event: {nameof(SourceStartedEvent)}");
        }
    }
}