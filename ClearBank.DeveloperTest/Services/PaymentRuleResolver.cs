using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Services.Rules;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentRuleResolver : IPaymentRuleResolver
    {
        private readonly IDictionary<PaymentScheme, IPaymentRule> _rules;

        public PaymentRuleResolver()
        {
            _rules = new Dictionary<PaymentScheme, IPaymentRule>
            {
                { PaymentScheme.Bacs, new BacsPaymentRule() },
                { PaymentScheme.FasterPayments, new FasterPaymentsRule() },
                { PaymentScheme.Chaps, new ChapsPaymentRule() }
            };
        }

        public IPaymentRule Resolve(PaymentScheme paymentScheme)
        {
            if (_rules.TryGetValue(paymentScheme, out var rule))
            {
                return rule;
            }

            throw new ArgumentOutOfRangeException(
                nameof(paymentScheme),
                paymentScheme,
                "Unsupported payment scheme.");
        }
    }
}
