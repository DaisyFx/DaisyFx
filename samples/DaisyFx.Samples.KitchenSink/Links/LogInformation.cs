using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Samples.KitchenSink.Links
{
    public class LogInformation<T> : StatefulLink<T, T> where T : notnull
    {
        protected override ValueTask<T> ExecuteAsync(T input, ChainContext context)
        {
            context.Logger.LogInformation(input.ToString());
            return new ValueTask<T>(input);
        }
    }
}