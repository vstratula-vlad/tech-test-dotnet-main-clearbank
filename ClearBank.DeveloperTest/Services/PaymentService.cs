using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStoreFactory _dataStoreFactory;
        private readonly IPaymentRuleResolver _paymentRuleResolver;

        public PaymentService(IDataStoreFactory dataStoreFactory, IPaymentRuleResolver paymentRuleResolver)
        {
            _dataStoreFactory = dataStoreFactory ?? throw new ArgumentNullException(nameof(dataStoreFactory));
            _paymentRuleResolver = paymentRuleResolver ?? throw new ArgumentNullException(nameof(paymentRuleResolver));
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new MakePaymentResult { Success = false };

            var dataStore = _dataStoreFactory.Create();
            var account = dataStore.GetAccount(request.DebtorAccountNumber);

            if (account == null)
            {
                return result;
            }

            var paymentRule = _paymentRuleResolver.Resolve(request.PaymentScheme);

            if (!paymentRule.CanProcessPayment(account, request))
            {
                return result;
            }

            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            result.Success = true;
            return result;
        }
    }
}
