using System.Collections;
using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    public class PoolObjectTimer : MonoBehaviour, IPoolable
    {
        public float ReleaseAfterDuration = 1f;

        private Coroutine waitRoutine;
        private Pool pool;

        public void OnInstantiatedInPool(Pool pool)
        {
            
        }

        public void OnRelease(Pool pool, PoolObject poolObject)
        {
            this.CoroutineStop(waitRoutine);
        }

        private IEnumerator ReleaseAfterDurationRoutine()
        {
            yield return new WaitForSeconds(ReleaseAfterDuration);

            pool.Release(gameObject);
        }

        public void OnSpawn(Pool pool, PoolObject poolObject)
        {
            this.pool = pool;
            StartCoroutine(ReleaseAfterDurationRoutine());
        }
    }
}