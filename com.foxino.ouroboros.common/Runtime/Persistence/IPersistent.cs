namespace Ouroboros.Common.Persistence
{
    public interface IPersistence
    {
        void Save<T>(T data, string key);
        T Load<T>(string key);
        void Delete(string key);
        bool HasData(string key);
    }
}
