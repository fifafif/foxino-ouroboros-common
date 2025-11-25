using UnityEngine;

namespace Ouroboros.Common.Utils.Physics
{
    public static class PhysicsExtensions
    {
        public static Vector3 GetFirstContactPoint(this Collision collision)
        {
            if (collision.contactCount <= 0)
            {
                return collision.rigidbody.position;
            }

            return collision.contacts[0].point;
        }
    }
}