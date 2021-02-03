using System;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal class MappingConnector<TInput, TOutput> : Connector<TInput, TOutput>
    {
        private readonly MapperDelegate<TInput, TOutput> _function;

        public MappingConnector(MapperDelegate<TInput, TOutput> function, ConnectorContext context) : base(GetFunctionName(function), context)
        {
            _function = function;
        }

        protected override ValueTask<TOutput> ProcessAsync(TInput input, ChainContext context)
        {
            var result = _function(input);
            return new ValueTask<TOutput>(result);
        }

        private static string GetFunctionName(Delegate @delegate)
        {
            var method = @delegate.Method;
            var declaringTypeName = method.DeclaringType?.Name ?? "Unknown";
            return $"{declaringTypeName}.{method.Name}";
        }
    }
}