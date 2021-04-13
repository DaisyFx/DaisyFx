using System.Collections.Generic;
using System.Threading.Tasks;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class SubChainConnector
    {
        [Fact]
        public async Task Process_CallsSubChainWithInput()
        {
            const string payload = "Test";
            var result = new List<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root => root
                    .SubChain(subChain => subChain
                        .TestInspect(onProcess: (input, _) => result.Add(input))
                    )
            };

            await chainBuilder.BuildAndExecuteAsync( payload);

            Assert.Single(result, payload);
        }

        [Fact]
        public async Task Process_ReturnsInput()
        {
            const string payload = "Test";

            var result = new List<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root => root
                    .SubChain(subChain => subChain
                        .Link<NoopLink<string>, string>()
                    )
                    .TestInspect(onProcess: (input, _) => result.Add(input))
            };

            await chainBuilder.BuildAndExecuteAsync( payload);

            Assert.Single(result, payload);
        }

        [Fact]
        public async Task Dispose_DisposesSubChain()
        {
            var disposed = false;
            var chainBuilder = new TestChain<Signal[]>
            {
                ConfigureRootAction = root => root
                    .SubChain(subChain => subChain
                        .TestInspect(onDispose: () => disposed = true)
                    )
            };

            var chain = await chainBuilder.BuildAsync();
            chain.Dispose();

            Assert.True(disposed);
        }

        [Fact]
        public async Task Process_DisposesRegisteredDisposables()
        {
            var chainDisposableCount = 0;
            var fakeDisposable = new FakeDisposable();
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .SubChain(subChain => subChain
                        .TestInspect(onProcess: (_, context) => context.RegisterForDispose(fakeDisposable))
                    )
                    .TestInspect(onProcess: (_, context) => chainDisposableCount = context.OnComplete.Count)
            };

            var chain = await chainBuilder.BuildAsync();
            var result = await chain.ExecuteAsync(Signal.Static, default);

            Assert.Equal(ExecutionResultStatus.Completed, result.Status);
            Assert.Equal(0, chainDisposableCount);
            Assert.Equal(1, fakeDisposable.DisposeCount);
        }
    }
}