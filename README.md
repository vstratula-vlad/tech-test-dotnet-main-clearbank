# ClearBank Developer Test – Refactor Notes


## Key Changes

### 1. Introduced abstractions for data access

Previously, `PaymentService` instantiated `AccountDataStore` or `BackupAccountDataStore` directly and read configuration internally. This mixed responsibilities and made testing difficult.

To address this:

- `IAccountDataStore` abstracts account retrieval and persistence.
- `IDataStoreFactory` selects the correct data store based on configuration.
- `DataStoreFactory` implements this logic using the existing `DataStoreType` setting.

`PaymentService` now depends only on `IDataStoreFactory`, which makes it fully mockable and testable.

---

### 2. Extracted payment rules into dedicated classes

Originally, all the business rules for Bacs, FasterPayments, and Chaps were implemented within a single switch statement inside `PaymentService`.

To improve separation of concerns and allow easier extension:

- `IPaymentRule` defines a single `CanProcessPayment` method.
- `IPaymentRuleResolver` resolves the correct rule based on `PaymentScheme`.
- `PaymentRuleResolver` maps schemes to rule classes.
- Rule implementations:
  - `BacsPaymentRule`
  - `FasterPaymentsRule`
  - `ChapsPaymentRule`

Each rule class is now responsible only for the logic of its particular scheme.

---

### 3. Simplified PaymentService

`MakePayment` now does only four things:

1. Retrieve the account via the factory-created data store.
2. Resolve the appropriate payment rule.
3. Use the rule to validate the payment.
4. Deduct the amount and persist the updated account.

The signature of `MakePayment` remains unchanged (as required), but the logic around it is now clean, isolated, and adheres to SRP.

---

## Testing Approach

### Given/When/Then naming style

All tests use the naming convention:

`Given_<precondition>_When_<action>_Then_<expected outcome>`

This approach was chosen because it:

- Makes test intent immediately clear.
- Encodes the behaviour being tested into the name.
- Aligns with a behaviour-driven mindset.
- Produces a readable and maintainable test suite.

### Rule tests

Each rule has small, direct unit tests verifying its behaviour:

- **BacsPaymentRule:** allows payments only when the scheme is enabled.
- **FasterPaymentsRule:** requires the scheme to be enabled and the account to hold sufficient funds.
- **ChapsPaymentRule:** requires the scheme to be enabled and the account status to be `Live`.

### PaymentService tests

The service tests verify orchestration rather than rule logic:

- `GivenNullRequest_WhenMakePayment_ThenThrowArgumentNullException`
- `GivenAccountDoesNotExist_WhenMakePayment_ThenReturnFailure`
- `GivenRuleDeniesPayment_WhenMakePayment_ThenReturnFailureAndDoNotPersist`
- `GivenRuleAllowsPayment_WhenMakePayment_ThenDeductBalanceAndPersist`

All external dependencies are mocked to keep behaviour isolated and predictable.

---

## If More Time Were Available

Given additional time, the following improvements could be made:

- Returning richer failure reasons instead of a simple boolean.
- Implementing validation for negative or zero amounts.
- Adding integration tests using the concrete data store factory.
- Introducing a composition root to wire services together using dependency injection.

---

