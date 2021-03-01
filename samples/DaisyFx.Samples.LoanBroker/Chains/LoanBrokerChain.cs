using DaisyFx.NCrontab;
using DaisyFx.Samples.LoanBroker.Links;
using DaisyFx.Samples.LoanBroker.Models;

namespace DaisyFx.Samples.LoanBroker.Chains
{
    public class LoanBrokerChain : ChainBuilder<Signal>
    {
        public override string Name { get; } = "LoanBroker";

        public override void ConfigureSources(SourceConnectorCollection<Signal> sources)
        {
            sources.Add<NCrontabSource>("Cron");
        }

        public override void ConfigureRootConnector(IConnectorLinker<Signal> root)
        {
            root.Link<GetLoanInquiry, LoanInquiry>()
                .Link<CreateLoanApplication, LoanApplication>()
                .If(application => !CreditScoreIsValid(application), then => then
                    .Link<DenyLoan, Signal>()
                )
                .If(CreditScoreIsValid, then => then
                    .Link<CreateLoanContract, LoanContract>()
                    .Link<ApproveLoan, Signal>()
                );
        }

        private static bool CreditScoreIsValid(LoanApplication application)
        {
            return application.CreditScore >= 700;
        }
    }
}