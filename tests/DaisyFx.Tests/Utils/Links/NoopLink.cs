using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Links
{
    public class NoopLink<T> : StatelessLink<T, T>
    {
        protected override ValueTask<T> Invoke(T input, ChainContext context)
        {
            return new(input);
        }
    }
}