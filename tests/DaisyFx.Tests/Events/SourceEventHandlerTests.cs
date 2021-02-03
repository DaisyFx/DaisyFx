using System.Threading.Tasks;
using DaisyFx.Events.Source;
using DaisyFx.Sources;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Tests.Events
{
    public class SourceEventHandlerTests
    {
        [Fact]
        public async Task SourceStart_SourceStartedHandler()
        {
            var eventTracker = new TestEventTracker<SourceStartedEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<SourceStartedEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<SignalTestSource>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();

            await chain.Sources.WaitForCompletionAsync();

            var source = chain.Sources[0];

            Assert.Single(eventTracker.TrackedEvents, e =>
                e.ChainName == chain.Name &&
                e.SourceIndex == source.Index &&
                e.SourceName == source.Name &&
                e.SourceExecutionId != default);
        }

        [Fact]
        public async Task SourceCanceled_SourceStoppedHandler()
        {
            var eventTracker = new TestEventTracker<SourceStoppedEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<SourceStoppedEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<InfiniteDelaySource>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();
            await chain.StopAllSourcesAsync();

            var source = chain.Sources[0];
            Assert.Single(eventTracker.TrackedEvents, e =>
                e.ChainName == chain.Name &&
                e.SourceIndex == source.Index &&
                e.SourceName == source.Name &&
                e.SourceExecutionId != default &&
                e.Result == SourceResult.Canceled);
        }

        [Fact]
        public async Task SourceException_NotifiesSourceStoppedHandler()
        {
            var eventTracker = new TestEventTracker<SourceStoppedEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<SourceStoppedEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<ThrowingTestSource<Signal>>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();

            await chain.Sources.WaitForCompletionAsync();

            var source = chain.Sources[0];
            Assert.Single(eventTracker.TrackedEvents, e =>
                e.ChainName == chain.Name &&
                e.SourceIndex == source.Index &&
                e.SourceName == source.Name &&
                e.Result == SourceResult.Faulted &&
                e.SourceExecutionId != default &&
                e.Exception is TestException);
        }

        [Fact]
        public async Task SourceCompleted_NotifiesSourceStoppedHandler()
        {
            var eventTracker = new TestEventTracker<SourceStoppedEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<SourceStoppedEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<SignalTestSource>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();

            await chain.Sources.WaitForCompletionAsync();

            var source = chain.Sources[0];
            Assert.Single(eventTracker.TrackedEvents, e =>
                e.ChainName == chain.Name &&
                e.SourceIndex == source.Index &&
                e.SourceName == source.Name &&
                e.Result == SourceResult.Completed &&
                e.SourceExecutionId != default);
        }

        [Fact]
        public async Task SourceException_NotifiesSourceExceptionHandler()
        {
            var eventTracker = new TestAsyncEventTracker<SourceExceptionEvent>();

            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestAsyncEventTrackerHandler<SourceExceptionEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources => sources
                    .Add<ThrowingTestSource<Signal>>("test")
            };
            var chain = await chainBuilder.BuildAsync(serviceProvider);

            chain.StartAllSources();

            await chain.Sources.WaitForCompletionAsync();

            var source = chain.Sources[0];
            Assert.Single(eventTracker.TrackedEvents, e =>
                e.ChainName == chain.Name &&
                e.SourceIndex == source.Index &&
                e.SourceName == source.Name &&
                e.SourceExecutionId != default &&
                e.Exception is TestException);
        }
    }
}