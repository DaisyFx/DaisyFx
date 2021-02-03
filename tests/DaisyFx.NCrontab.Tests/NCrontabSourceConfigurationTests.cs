using NCrontab;
using Xunit;

namespace DaisyFx.NCrontab.Tests
{
    public class NCrontabSourceConfigurationTests
    {
        public static TheoryData<string, bool> InvalidExpressions => new TheoryData<string, bool>
        {
            {"", false},
            {"", true},
            {"*", false},
            {"* *", false},
            {"* * *", false},
            {"* * * *", false},
            {"* * * * *", true},
            {"* * * * * *", false}
        };

        [Theory]
        [MemberData(nameof(InvalidExpressions))]
        public void InvalidExpression_Throws_CrontabException(string expression, bool includeSeconds)
        {
            Assert.Throws<CrontabException>(() =>
            {
                new NCrontabSourceConfiguration
                {
                    CronExpression = expression,
                    IncludeSeconds = includeSeconds
                }.GetCurrentCron();
            });
        }
    }
}