using System;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;

namespace DaisyFx.Samples.LoanBroker.Services.LoanService
{
    public class LoanService : ILoanService
    {
        private readonly Random _random = new();

        public Task<LoanInquiry> GetLoanInquiryAsync(int maxAgeDays, CancellationToken cancellationToken)
        {
            var inquiry = new LoanInquiry(
                Ssn: "1234",
                Name: "John Doe",
                Amount: _random.Next(1000, 20000),
                LoanDuration: _random.Next(1, 10));

            return Task.FromResult(inquiry);
        }

        public Task ApproveLoanAsync(LoanContract loanContract, CancellationToken cancellationToken)
        {
            return Task.Delay(200, cancellationToken);
        }

        public Task DenyLoanAsync(LoanApplication loanApplication, CancellationToken cancellationToken)
        {
            return Task.Delay(200, cancellationToken);
        }
    }
}