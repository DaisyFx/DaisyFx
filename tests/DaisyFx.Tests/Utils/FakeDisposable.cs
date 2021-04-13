using System;

namespace DaisyFx.Tests.Utils
{
    public class FakeDisposable : IDisposable
    {
        public bool Disposed => DisposeCount > 0;
        public int DisposeCount { get; private set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }
}