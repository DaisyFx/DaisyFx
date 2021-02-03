using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}