using ClearBank.DeveloperTest.Services.Rules;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentRuleTests
    {
        [Fact]
        public void GivenSchemeEnabled_WhenBacsPaymentRule_ThenAllow()
        {
            var rule = new BacsPaymentRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };
            var request = new MakePaymentRequest { Amount = 100m };

            var result = rule.CanProcessPayment(account, request);

            Assert.True(result);
        }

        [Fact]
        public void GivenSchemeNotEnabled_WhenBacsPaymentRule_ThenDeny()
        {
            var rule = new BacsPaymentRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
            };
            var request = new MakePaymentRequest { Amount = 100m };

            var result = rule.CanProcessPayment(account, request);

            Assert.False(result);
        }

        [Fact]
        public void GivenSchemeEnabledAndSufficientBalance_WhenFasterPaymentsRule_ThenAllow()
        {
            var rule = new FasterPaymentsRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 200m
            };
            var request = new MakePaymentRequest { Amount = 150m };

            var result = rule.CanProcessPayment(account, request);

            Assert.True(result);
        }

        [Fact]
        public void GivenSchemeEnabledAndInsufficientBalance_WhenFasterPaymentsRule_ThenDeny()
        {
            var rule = new FasterPaymentsRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 50m
            };
            var request = new MakePaymentRequest { Amount = 100m };

            var result = rule.CanProcessPayment(account, request);

            Assert.False(result);
        }

        [Fact]
        public void GivenSchemeEnabledAndLiveStatus_WhenChapsPaymentRule_ThenAllow()
        {
            var rule = new ChapsPaymentRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };
            var request = new MakePaymentRequest();

            var result = rule.CanProcessPayment(account, request);

            Assert.True(result);
        }

        [Fact]
        public void GivenSchemeEnabledAndStatusNotLive_WhenChapsPaymentRule_ThenDeny()
        {
            var rule = new ChapsPaymentRule();
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            var request = new MakePaymentRequest();

            var result = rule.CanProcessPayment(account, request);

            Assert.False(result);
        }
    }
}
