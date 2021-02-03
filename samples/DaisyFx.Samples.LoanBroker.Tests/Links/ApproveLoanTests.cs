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
    public class ApproveLoanTests
    {
        [Fact]
        public async Task Execute_CallsLoanService()
        {
            var fakeLoanService = new FakeLoanService();
            var testRunner = new StatefulLinkTestRunner<ApproveLoan, LoanContract, Signal>(
                services => services.AddSingleton<ILoanService>(fakeLoanService)
            );

            var loanContract = TestDataFactory.CreateLoanContract();
            await testRunner.ExecuteAsync(loanContract);

            Assert.Contains(loanContract, fakeLoanService.ApprovedLoans);
        }
    }
}