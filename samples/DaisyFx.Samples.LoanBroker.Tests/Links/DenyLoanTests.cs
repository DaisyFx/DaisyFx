using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Links;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.LoanService;
using DaisyFx.Samples.LoanBroker.Tests.Fakes;
using DaisyFx.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Samples.LoanBroker.Tests.Links
{
    public class DenyLoanTests
    {
        [Fact]
        public async Task Execute_CallsLoanService()
        {
            var fakeLoanService = new FakeLoanService();
            var testRunner = new StatefulLinkTestRunner<DenyLoan, LoanApplication, Signal>(
                services => services.AddSingleton<ILoanService>(fakeLoanService)
            );

            var loanApplication = TestDataFactory.CreateLoanApplication();
            await testRunner.ExecuteAsync(loanApplication);

            Assert.Contains(loanApplication, fakeLoanService.DeniedLoans);
        }
    }
}