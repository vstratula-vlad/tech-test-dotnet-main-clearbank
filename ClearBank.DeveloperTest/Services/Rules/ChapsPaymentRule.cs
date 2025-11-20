using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Rules
{
    public class ChapsPaymentRule : IPaymentRule
    {
        public bool CanProcessPayment(Account account, MakePaymentRequest request)
        {
            if (account == null || request == null)
            {
                return false;
            }

            var canUseScheme = account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps);
            var isLive = account.Status == AccountStatus.Live;

            return canUseScheme && isLive;
        }
    }
}
