namespace DaisyFx.Events.Chain
{
    public readonly struct ChainExecutionStartedEvent : IDaisyEvent
    {
        public ChainExecutionStartedEvent(IReadOnlyChainContext context)
        {
            Context = context;
        }

        public IReadOnlyChainContext Context { get; }
    }
}