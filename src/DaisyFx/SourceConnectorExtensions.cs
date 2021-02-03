using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyFx.Sources;

namespace DaisyFx
{
    public static class SourceConnectorExtensions
    {
        public static Task WaitForCompletionAsync(this IEnumerable<ISourceConnector> connectors)
        {
            return Task.WhenAll(connectors.Select(c => c.WaitForCompletionAsync()));
        }
    }
}