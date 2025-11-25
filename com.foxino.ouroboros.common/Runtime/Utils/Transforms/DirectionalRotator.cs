using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class DirectionalRotator : MonoBehaviour
    {
        public Transform Target;
        public bool IsInverseRotation;

        private void Update()
        {
            if (Target == null) return;

            if (IsInverseRotation)
            {
                var direction = Target.position - transform.position;
                transform.LookAt(transform.position - direction);
            }
            else
            {
                transform.LookAt(Target);
            }
        }
    }
}