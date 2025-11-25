using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class CopyTransform2D : MonoBehaviour
    {
        public UnityUpdateMode UpdateMode = UnityUpdateMode.Update;
        public Transform Target;

        public bool IsMatchingPositionOnly;

        private void Update()
        {
            if (UpdateMode != UnityUpdateMode.Update) return;

            UpdateTransform();
        }

        private void LateUpdate()
        {
            if (UpdateMode != UnityUpdateMode.LateUpdate) return;

            UpdateTransform();
        }
        
        private void FixedUpdate()
        {
            if (UpdateMode != UnityUpdateMode.FixedUpdate) return;

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (Target == null) return;

            if (IsMatchingPositionOnly)
            {
                var pos = Target.position;
                pos.z = transform.position.z;
                transform.position = pos;
            }
            else
            {
                transform.MatchPositionAndRotation2D(Target);
            }
        }
    }
}