using System.Linq;
using System.Threading.Tasks;

namespace DaisyFx.Samples.KitchenSink.Links
{
    public class GenerateArray : StatelessLink<string, int[]>
    {
        private readonly GenerateArrayOptions _config;

        public GenerateArray()
        {
            _config = ReadConfiguration<GenerateArrayOptions>();
        }

        protected override ValueTask<int[]> Invoke(string input, ChainContext context)
        {
            return new(Enumerable.Range(_config.Start, _config.Count).ToArray());
        }
    }

    public class GenerateArrayOptions
    {
        public int Start { get; set; } = 1;
        public int Count { get; set; } = 10;
    }
}