using UnityEngine;

namespace Ouroboros.Common.Utils.Physics
{
    public struct RigidbodyData
    {
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public RigidbodyConstraints Constraints;

        public static RigidbodyData Create(Rigidbody rigidbody)
        {
            return new RigidbodyData
            {
                Velocity = rigidbody.linearVelocity,
                AngularVelocity = rigidbody.angularVelocity,
                Constraints = rigidbody.constraints
            };
        }

        public static RigidbodyData CreateAndPauseBody(Rigidbody rigidbody)
        {
            var data = Create(rigidbody);

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            return data;
        }

        public void Apply(Rigidbody rigidbody)
        {
            rigidbody.constraints = Constraints;
            rigidbody.linearVelocity = Velocity;
            rigidbody.angularVelocity = AngularVelocity;
        }
    }
}