using DaisyFx.NCrontab;
using DaisyFx.Samples.LoanBroker.Links;
using DaisyFx.Samples.LoanBroker.Models;

namespace DaisyFx.Samples.LoanBroker.Chains
{
    public class LoanBrokerChain : ChainBuilder<Signal>
    {
        public override string Name { get; } = "LoanBroker";

        public override void ConfigureRootConnector(IConnectorLinker<Signal> root)
        {
            root.Link<GetLoanInquiry, LoanInquiry>()
                .Link<CreateLoanApplication, LoanApplication>()
                .Conditional(CreditScoreIsBad, builder => builder
                    .Link<DenyLoan, Signal>()
                    .Complete("Denied")
                )
                .Link<CreateLoanContract, LoanContract>()
                .Link<ApproveLoan, Signal>();
        }

        public override void ConfigureSources(SourceConnectorCollection<Signal> sources)
        {
            sources.Add<NCrontabSource>("Cron");
        }

        private static bool CreditScoreIsBad(LoanApplication application)
        {
            return application.CreditScore < 700;
        }
    }
}