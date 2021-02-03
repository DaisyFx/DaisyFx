using DaisyFx.Events.Chain;
using DaisyFx.Events.Source;

namespace DaisyFx.Events
{
    public interface IDaisyExceptionEventHandler :
        IDaisyEventHandlerAsync<ChainExceptionEvent>,
        IDaisyEventHandlerAsync<SourceExceptionEvent>
    {
    }
}