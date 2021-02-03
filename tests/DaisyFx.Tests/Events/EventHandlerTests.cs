using System;
using System.Threading.Tasks;
using DaisyFx.Events;
using DaisyFx.Events.Source;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Tests.Events
{
    public class EventHandlerTests
    {
        [Fact]
        public async Task PublishEvent_MultipleHandlersWithSingleInterface_NotifiesAllHandlers()
        {
            var eventTracker = new TestEventTracker<SourceStartedEvent>();

            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<SourceStartedEvent>>()
                    .AddEventHandlerSingleton<SourceStartedEventTrackerHandler>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<SignalTestSource>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();
            await chain.Sources.WaitForCompletionAsync();

            Assert.Equal(2, eventTracker.TrackedHandlerIds.Count);
            Assert.Equal(2, eventTracker.TrackedEvents.Count);
        }

        [Fact]
        public async Task AddEventHandler_SingleHandlerWithMultipleInterfaces_NotifiesSingleInstance()
        {
            var sourceStartedTracker = new TestEventTracker<SourceStartedEvent>();
            var sourceStoppedTracker = new TestEventTracker<SourceStoppedEvent>();

            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(sourceStartedTracker)
                    .AddSingleton(sourceStoppedTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<SourceEventHandler>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<SignalTestSource>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();
            await chain.Sources.WaitForCompletionAsync();

            Assert.Single(sourceStartedTracker.TrackedHandlerIds);
            Assert.Single(sourceStoppedTracker.TrackedHandlerIds);
            Assert.Equal(sourceStoppedTracker.TrackedHandlerIds, sourceStoppedTracker.TrackedHandlerIds);
        }
    }

    public class SourceStartedEventTrackerHandler : TestEventTrackerHandler<SourceStartedEvent>
    {
        public SourceStartedEventTrackerHandler(TestEventTracker<SourceStartedEvent> eventTracker) : base(eventTracker)
        {
        }
    }

    public class SourceEventHandler :
        IDaisyEventHandler<SourceStartedEvent>,
        IDaisyEventHandler<SourceStoppedEvent>
    {
        private readonly TestEventTracker<SourceStartedEvent> _sourceStartedTracker;
        private readonly TestEventTracker<SourceStoppedEvent> _sourceStoppedTracker;
        private readonly Guid _id = Guid.NewGuid();

        public SourceEventHandler(
            TestEventTracker<SourceStartedEvent> sourceStartedTracker,
            TestEventTracker<SourceStoppedEvent> sourceStoppedTracker)
        {
            _sourceStartedTracker = sourceStartedTracker;
            _sourceStoppedTracker = sourceStoppedTracker;
        }

        public void Handle(in SourceStartedEvent e) =>  _sourceStartedTracker.TrackEvent(_id, e);
        public void Handle(in SourceStoppedEvent e) => _sourceStoppedTracker.TrackEvent(_id, e);
    }
}