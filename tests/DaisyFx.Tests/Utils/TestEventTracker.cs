using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DaisyFx.Events;

namespace DaisyFx.Tests.Utils
{
    public class TestEventTracker<T> where T : struct, IDaisyEvent
    {
        private readonly ConcurrentDictionary<Guid, T> _trackedEventsByHandlerId = new();
        public IReadOnlyCollection<T> TrackedEvents => _trackedEventsByHandlerId.Values.ToList();
        public IReadOnlyCollection<Guid> TrackedHandlerIds => _trackedEventsByHandlerId.Keys.ToList();

        public void TrackEvent(Guid handlerId, T daisyEvent)
        {
            _trackedEventsByHandlerId.TryAdd(handlerId, daisyEvent);
        }
    }

    public class TestEventTrackerHandler<T> : IDaisyEventHandler<T> where T : struct, IDaisyEvent
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly TestEventTracker<T> _eventTracker;

        public TestEventTrackerHandler(TestEventTracker<T> eventTracker)
        {
            _eventTracker = eventTracker;
        }

        public void Handle(in T daisyEvent)
        {
            _eventTracker.TrackEvent(_id, daisyEvent);
        }
    }
}