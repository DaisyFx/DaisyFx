using System.Threading.Tasks;

namespace DaisyFx.Events
{
    public interface IDaisyEventHandler
    {
    }

    public interface IDaisyEventHandler<TEvent> : IDaisyEventHandler where TEvent : struct, IDaisyEvent
    {
        void Handle(in TEvent daisyEvent);
    }

    public interface IDaisyEventHandlerAsync<in TEvent> : IDaisyEventHandler where TEvent : class, IDaisyEventAsync
    {
        ValueTask HandleAsync(TEvent daisyEvent);
    }
}