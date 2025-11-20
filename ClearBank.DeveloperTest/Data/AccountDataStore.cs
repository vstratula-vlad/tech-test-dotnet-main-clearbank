using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public class AccountDataStore : IAccountDataStore
    {
        public Account GetAccount(string accountNumber)
        {
            return new Account();
        }

        public void UpdateAccount(Account account)
        {
            // Update account in database, code removed for brevity
        }
    }
}
