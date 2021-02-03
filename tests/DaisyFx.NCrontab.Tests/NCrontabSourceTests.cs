using System;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.NCrontab.Tests
{
    public class NCrontabSourceTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public NCrontabSourceTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task Execute_TriggersNext()
        {
            var sourceRunner = new SourceTestRunner<NCrontabSource, Signal>(_serviceProvider,
                (nameof(NCrontabSourceConfiguration.CronExpression), "* * * * * *"),
                (nameof(NCrontabSourceConfiguration.IncludeSeconds), "true"));

            var tcs = new TaskCompletionSource<bool>();
            using var cts = new CancellationTokenSource();

            Task<ExecutionResult> NextDelegate(Signal input)
            {
                tcs.SetResult(true);
                return Task.FromResult(ExecutionResult.Completed);
            }

            _ = sourceRunner.Execute(NextDelegate, cts.Token);

            var resultTask = await Task.WhenAny(tcs.Task, Task.Delay(1500));
            cts.Cancel();

            Assert.Equal(tcs.Task, resultTask);
        }

        [Fact]
        public async Task Execute_CancellationDuringWait_CancelsExecution()
        {
            var future = DateTime.Now.AddHours(1);
            var sourceRunner = new SourceTestRunner<NCrontabSource, Signal>(_serviceProvider,
                (nameof(NCrontabSourceConfiguration.CronExpression), $"{future.Minute} {future.Hour} * * *"),
                (nameof(NCrontabSourceConfiguration.IncludeSeconds), "false"));

            using var cts = new CancellationTokenSource();

            var executeTask = sourceRunner.Execute(_ => throw new NotSupportedException(), cts.Token);
            cts.Cancel();

            var resultTask = await Task.WhenAny(executeTask, Task.Delay(100));

            Assert.Equal(executeTask, resultTask);
            Assert.Equal(TaskStatus.Canceled, executeTask.Status);
        }

        [Fact]
        public async Task Execute_CancellationDuringNextExecution_GracefullyCancelsExecution()
        {
            var sourceRunner = new SourceTestRunner<NCrontabSource, Signal>(_serviceProvider,
                (nameof(NCrontabSourceConfiguration.CronExpression), "* * * * * *"),
                (nameof(NCrontabSourceConfiguration.IncludeSeconds), "true"));

            var cts = new CancellationTokenSource();

            var executeTask = sourceRunner.Execute(_ =>
            {
                cts.Cancel();
                return Task.FromResult(ExecutionResult.Completed);
            }, cts.Token);


            var resultTask = await Task.WhenAny(executeTask, Task.Delay(1500));

            Assert.Equal(executeTask, resultTask);
            Assert.Equal(TaskStatus.RanToCompletion, executeTask.Status);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}