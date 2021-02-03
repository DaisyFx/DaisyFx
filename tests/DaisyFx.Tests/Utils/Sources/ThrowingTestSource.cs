using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Sources
{
    public class ThrowingTestSource<T> : Source<T> where T : class
    {
        public override Task ExecuteAsync(SourceNextDelegate<T> next, CancellationToken cancellationToken)
        {
            throw new TestException();
        }
    }
}