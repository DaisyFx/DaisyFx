using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Events
{
    internal class EventBroker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventBroker> _logger;

        public EventBroker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<EventBroker>>();
        }

        public void Publish<TEvent>(in TEvent daisyEvent) where TEvent : struct, IDaisyEvent
        {
            var handlers =
                _serviceProvider.GetRequiredService<EventHandlerCollection<IDaisyEventHandler<TEvent>>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                try
                {
                    handler.Handle(daisyEvent);
                }
                catch (Exception e)
                {
                    LogObserverException(e);
                }
            }
        }

        public async ValueTask PublishAsync<TEvent>(TEvent daisyEvent) where TEvent : class, IDaisyEventAsync
        {
            var handlers =
                _serviceProvider.GetRequiredService<EventHandlerCollection<IDaisyEventHandlerAsync<TEvent>>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                try
                {
                    await handler.HandleAsync(daisyEvent);
                }
                catch (Exception e)
                {
                    LogObserverException(e);
                }
            }
        }

        private void LogObserverException(Exception observerException)
        {
            _logger.LogError(observerException, "Unexpected exception");
        }
    }
}