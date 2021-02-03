using System;
using System.Threading;
using DaisyFx.Tests.Utils;
using DaisyFx.Tests.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DaisyFx.Tests
{
    public class ChainContextTests
    {
        [Fact]
        public void Set_ValueWithNewKey_AddsValue()
        {
            var context = CreateChainContext();
            const string testKey = "TestKey";
            const string expectedValue = "TestValue";

            context.Set(testKey, expectedValue);

            context.TryGet(testKey, out string? actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Set_ValueWithExistingKey_UpdatesValue()
        {
            var context = CreateChainContext();
            const string testKey = "TestKey";
            const string expectedOriginalValue = "TestValue1";
            const string expectedUpdatedValue = "TestValue2";

            context.Set(testKey, expectedOriginalValue);
            context.Set(testKey, expectedUpdatedValue);

            context.TryGet(testKey, out string? actualValue);
            Assert.Equal(expectedUpdatedValue, actualValue);
        }

        [Fact]
        public void Set_ValuesWithDifferentTypes_AddsValues()
        {
            var context = CreateChainContext();
            const string testKey1 = "TestKey1";
            const string expectedValue1 = "TestValue1";

            const string testKey2 = "TestKey2";
            const int expectedValue2 = 2;

            context.Set(testKey1, expectedValue1);
            context.Set(testKey2, expectedValue2);

            var key1Found = context.TryGet(testKey1, out string? actualValue1);
            var key2Found = context.TryGet(testKey2, out int actualValue2);

            Assert.True(key1Found);
            Assert.True(key2Found);
            Assert.Equal(expectedValue1, actualValue1);
            Assert.Equal(expectedValue2, actualValue2);
        }

        [Fact]
        public void TryGet_ExistingKey_ReturnsTrue()
        {
            var context = CreateChainContext();
            const string testKey = "TestKey";

            context.Set(testKey, "TestValue");

            Assert.True(context.TryGet(testKey, out string? _));
        }

        [Fact]
        public void TryGet_NonExistingKey_ReturnsFalse()
        {
            var context = CreateChainContext();

            Assert.False(context.TryGet("TestKey", out string? _));
        }

        [Fact]
        public void TryGet_InvalidCast_ThrowsInvalidCastException()
        {
            var context = CreateChainContext();
            const string testKey = "TestKey";

            // Set string value
            context.Set(testKey, "TestValue");

            Assert.Throws<InvalidCastException>(() =>
            {
                // Try get value as int
                context.TryGet(testKey, out int _);
            });
        }

        [Fact]
        public void Dispose_DisposesItems()
        {
            var item = new FakeItem();
            using (var context = CreateChainContext())
            {
                context.Set("TestKey", item);
            }

            Assert.True(item.Disposed);
        }

        [Theory]
        [InlineData(ExecutionResult.Completed, LogLevel.Information)]
        [InlineData(ExecutionResult.Faulted, LogLevel.Error)]
        public void SetResult_Logs(ExecutionResult status, LogLevel logLevel)
        {
            const string reason = "TestReason";
            var logSink = new TestLogSink();
            var context = CreateChainContext(logSink: logSink);

            context.SetResult(status, null, reason);

            Assert.Contains(logSink.LogEntries,
                e => e.LogLevel == logLevel &&
                     e.Message.Contains(reason));
        }

        [Fact]
        public void SetResult_ToUnknown_Throws()
        {
            var logSink = new TestLogSink();
            var context = CreateChainContext(logSink: logSink);

            Assert.Throws<ArgumentOutOfRangeException>(() => context.SetResult(ExecutionResult.Unknown));
        }

        [Fact]
        public void SetResult_CalledTwice_Throws()
        {
            var logSink = new TestLogSink();
            var context = CreateChainContext(logSink: logSink);

            context.SetResult(ExecutionResult.Completed);

            Assert.Throws<NotSupportedException>(() => context.SetResult(ExecutionResult.Completed));
        }

        private static ChainContext CreateChainContext(string name = "TestName", TestLogSink? logSink = null)
        {
            var serviceProvider = new TestServiceProvider(
                configureServices: services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.AddProvider(new TestLoggerProvider(logSink ?? new TestLogSink()));
                    });
                }
            );

            return new ChainContext(name, serviceProvider, CancellationToken.None);
        }

        private class FakeItem : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}