using DaisyFx.Connectors;

namespace DaisyFx.Events.Connector
{
    public readonly struct ConnectorStartingEvent : IDaisyEvent
    {
        public ConnectorStartingEvent(IReadOnlyChainContext context, IConnector connector)
        {
            Context = context;
            Connector = connector;
        }

        public IReadOnlyChainContext Context { get; }
        public IConnector Connector { get; }
    }
}