using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Locking;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using DaisyFx.Tests.Utils.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Tests
{
    public class ChainTests
    {
        [Fact]
        public async Task ConfigureRoot_MultipleTimes_Throws()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root =>
                {
                    root.Link<NoopLink<Signal>, Signal>();
                    root.Link<NoopLink<Signal>, Signal>();
                }
            };

            await Assert.ThrowsAsync<NotSupportedException>(async () => await chainBuilder.BuildAsync());
        }

        [Fact]
        public async Task Get_Name_ReturnsName()
        {
            var chainBuilder = new TestChain<Signal>("TestChainName")
            {
                ConfigureRootAction = root =>
                {
                    root.Link<NoopLink<Signal>, Signal>();
                }
            };

            var chain = await chainBuilder.BuildAsync();

            Assert.Equal(chainBuilder.Name, chain.Name);
        }

        [Fact]
        public async Task Configure_Sources_UsesSources()
        {
            string[] payloads = {"test1", "test2"};
            var result = new ConcurrentBag<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureSourcesAction = sources =>
                {
                    sources.Add<SignalTestSource, Signal>("1", _ => payloads[0]);
                    sources.Add<SignalTestSource, Signal>("2", _ => payloads[1]);
                },
                ConfigureRootAction = root =>
                {
                    root.TestInspect(onProcess: (input, _) => result.Add(input));
                }
            };

            var chain = await chainBuilder.BuildAsync();

            chain.StartAllSources();
            await chain.Sources.WaitForCompletionAsync();

            foreach (var payload in payloads)
            {
                Assert.Contains(payload, result);
            }
        }

        [Fact]
        public async void ExecuteAsync_Completes()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root =>
                    root.Link<NoopLink<Signal>, Signal>()
            };

            var chain = await chainBuilder.BuildAsync();
            await chain.ExecuteAsync(Signal.Static, CancellationToken.None);
        }

        [Fact]
        public async Task Execute_WithCanceledCancellationToken_Throws()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root =>
                    root.Link<NoopLink<Signal>, Signal>()
            };

            var chain = await chainBuilder.BuildAsync();
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await chain.ExecuteAsync(Signal.Static, new CancellationToken(true));
            });
        }

        [Fact(Timeout = 5000)]
        public async Task Set_LockStrategy_UsesLockStrategy()
        {
            var result = new ConcurrentBag<Signal>();
            var lockStrategy = new SharedLockStrategy(1);

            var chainBuilder = new TestChain<Signal>
            {
                LockStrategy = lockStrategy,
                ConfigureRootAction = root =>
                {
                    root.TestInspect(onProcess: (input, _) => result.Add(input));
                }
            };
            var chain = await chainBuilder.BuildAsync();

            await lockStrategy.RequestLockAsync(CancellationToken.None);

            const int releaseTimeout = 500;
            var releaseTask = Task.Run(async () =>
            {
                await Task.Delay(releaseTimeout);
                lockStrategy.ReleaseLock();
            });

            var assertEmptyTask = Task.Run(async () =>
            {
                await Task.Delay(releaseTimeout / 2);
                Assert.Empty(result);
            });

            var assertResultTask = Task.Run(async () =>
            {
                await Task.Delay(releaseTimeout * 2);
                Assert.Single(result);
            });

            await Task.WhenAll(
                chain.ExecuteAsync(Signal.Static, CancellationToken.None),
                releaseTask,
                assertEmptyTask,
                assertResultTask
            );
        }

        [Fact(Timeout = 5000)]
        public async Task Cancel_CancellationToken_FailsChain()
        {
            var result = new ConcurrentBag<Signal>();

            var chainBuilder = new TestChain<Signal>()
            {
                ConfigureRootAction = root =>
                {
                    root
                        .TestInspect(onProcess: (input, _) => result.Add(input))
                        .TestDelay(500)
                        .TestInspect(onProcess: (input, _) => result.Add(input));
                }
            };
            var chain = await chainBuilder.BuildAsync();

            var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(250);

            var executionResult = await chain.ExecuteAsync(Signal.Static, cancellationSource.Token);

            Assert.Equal(ExecutionResult.Faulted, executionResult);
            Assert.Single(result);
        }

        [Fact(Timeout = 5000)]
        public async Task Stop_WhenRunning_StopsExecution()
        {
            var result = new ConcurrentBag<Signal>();

            var chainBuilder = new TestChain<Signal>()
            {
                ConfigureSourcesAction = sources =>
                {
                    sources.Add<SignalTestSource>("signal");
                },
                ConfigureRootAction = root =>
                {
                    root
                        .TestInspect(onProcess: (input, _) => result.Add(input))
                        .TestDelay(500)
                        .TestInspect(onProcess: (input, _) => result.Add(input));
                }
            };

            var chain = await chainBuilder.BuildAsync();

            chain.StartAllSources();
            await Task.Delay(250);
            await chain.StopAllSourcesAsync(true);

            Assert.Single(result);
        }

        [Fact]
        public async Task SourceCompletion_DisposesSource()
        {
            var sourceName = "test";
            var tracker = new TestExecutionTracker();
            var serviceProvider = new TestServiceProvider(
                configureServices: services =>
                {
                    services.AddSingleton(tracker);
                });

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources =>
                {
                    sources.Add<SignalTestSource>(sourceName);
                }
            };

            var chain = await chainBuilder.BuildAsync(serviceProvider);

            // Source is disposed once in constructor of SourceConnector<>
            Assert.True(tracker.DisposeCountBySourceName.ContainsKey(sourceName));
            Assert.Equal(1, tracker.DisposeCountBySourceName[sourceName]);

            chain.StartAllSources();
            await chain.Sources.WaitForCompletionAsync();

            Assert.Equal(2, tracker.DisposeCountBySourceName[sourceName]);
        }

        [Fact]
        public async Task GetSources_ReturnsSourceConnectorsWithIndexes()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureSourcesAction = sources =>
                {
                    sources.Add<SignalTestSource>("1");
                    sources.Add<SignalTestSource>("2");
                    sources.Add<SignalTestSource>("3");
                }
            };
            var chain = await chainBuilder.BuildAsync();

            var indexes = chain.Sources.Select(s => s.Index);

            Assert.Equal(new[] {0, 1, 2}, indexes);
        }

        [Fact]
        public async Task GetConnectors_ReturnsConnectorsWithIndexes()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .Link<NoopLink<Signal>, Signal>()
                    .Link<NoopLink<Signal>, Signal>()
                    .Link<NoopLink<Signal>, Signal>()
            };

            var chain = await chainBuilder.BuildAsync();

            var indexes = chain.Connectors.Select(c => c.Index);

            Assert.Equal(new[] {0, 1, 2}, indexes);
        }
    }
}