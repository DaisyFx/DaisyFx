using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.LoanService;

namespace DaisyFx.Samples.LoanBroker.Links
{
    public class DenyLoan : StatefulLink<LoanApplication, Signal>
    {
        private readonly ILoanService _loanService;

        public DenyLoan(ILoanService loanService)
        {
            _loanService = loanService;
        }

        protected override async ValueTask<Signal> ExecuteAsync(LoanApplication loan, ChainContext context)
        {
            await _loanService.DenyLoanAsync(loan, context.CancellationToken);
            return Signal.Static;
        }
    }
}