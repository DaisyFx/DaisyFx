namespace DaisyFx.Samples.LoanBroker.Models
{
    public record LoanApplication(
        string Ssn,
        string Name,
        int Amount,
        int LoanDuration,
        int CreditScore);
}