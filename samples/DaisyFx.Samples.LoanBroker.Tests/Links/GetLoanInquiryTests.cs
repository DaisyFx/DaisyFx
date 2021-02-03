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
    public class GetLoanInquiryTests
    {
        [Fact]
        public async Task Execute_ReturnsLoanInquiry()
        {
            var expectedLoanInquiry = TestDataFactory.CreateLoanInquiry();
            var fakeLoanService = new FakeLoanService(_ => expectedLoanInquiry);
            var testRunner = new StatefulLinkTestRunner<GetLoanInquiry, Signal, LoanInquiry>(
                services => services.AddSingleton<ILoanService>(fakeLoanService)
            );

            var loanInquiry = await testRunner.ExecuteAsync(Signal.Static);

            Assert.Equal(expectedLoanInquiry, loanInquiry);
        }

        [Fact]
        public async Task Execute_UsesConfiguredMaxAgeDays()
        {
            const int expectedMaxAgeDays = 10;
            var actualMaxAgeDays = 0;

            var fakeLoanService = new FakeLoanService(requestedMaxAgeDays =>
            {
                actualMaxAgeDays = requestedMaxAgeDays;
                return FakeLoanService.DefaultLoanInquiryProvider(requestedMaxAgeDays);
            });
            var testRunner = new StatefulLinkTestRunner<GetLoanInquiry, Signal, LoanInquiry>(
                services => services.AddSingleton<ILoanService>(fakeLoanService),
                configuration: new[]
                {
                    (nameof(GetLoanInquiryConfiguration.MaxAgeDays), expectedMaxAgeDays.ToString())
                }
            );

            await testRunner.ExecuteAsync(Signal.Static);

            Assert.Equal(expectedMaxAgeDays, actualMaxAgeDays);
        }
    }
}