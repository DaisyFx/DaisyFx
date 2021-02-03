using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;

namespace DaisyFx.Samples.LoanBroker.Services.LoanService
{
    public interface ILoanService
    {
        Task<LoanInquiry> GetLoanInquiryAsync(int maxAgeDays, CancellationToken cancellationToken);
        Task ApproveLoanAsync(LoanContract loanContract, CancellationToken cancellationToken);
        Task DenyLoanAsync(LoanApplication loanApplication, CancellationToken cancellationToken);
    }
}