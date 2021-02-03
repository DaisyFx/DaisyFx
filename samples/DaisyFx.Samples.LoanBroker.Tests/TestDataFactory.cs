using DaisyFx.Samples.LoanBroker.Models;

namespace DaisyFx.Samples.LoanBroker.Tests
{
    public static class TestDataFactory
    {
        public static LoanContract CreateLoanContract()
        {
            return new ("1234", "Test", 1234, 4, 4, "Test", 4);
        }

        public static LoanInquiry CreateLoanInquiry()
        {
            return new ("1234", "Test", 1234, 4);
        }

        public static LoanApplication CreateLoanApplication()
        {
            return new ("1234", "Test", 1234, 4, 800);
        }
    }
}