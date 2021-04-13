using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Samples.KitchenSink.Links
{
    public class GenerateArray : StatelessLink<string, int[]>
    {
        private readonly ILogger<GenerateArray> _logger;
        private readonly GenerateArrayOptions _config;

        public GenerateArray(ILogger<GenerateArray> logger)
        {
            _logger = logger;
            _config = ReadConfiguration<GenerateArrayOptions>();
        }

        protected override ValueTask<int[]> ExecuteAsync(string input, ChainContext context)
        {
            _logger.LogInformation("Producing value");
            context.RegisterForDispose(new DisposableAction(LogScopeCompleted));
            return new(Enumerable.Range(_config.Start, _config.Count).ToArray());
        }

        private void LogScopeCompleted()
        {
            _logger.LogInformation("Scope completed");
        }

        protected override void Dispose()
        {
            _logger.LogInformation("Disposing");
            base.Dispose();
        }
    }

    public class GenerateArrayOptions
    {
        public int Start { get; set; } = 1;
        public int Count { get; set; } = 10;
    }
}