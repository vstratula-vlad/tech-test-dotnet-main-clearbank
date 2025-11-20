using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IDataStoreFactory> _dataStoreFactoryMock = new();
        private readonly Mock<IAccountDataStore> _accountDataStoreMock = new();
        private readonly Mock<IPaymentRuleResolver> _paymentRuleResolverMock = new();
        private readonly Mock<IPaymentRule> _paymentRuleMock = new();

        private readonly PaymentService _sut;

        public PaymentServiceTests()
        {
            _dataStoreFactoryMock.Setup(f => f.Create()).Returns(_accountDataStoreMock.Object);

            _sut = new PaymentService(
                _dataStoreFactoryMock.Object,
                _paymentRuleResolverMock.Object);
        }

        [Fact]
        public void GivenNullRequest_WhenMakePayment_ThenThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.MakePayment(null));
        }

        [Fact]
        public void GivenAccountDoesNotExist_WhenMakePayment_ThenReturnFailure()
        {
            var request = CreateRequest();

            _accountDataStoreMock
                .Setup(s => s.GetAccount(request.DebtorAccountNumber))
                .Returns((Account)null);

            var result = _sut.MakePayment(request);

            Assert.False(result.Success);
            _accountDataStoreMock.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void GivenRuleDeniesPayment_WhenMakePayment_ThenReturnFailureAndDoNotPersist()
        {
            var request = CreateRequest(amount: 50m);
            var account = CreateAccount(balance: 100m);

            _accountDataStoreMock
                .Setup(s => s.GetAccount(request.DebtorAccountNumber))
                .Returns(account);

            _paymentRuleResolverMock
                .Setup(r => r.Resolve(request.PaymentScheme))
                .Returns(_paymentRuleMock.Object);

            _paymentRuleMock
                .Setup(r => r.CanProcessPayment(account, request))
                .Returns(false);

            var result = _sut.MakePayment(request);

            Assert.False(result.Success);
            Assert.Equal(100m, account.Balance);
            _accountDataStoreMock.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void GivenRuleAllowsPayment_WhenMakePayment_ThenDeductBalanceAndPersist()
        {
            var request = CreateRequest(amount: 30m);
            var account = CreateAccount(balance: 100m);

            _accountDataStoreMock
                .Setup(s => s.GetAccount(request.DebtorAccountNumber))
                .Returns(account);

            _paymentRuleResolverMock
                .Setup(r => r.Resolve(request.PaymentScheme))
                .Returns(_paymentRuleMock.Object);

            _paymentRuleMock
                .Setup(r => r.CanProcessPayment(account, request))
                .Returns(true);

            var result = _sut.MakePayment(request);

            Assert.True(result.Success);
            Assert.Equal(70m, account.Balance);
            _accountDataStoreMock.Verify(s => s.UpdateAccount(account), Times.Once);
        }

        private static MakePaymentRequest CreateRequest(
            decimal amount = 50m,
            PaymentScheme paymentScheme = PaymentScheme.Bacs)
        {
            return new MakePaymentRequest
            {
                DebtorAccountNumber = "DEBTOR-123",
                CreditorAccountNumber = "CREDITOR-789",
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                PaymentScheme = paymentScheme
            };
        }

        private static Account CreateAccount(
            decimal balance = 100m,
            AllowedPaymentSchemes allowedSchemes = AllowedPaymentSchemes.Bacs,
            AccountStatus status = AccountStatus.Live)
        {
            return new Account
            {
                AccountNumber = "DEBTOR-123",
                Balance = balance,
                AllowedPaymentSchemes = allowedSchemes,
                Status = status
            };
        }
    }
}
