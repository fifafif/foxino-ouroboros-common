using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    public class PoolObjectTimer : MonoBehaviour, IPoolable
    {
        public float ReleaseAfterDuration = 1f;

        private Coroutine waitRoutine;

        public void OnInstantiatedInPool(Pool pool)
        {
            
        }

        public void OnRelease(Pool pool, PoolObject poolObject)
        {
            this.CoroutineStop(waitRoutine);
        }

        public void OnSpawn(Pool pool, PoolObject poolObject)
        {
            this.CoroutineStop(waitRoutine);
            waitRoutine = this.CoroutineWait(ReleaseAfterDuration, () => 
            {
                pool.Release(poolObject.Id);
            });
        }
    }
}