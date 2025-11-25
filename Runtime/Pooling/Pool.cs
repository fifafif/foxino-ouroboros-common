using System;
using System.Collections.Generic;
using Ouroboros.Common.Logging;
using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    [Serializable]
    public class Pool
    {
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public GameObject Prefab => prefab;
        public bool IsEnablingObjectOnSpawn { get; set; } = true;
        public Action<Pool, GameObject> OnObjectInstantiatedInPool { get; set; }
        public bool IsNameSetFromPrefabName => isNameSetFromPrefabName;

        [SerializeField] private GameObject prefab;
        [SerializeField] private string name;
        [SerializeField] private Transform parent;
        [SerializeField] private int poolCount = 10;
        [SerializeField] private bool canInstantiateMore = true;
        [SerializeField] private bool isNameSetFromPrefabName;

        private readonly Stack<PoolObject> objects;
        private readonly Dictionary<int, PoolObject> objectMap;

        public Pool()
        {
            objects = new Stack<PoolObject>(poolCount);
            objectMap = new Dictionary<int, PoolObject>(poolCount);
        }

        public Pool(int poolCount, GameObject prefab)
        {
            this.prefab = prefab;
            this.poolCount = poolCount;
            objects = new Stack<PoolObject>(poolCount);
            objectMap = new Dictionary<int, PoolObject>(poolCount);
        }

        public Pool(int poolCount, GameObject prefab, Transform parent)
        {
            this.prefab = prefab;
            this.poolCount = poolCount;
            this.parent = parent;
            objects = new Stack<PoolObject>(poolCount);
            objectMap = new Dictionary<int, PoolObject>(poolCount);
        }

        public void Init()
        {
            InstantiateObjects(poolCount);
        }

        public GameObject Spawn()
        {
            if (objects.Count <= 0)
            {
                if (canInstantiateMore)
                {
                    Logs.Warning<PoolManager>($"Pool at capacity! Instantiating more. pool={this}");
                    InstantiateObjects(poolCount);
                }
                else
                {
                    throw new Exception("No more objects in the pool!");
                }
            }

            var poolObject = objects.Pop();

            if (IsEnablingObjectOnSpawn)
            {
                poolObject.GameObject.SetActive(true);
            }
            
            poolObject.IsSpawned = true;
            poolObject.Poolable?.OnSpawn(this, poolObject);

            return poolObject.GameObject;
        }

        public void Release(GameObject gameObject)
        {
            Release(gameObject.GetInstanceID());
        }

        public void Release(int objectId)
        {
            if (!objectMap.TryGetValue(objectId, out var poolObject))
            {
                Logs.Warning<PoolManager>($"Pool: Trying to release object not from pool!");
                return;
            }
            
            
            if (!poolObject.IsSpawned)
            {
                Logs.Warning<PoolManager>($"Pool: Trying to release object which is not spawned! object={poolObject.GameObject}", poolObject.GameObject);
                return;
            }

            poolObject.GameObject.SetActive(false);
            poolObject.IsSpawned = false;
            poolObject.Poolable?.OnRelease(this, poolObject);
            objects.Push(poolObject);
        }

        private void InstantiateObjects(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var poolObject = Instantiate(prefab);
                poolObject.GameObject.SetActive(false);
                objects.Push(poolObject);
                objectMap.Add(poolObject.Id, poolObject);

                OnObjectInstantiatedInPool?.Invoke(this, poolObject.GameObject);
            }
        }


        private PoolObject Instantiate(GameObject prefab)
        {
            var instance = GameObject.Instantiate(prefab, parent);
            var poolObject = new PoolObject
            {
                GameObject = instance,
                Poolable = instance.GetComponent<IPoolable>()
            };

            if (poolObject.Poolable != null)
            {
                poolObject.Poolable.OnInstantiatedInPool(this);
            }    

            return poolObject;
        }

        public void Clear()
        {
            foreach (var kvp in objectMap)
            {
                UnityEngine.Object.Destroy(kvp.Value.GameObject);
            }
        }

        public override string ToString()
        {
            return $"[Pool Prefab={prefab}, Count={poolCount}]";
        }
    }
}