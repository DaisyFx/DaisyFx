namespace DaisyFx.Samples.LoanBroker.Models
{
    public record LoanInquiry(
        string Ssn,
        string Name,
        int Amount,
        int LoanDuration);
}