using System;
using Ouroboros.Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        public Action<Pool, GameObject> OnObjectInstantiatedInPool { get; set; }
        
        [SerializeField] private List<Pool> pools = new List<Pool>();
        [SerializeField] private List<Pool> runtimePools = new List<Pool>();

        private readonly Dictionary<int, Pool> poolMap = new Dictionary<int, Pool>();
        private readonly Dictionary<string, Pool> namePoolMap = new Dictionary<string, Pool>();
        private readonly Dictionary<string, PoolObjectRequest> requestMap = new Dictionary<string, PoolObjectRequest>();
        private readonly Dictionary<int, Pool> poolObjectMap = new Dictionary<int, Pool>();

        private void Awake()
        {
            foreach (var pool in pools)
            {
                if (pool.IsNameSetFromPrefabName)
                {
                    pool.Name = pool.Prefab.name;
                }
                
                pool.OnObjectInstantiatedInPool += OnObjectInstantiatedInPool;
                pool.Init();
                AddPool(pool);
            }
        }
        
        public bool HasPool(GameObject gameObject)
        {
            return poolMap.ContainsKey(gameObject.GetInstanceID());
        }

        public bool HasPool(string poolName)
        {
            return namePoolMap.ContainsKey(poolName);
        }

        public void CreatePool(GameObject gameObject, int agentPoolCount, string poolName = null)
        {
            CreatePool(gameObject, null, agentPoolCount, poolName);
        }

        public void CreatePool(
            GameObject gameObject, Transform parent, int agentPoolCount, string poolName = null)
        {
            var pool = new Pool(agentPoolCount, gameObject, parent);
            pool.Name = poolName;
            pool.OnObjectInstantiatedInPool += OnObjectInstantiatedInPool;
            pool.Init();

            AddPool(pool, poolName);
        }

        public void AddPool(Pool pool, string poolName = null)
        {
            var id = pool.Prefab.GetInstanceID();
            pool.SetId(id);
            poolName = poolName.IfNullOrEmptyReturnOther(pool.Prefab.name);

            if (poolMap.ContainsKey(id))
            {
                Debug.LogError($"[PoolManager] Duplicate Pool! pool={pool}, name={poolName}");
                return;
            }

            runtimePools.Add(pool);
            poolMap.Add(id, pool);
            namePoolMap.Add(poolName, pool);
        }

        public Pool GetPool(GameObject prefab)
        {
            poolMap.TryGetValue(prefab.GetInstanceID(), out var pool);
            return pool;
        }

        public Pool GetPool(string poolName)
        {
            namePoolMap.TryGetValue(poolName, out var pool);
            return pool;
        }

        public Pool GetPool(int poolId)
        {
            poolMap.TryGetValue(poolId, out var pool);
            return pool;
        }

        public GameObject Spawn(GameObject gameObject)
        {
            var pool = GetPool(gameObject);
            if (pool == null)
            {
                Debug.LogWarning(
                    $"[PoolManager] Pool not found for gameObject={gameObject}", gameObject);
                return null;
            }

            return SpawnFromPool(pool);
        }

        public GameObject SpawnFromPool(Pool pool)
        {
            var obj = pool.Spawn();
            if (obj != null)
            {
                poolObjectMap[obj.GetInstanceID()] = pool;
            }

            return obj;
        }

        public GameObject Spawn(string poolName)
        {
            var pool = GetPool(poolName);
            if (pool == null)
            {
                return null;
            }

            return SpawnFromPool(pool);
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (!poolObjectMap.TryGetValue(instance.GetInstanceID(), out var pool))
            {
                Debug.LogWarning(
                    $"[PoolManager] No pool found for instance to release! instance={instance}", instance);
            }

            ReleaseFromPool(pool, instance);            
        }

        private void ReleaseFromPool(Pool pool, GameObject instance)
        {
            pool.Release(instance);
            poolObjectMap.Remove(instance.GetInstanceID());
        }

        public void Release(GameObject gameObject, GameObject prefab)
        {
            var pool = GetPool(prefab);
            if (pool == null) return;

            ReleaseFromPool(pool, gameObject);            
        }

        public void Release(GameObject gameObject, int poolId)
        {
            var pool = GetPool(poolId);
            if (pool == null) return;

            ReleaseFromPool(pool, gameObject);            
        }

        public void Release(GameObject gameObject, string poolName)
        {
            var pool = GetPool(poolName);
            if (pool == null) return;

            ReleaseFromPool(pool, gameObject);            
        }

        public void Clear()
        {
            foreach (var kvp in poolMap)
            {
                kvp.Value.Clear();
            }

            poolMap.Clear();
            namePoolMap.Clear();
        }

        public void RequestPoolObject(PoolObjectRequest request, int countMultiplier)
        {
            request.Count *= countMultiplier;

            if (requestMap.TryGetValue(request.Id, out var objectRequest))
            {
                objectRequest.Count += request.Count;
            }
            else
            {
                requestMap.Add(request.Id, request);
            }
        }

        public void PoolAllRequests(Transform parent)
        {
            foreach (var kvp in requestMap)
            {
                if (HasPool(kvp.Value.Prefab)) continue;

                //Debug.Log($"XXX pooling {kvp.Value.Prefab}, id={kvp.Value.Prefab.GetInstanceID()}");

                CreatePool(kvp.Value.Prefab, parent, kvp.Value.Count, kvp.Value.Id);
            }

            requestMap.Clear();
        }
    }
}