using System;
using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils
{
    public class FakeAsyncDisposable : IAsyncDisposable
    {
        public bool Disposed { get; private set; }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return new();
        }
    }
}