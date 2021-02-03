using System.Threading.Tasks;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class MappingConnectorTests
    {
        [Fact]
        public async Task Process_ReturnsMappedValue()
        {
            const string testInput = "SomeString";
            var result = 0;

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root => root
                    .Map(input => input.Length)
                    .TestInspect(onProcess: (input, _) => result = input)
            };

            await chainBuilder.BuildAndExecuteAsync(testInput);

            Assert.Equal(testInput.Length, result);
        }
    }
}