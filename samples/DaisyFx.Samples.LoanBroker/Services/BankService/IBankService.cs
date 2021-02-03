using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Samples.LoanBroker.Services.BankService
{
    public interface IBankService
    {
        Task<LoanQuote[]> GetLoanQuotesAsync(int amount, int loanDuration, int creditScore, CancellationToken cancellationToken);
    }
}