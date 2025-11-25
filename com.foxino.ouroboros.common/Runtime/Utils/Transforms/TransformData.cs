using System;
using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformData(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public void Apply(Transform transform)
        {
            transform.SetPositionAndRotation(Position, Rotation);
        }
    }
}
