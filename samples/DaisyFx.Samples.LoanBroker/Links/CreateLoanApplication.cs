using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.CreditService;

namespace DaisyFx.Samples.LoanBroker.Links
{
    public class CreateLoanApplication : StatefulLink<LoanInquiry, LoanApplication>
    {
        private readonly ICreditService _creditService;

        public CreateLoanApplication(ICreditService creditService)
        {
            _creditService = creditService;
        }

        protected override async ValueTask<LoanApplication> ExecuteAsync(LoanInquiry input, ChainContext context)
        {
            var creditReport = await _creditService.GetCreditReportAsync(input.Ssn, context.CancellationToken);

            return new LoanApplication(
                input.Ssn,
                input.Name,
                input.Amount,
                input.LoanDuration,
                creditReport.CreditScore);
        }
    }
}