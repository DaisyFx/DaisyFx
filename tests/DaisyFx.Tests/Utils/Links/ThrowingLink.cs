using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Links
{
    public class ThrowingLink<T> : StatelessLink<T, T>
    {
        protected override ValueTask<T> Invoke(T input, ChainContext context)
        {
            throw new TestException();
        }
    }
}