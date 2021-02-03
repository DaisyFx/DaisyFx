# LoanBroker sample

This sample integrates 3 services (CreditService, BankServices and LoanService).

## Business workflow

1. Get a loan inquiry
2. Create a loan application with a credit score (using the CreditService)
3. Decline the application if the credit score is lower than 700
4. Get quotes for the loan from multiple banks (using the BankService)
5. Create a contract using the quote with lowest interest rate
6. Approve the loan (using the LoanService)