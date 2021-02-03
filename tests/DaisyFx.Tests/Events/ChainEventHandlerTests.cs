using System.Threading.Tasks;
using DaisyFx.Events.Chain;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Tests.Events
{
    public class ChainEventHandlerTests
    {
        [Fact]
        public async Task ChainExecutionStart_NotifiesHandler()
        {
            var eventTracker = new TestEventTracker<ChainExecutionStartedEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<ChainExecutionStartedEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .Link<NoopLink<Signal>, Signal>()
            };

            await chainBuilder.BuildAndExecuteAsync(Signal.Static, default, serviceProvider);

            Assert.Single(eventTracker.TrackedEvents);
        }

        [Fact]
        public async Task ChainExecutionCompletedResult_NotifiesHandler()
        {
            var eventTracker = new TestEventTracker<ChainExecutionResultEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<ChainExecutionResultEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .Link<NoopLink<Signal>, Signal>()
            };

            await chainBuilder.BuildAndExecuteAsync(Signal.Static, default, serviceProvider);

            Assert.Single(eventTracker.TrackedEvents, e => e.Result == ExecutionResult.Completed);
        }

        [Fact]
        public async Task ChainExecutionCompletedResult_WithReason_NotifiesHandler()
        {
            var reason = "TestReason";
            var eventTracker = new TestEventTracker<ChainExecutionResultEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<ChainExecutionResultEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .Complete(reason)
            };

            await chainBuilder.BuildAndExecuteAsync(Signal.Static, default, serviceProvider);

            Assert.Single(eventTracker.TrackedEvents, e => e.Result == ExecutionResult.Completed &&
                                                           e.ResultReason == reason);
        }

        [Fact]
        public async Task ChainExecutionFaultedResult_NotifiesHandler()
        {
            var eventTracker = new TestEventTracker<ChainExecutionResultEvent>();
            var serviceProvider = new TestServiceProvider(
                configureServices: services => services
                    .AddSingleton(eventTracker),
                configureDaisy: daisy => daisy
                    .AddEventHandlerSingleton<TestEventTrackerHandler<ChainExecutionResultEvent>>()
            );

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .Link<ThrowingLink<Signal>, Signal>()
            };

            await chainBuilder.BuildAndExecuteAsync(Signal.Static, default, serviceProvider);

            Assert.Single(eventTracker.TrackedEvents, e => e.Result == ExecutionResult.Faulted);
        }
    }
}