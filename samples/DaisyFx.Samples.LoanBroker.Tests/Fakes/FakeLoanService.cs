using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.LoanService;

namespace DaisyFx.Samples.LoanBroker.Tests.Fakes
{
    public class FakeLoanService : ILoanService
    {
        private readonly Func<int, LoanInquiry> _loanInquiryProvider;

        public FakeLoanService(Func<int, LoanInquiry>? loanInquiryProvider = null)
        {
            _loanInquiryProvider = loanInquiryProvider ?? DefaultLoanInquiryProvider;
        }

        public List<LoanContract> ApprovedLoans { get; } = new();
        public List<LoanApplication> DeniedLoans { get; } = new();

        public Task<LoanInquiry> GetLoanInquiryAsync(int maxAgeDays, CancellationToken cancellationToken)
        {
            return Task.FromResult(_loanInquiryProvider(maxAgeDays));
        }

        public Task ApproveLoanAsync(LoanContract loanContract, CancellationToken cancellationToken)
        {
            ApprovedLoans.Add(loanContract);
            return Task.CompletedTask;
        }

        public Task DenyLoanAsync(LoanApplication loanApplication, CancellationToken cancellationToken)
        {
            DeniedLoans.Add(loanApplication);
            return Task.CompletedTask;
        }

        public static LoanInquiry DefaultLoanInquiryProvider(int maxAgeDays)
        {
           return TestDataFactory.CreateLoanInquiry();
        }
    }
}