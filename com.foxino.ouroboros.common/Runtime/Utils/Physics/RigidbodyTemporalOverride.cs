using System;
using UnityEngine;

namespace Ouroboros.Common.Utils.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyTemporalOverride : MonoBehaviour
    {
        private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void AddVelocityOverride(
            Vector3 velocity, float duration, float drag, Action onFinish = null)
        {
            var origVelocity = rigidbody.linearVelocity;
            var origDrag = rigidbody.linearDamping;

            rigidbody.linearVelocity = velocity;
            rigidbody.linearDamping = drag;

            this.CoroutineWait(duration, () =>
            {
                rigidbody.linearVelocity = origVelocity;
                rigidbody.linearDamping = origDrag;
                onFinish?.Invoke();
            });
        }
    }
}