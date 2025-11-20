using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentRuleResolver
    {
        IPaymentRule Resolve(PaymentScheme paymentScheme);
    }
}
