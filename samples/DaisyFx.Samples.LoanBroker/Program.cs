using DaisyFx.Samples.LoanBroker.Chains;
using DaisyFx.Samples.LoanBroker.Services.BankService;
using DaisyFx.Samples.LoanBroker.Services.CreditService;
using DaisyFx.Samples.LoanBroker.Services.LoanService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaisyFx.Samples.LoanBroker
{
    class Program
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
            services.AddSingleton<IBankService, BankService>();
            services.AddSingleton<ICreditService, CreditService>();
            services.AddSingleton<ILoanService, LoanService>();

            services.AddDaisy(hostContext.Configuration, d =>
            {
                d.AddChain<LoanBrokerChain>();
            });
        }
    }
}