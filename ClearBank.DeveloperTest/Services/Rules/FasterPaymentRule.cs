using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Rules
{
    public class FasterPaymentsRule : IPaymentRule
    {
        public bool CanProcessPayment(Account account, MakePaymentRequest request)
        {
            if (account == null || request == null)
            {
                return false;
            }

            var canUseScheme = account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments);
            var hasSufficientFunds = account.Balance >= request.Amount;

            return canUseScheme && hasSufficientFunds;
        }
    }
}
