namespace ClearBank.DeveloperTest.Data
{
    public interface IDataStoreFactory
    {
        IAccountDataStore Create();
    }
}
