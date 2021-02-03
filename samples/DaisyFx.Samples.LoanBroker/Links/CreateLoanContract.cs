using System;
using System.Linq;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.BankService;

namespace DaisyFx.Samples.LoanBroker.Links
{
    public class CreateLoanContract : StatefulLink<LoanApplication, LoanContract>
    {
        private readonly IBankService _bankService;

        public CreateLoanContract(IBankService bankService)
        {
            _bankService = bankService;
        }

        protected override async ValueTask<LoanContract> Invoke(LoanApplication input, ChainContext context)
        {
            var quotes = await _bankService.GetLoanQuotesAsync(input.Amount, input.LoanDuration, input.CreditScore,
                context.CancellationToken);

            var lowestQuote = quotes
                .OrderBy(q => q.InterestRate)
                .FirstOrDefault() ?? throw new Exception("No quote available");

            return new LoanContract(
                input.Ssn,
                input.Name,
                input.Amount,
                input.LoanDuration,
                input.CreditScore,
                lowestQuote.BankName,
                lowestQuote.InterestRate);
        }
    }
}