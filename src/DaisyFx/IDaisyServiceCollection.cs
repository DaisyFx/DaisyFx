using System;
using DaisyFx.Events;
using DaisyFx.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaisyFx
{
    public interface IDaisyServiceCollection
    {
        public IServiceCollection ServiceCollection { get; }
        public IConfiguration Configuration { get; }

        DaisyServiceCollection AddHostMode<THostInterface>(string alias,
            Action<DaisyServiceCollection, IServiceCollection, IConfiguration> configureServices)
            where THostInterface : class, IHostInterface;

        DaisyServiceCollection AddChain<TChainBuilder>()
            where TChainBuilder : class, IChainBuilder;

        DaisyServiceCollection AddEventHandlerSingleton<TEventHandler>()
            where TEventHandler : class, IDaisyEventHandler;
    }
}