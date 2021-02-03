using DaisyFx.Samples.KitchenSink.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaisyFx.Samples.KitchenSink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddDaisy(hostContext.Configuration, d =>
            {
                d.AddChain<KitchenSinkChain>();
                d.AddEventHandlerSingleton<ExceptionEventHandler>();
                d.AddEventHandlerSingleton<SourceStartedEventHandler>();
            });
        }
    }
}