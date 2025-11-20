using System;
using System.Configuration;

namespace ClearBank.DeveloperTest.Data
{
    public class DataStoreFactory : IDataStoreFactory
    {
        public IAccountDataStore Create()
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            if (string.Equals(dataStoreType, "Backup", StringComparison.OrdinalIgnoreCase))
            {
                return new BackupAccountDataStore();
            }

            return new AccountDataStore();
        }
    }
}
