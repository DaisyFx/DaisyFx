using System.Threading.Tasks;
using DaisyFx.Samples.LoanBroker.Models;
using DaisyFx.Samples.LoanBroker.Services.LoanService;

namespace DaisyFx.Samples.LoanBroker.Links
{
    public class GetLoanInquiry : StatefulLink<Signal, LoanInquiry>
    {
        private readonly ILoanService _loanService;
        private readonly GetLoanInquiryConfiguration _configuration;

        public GetLoanInquiry(ILoanService loanService)
        {
            _loanService = loanService;
            _configuration = ReadConfiguration<GetLoanInquiryConfiguration>();
        }

        protected override async ValueTask<LoanInquiry> Invoke(Signal input, ChainContext context)
        {
            return await _loanService.GetLoanInquiryAsync(_configuration.MaxAgeDays, context.CancellationToken);
        }
    }

    public class GetLoanInquiryConfiguration
    {
        public int MaxAgeDays { get; set; }
    }
}