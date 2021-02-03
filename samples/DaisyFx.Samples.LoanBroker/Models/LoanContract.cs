namespace DaisyFx.Samples.LoanBroker.Models
{
    public record LoanContract(
        string Ssn,
        string Name,
        int Amount,
        int LoanDuration,
        int CreditScore,
        string BankName,
        int InterestRate);
}