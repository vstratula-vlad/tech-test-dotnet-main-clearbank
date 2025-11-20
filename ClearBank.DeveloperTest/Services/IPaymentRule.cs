using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentRule
    {
        bool CanProcessPayment(Account account, MakePaymentRequest request);
    }
}
