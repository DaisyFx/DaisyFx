using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Services.BankService;

namespace DaisyFx.Samples.LoanBroker.Tests.Fakes
{
    public class FakeBankService : IBankService
    {
        private readonly LoanQuote[] _loanQuotes;

        public FakeBankService(LoanQuote[] loanQuotes)
        {
            _loanQuotes = loanQuotes;
        }

        public Task<LoanQuote[]> GetLoanQuotesAsync(int amount, int loanDuration, int creditScore,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_loanQuotes);
        }
    }
}