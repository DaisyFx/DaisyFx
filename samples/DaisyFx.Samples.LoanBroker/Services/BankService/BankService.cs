using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Samples.LoanBroker.Services.BankService
{
    public class BankService : IBankService
    {
        private readonly (string name, int baseInterestRate)[] _banks =
        {
            new("Bank 1", 3),
            new("Bank 2", 2),
            new("Bank 3", 5)
        };

        public async Task<LoanQuote[]> GetLoanQuotesAsync(int amount,
            int loanDuration,
            int creditScore,
            CancellationToken cancellationToken)
        {
            await Task.Delay(200, cancellationToken);

            return _banks
                .Select(bank =>
                {
                    var (name, baseInterestRate) = bank;
                    var interest = CalculateInterestRate(baseInterestRate, amount, loanDuration, creditScore);
                    return new LoanQuote(name, interest);
                })
                .ToArray();
        }

        private static int CalculateInterestRate(int baseInterestRate, int amount, int loanDuration, int creditScore)
        {
            return baseInterestRate +
                   GetLoanAmountFactor(amount) +
                   GetLoanDurationFactor(loanDuration) +
                   GetCreditScoreFactor(creditScore);
        }

        private static int GetLoanAmountFactor(int amount)
        {
            return amount switch
            {
                < 10_000 => 8,
                < 50_000 => 5,
                < 100_000 => 2,
                _ => 1
            };
        }

        private static int GetLoanDurationFactor(int loanDuration)
        {
            return loanDuration switch
            {
                < 5 => 8,
                < 10 => 5,
                < 20 => 2,
                _ => 1
            };
        }

        private static int GetCreditScoreFactor(int creditScore)
        {
            return creditScore switch
            {
                < 500 => 8,
                < 800 => 5,
                < 900 => 2,
                _ => 1
            };
        }
    }
}