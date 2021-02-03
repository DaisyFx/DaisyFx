using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Services.CreditService;

namespace DaisyFx.Samples.LoanBroker.Tests.Fakes
{
    public class FakeCreditService : ICreditService
    {
        private readonly int _creditScore;

        public FakeCreditService(int creditScore)
        {
            _creditScore = creditScore;
        }

        public Task<CreditReport> GetCreditReportAsync(string ssn, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CreditReport(_creditScore));
        }
    }
}