using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Samples.LoanBroker.Services.CreditService
{
    public class CreditService : ICreditService
    {
        private readonly Random _random = new();

        public Task<CreditReport> GetCreditReportAsync(string ssn, CancellationToken cancellationToken)
        {
            var creditReport = new CreditReport(CreditScore: _random.Next(300, 900));
            return Task.FromResult(creditReport);
        }
    }
}