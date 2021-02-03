using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Links;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.BankService;
using DaisyFx.Samples.LoanBroker.Tests.Fakes;
using DaisyFx.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Samples.LoanBroker.Tests.Links
{
    public class CreateLoanContractTests
    {
        [Fact]
        public async Task Execute_ReturnsLoanContract()
        {
            var lowQuote = new LoanQuote("Bank 1", 1);
            var highQuote = new LoanQuote("Bank 2", 5);
            var testRunner = new StatefulLinkTestRunner<CreateLoanContract, LoanApplication, LoanContract>(
                services => services.AddSingleton<IBankService>(new FakeBankService(new[] {lowQuote, highQuote}))
            );

            var loanApplication = TestDataFactory.CreateLoanApplication();
            var loanContract = await testRunner.ExecuteAsync(loanApplication);

            Assert.Equal(lowQuote.BankName, loanContract.BankName);
            Assert.Equal(lowQuote.InterestRate, loanContract.InterestRate);
            Assert.Equal(loanApplication.Ssn, loanContract.Ssn);
            Assert.Equal(loanApplication.Name, loanContract.Name);
            Assert.Equal(loanApplication.Amount, loanContract.Amount);
            Assert.Equal(loanApplication.LoanDuration, loanContract.LoanDuration);
            Assert.Equal(loanApplication.CreditScore, loanContract.CreditScore);
        }
    }
}