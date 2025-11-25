namespace Ouroboros.Common.Pooling
{
    public interface IPoolable
    {
        void OnInstantiatedInPool(Pool pool);
        void OnRelease(Pool pool, PoolObject poolObject);
        void OnSpawn(Pool pool, PoolObject poolObject);
    }
}