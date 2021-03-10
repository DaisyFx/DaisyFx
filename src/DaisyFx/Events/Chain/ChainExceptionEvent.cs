namespace DaisyFx.Events.Chain
{
    public class ChainExceptionEvent : IDaisyEventAsync
    {
        public ChainExceptionEvent(IReadOnlyChainContext context, ChainException exception)
        {
            Context = context;
            Exception = exception;
        }

        public IReadOnlyChainContext Context { get; }
        public ChainException Exception { get; }
    }
}