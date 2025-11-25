using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    public class PoolObject
    {
        public int Id => GameObject.GetInstanceID();

        public GameObject GameObject;
        public IPoolable Poolable;
        public bool IsSpawned;
    }
}