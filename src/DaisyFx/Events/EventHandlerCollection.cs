using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DaisyFx.Events
{
    internal class EventHandlerCollection<TEventHandler> : ReadOnlyCollection<TEventHandler>
        where TEventHandler : IDaisyEventHandler
    {
        public EventHandlerCollection(IEnumerable<TEventHandler> eventHandlers) : base(eventHandlers.ToArray())
        {
        }
    }
}