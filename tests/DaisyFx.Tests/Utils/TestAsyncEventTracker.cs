using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyFx.Events;

namespace DaisyFx.Tests.Utils
{
    public class TestAsyncEventTracker<T> where T : IDaisyEventAsync
    {
        private readonly ConcurrentDictionary<Guid, T> _trackedEventsByHandlerId = new();
        public IReadOnlyCollection<T> TrackedEvents => _trackedEventsByHandlerId.Values.ToList();
        public IReadOnlyCollection<Guid> TrackedHandlerIds => _trackedEventsByHandlerId.Keys.ToList();

        public void TrackEvent(Guid handlerId, T daisyEvent)
        {
            _trackedEventsByHandlerId.TryAdd(handlerId, daisyEvent);
        }
    }

    public class TestAsyncEventTrackerHandler<T> : IDaisyEventHandlerAsync<T> where T : class, IDaisyEventAsync
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly TestAsyncEventTracker<T> _eventTracker;

        public TestAsyncEventTrackerHandler(TestAsyncEventTracker<T> eventTracker)
        {
            _eventTracker = eventTracker;
        }

        public ValueTask HandleAsync(T daisyEvent)
        {
            _eventTracker.TrackEvent(_id, daisyEvent);
            return new();
        }
    }
}