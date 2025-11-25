using System;
using UnityEngine;

namespace Ouroboros.Common.Utils.GameObjects
{
    public class LifespanDestructor : MonoBehaviour
    {
        public Action OnBeginDestroy { get; set; }
        
        public float Lifespan;
        public bool IsKeepingAlive;

        private bool isDestroyed;

        public void Init(float lifespan, bool isKeepingAlive)
        {
            Lifespan = lifespan;
            IsKeepingAlive = isKeepingAlive;
            isDestroyed = false;
        }

        private void Update()
        {
            if (isDestroyed) return;

            Lifespan -= Time.deltaTime;
        
            if (Lifespan <= 0f)
            {
                isDestroyed = true;
                OnBeginDestroy?.Invoke();

                if (!IsKeepingAlive)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}