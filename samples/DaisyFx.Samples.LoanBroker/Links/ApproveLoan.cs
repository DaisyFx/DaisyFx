using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.LoanService;

namespace DaisyFx.Samples.LoanBroker.Links
{
    public class ApproveLoan : StatefulLink<LoanContract, Signal>
    {
        private readonly ILoanService _loanService;

        public ApproveLoan(ILoanService loanService)
        {
            _loanService = loanService;
        }

        protected override async ValueTask<Signal> ExecuteAsync(LoanContract loanContract, ChainContext context)
        {
            await _loanService.ApproveLoanAsync(loanContract, context.CancellationToken);
            return Signal.Static;
        }
    }
}