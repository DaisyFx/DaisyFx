using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx
{
    public delegate ValueTask InitDelegate(CancellationToken cancellationToken);

    public delegate Task<ExecutionResult> SourceNextDelegate<in TInput>(TInput input);

    public delegate void ActionDelegate<in TInput>(TInput input, ChainContext context);

    public delegate TOutput MapperDelegate<in TInput, out TOutput>(TInput input);

    public delegate Task<ExecutionResult> ChainExecuteDelegate<in TInput>(TInput input, CancellationToken cancellationToken);

    public delegate ValueTask OnSourceExceptionDelegate(string sourceName, Exception exception, IServiceProvider services);
}