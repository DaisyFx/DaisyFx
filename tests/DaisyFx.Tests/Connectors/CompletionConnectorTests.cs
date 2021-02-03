using System.Threading.Tasks;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class CompletionConnectorTests
    {
        [Fact]
        public async Task Process_SetsResult()
        {
            const string reason = "TestReason";

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .SubChain(subChain => subChain
                        .Complete(reason)
                    )
                    .Link<ThrowingLink<Signal>, Signal>()
            };

            var result = await chainBuilder.BuildAndExecuteAsync(Signal.Static);
            Assert.Equal(ExecutionResult.Completed, result);
        }
    }
}