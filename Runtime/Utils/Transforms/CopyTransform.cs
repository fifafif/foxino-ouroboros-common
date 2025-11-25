using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class CopyTransform : MonoBehaviour
    {
        public Transform Target;

        [SerializeField] private UnityUpdateMode updateMode = UnityUpdateMode.Update;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private bool isMatchingPosition;
        [SerializeField] private bool isMatchingRotation;

        private void Update()
        {
            if (updateMode != UnityUpdateMode.Update) return;

            UpdateTransform();
        }

        private void LateUpdate()
        {
            if (updateMode != UnityUpdateMode.LateUpdate) return;

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (Target == null) return;

            if (isMatchingPosition
                && isMatchingRotation)
            {
                transform.MatchPositionAndRotation(Target);
                transform.Rotate(rotationOffset, Space.Self);
            }
            else
            {
                if (isMatchingPosition)
                {
                    transform.position = Target.position;
                }

                if (isMatchingRotation)
                {
                    transform.rotation = Target.rotation;
                    transform.Rotate(rotationOffset, Space.Self);
                }
            }
        }
    }
}