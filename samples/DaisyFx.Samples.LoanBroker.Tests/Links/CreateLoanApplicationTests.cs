using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Links;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.CreditService;
using DaisyFx.Samples.LoanBroker.Tests.Fakes;
using DaisyFx.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DaisyFx.Samples.LoanBroker.Tests.Links
{
    public class CreateLoanApplicationTests
    {
        [Fact]
        public async Task Execute_ReturnsLoanApplication()
        {
            const int creditScore = 500;
            var testRunner = new StatefulLinkTestRunner<CreateLoanApplication, LoanInquiry, LoanApplication>(
                services => services.AddSingleton<ICreditService>(new FakeCreditService(creditScore))
            );

            var loanInquiry = TestDataFactory.CreateLoanInquiry();
            var loanApplication = await testRunner.ExecuteAsync(loanInquiry);

            Assert.Equal(creditScore, loanApplication.CreditScore);
            Assert.Equal(loanInquiry.Ssn, loanApplication.Ssn);
            Assert.Equal(loanInquiry.Name, loanApplication.Name);
            Assert.Equal(loanInquiry.Amount, loanApplication.Amount);
            Assert.Equal(loanInquiry.LoanDuration, loanApplication.LoanDuration);
        }
    }
}