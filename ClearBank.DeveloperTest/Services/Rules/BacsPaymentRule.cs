using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Rules
{
    public class BacsPaymentRule : IPaymentRule
    {
        public bool CanProcessPayment(Account account, MakePaymentRequest request)
        {
            if (account == null || request == null)
            {
                return false;
            }

            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}
