using System;
using DaisyFx.Connectors;

namespace DaisyFx.Events.Connector
{
    public readonly struct ConnectorStoppedEvent : IDaisyEvent
    {
        public ConnectorStoppedEvent(IReadOnlyChainContext context, IConnector connector, TimeSpan duration)
        {
            Context = context;
            Connector = connector;
            Duration = duration;
        }

        public IReadOnlyChainContext Context { get; }
        public IConnector Connector { get; }
        public TimeSpan Duration { get; }
    }
}