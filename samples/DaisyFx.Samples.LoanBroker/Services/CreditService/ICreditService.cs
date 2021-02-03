using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Samples.LoanBroker.Services.CreditService
{
    public interface ICreditService
    {
        Task<CreditReport> GetCreditReportAsync(string ssn, CancellationToken cancellationToken);
    }
}