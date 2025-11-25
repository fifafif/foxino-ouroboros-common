using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class DirectionalTransformer : MonoBehaviour
    {
        public Transform Source;
        public Transform Target;
        public float DistanceFromSource;
        public bool IsInverseRotation;

        private void Update()
        {
            if (Target == null
                || Source == null)
            {
                return;
            }

            var direction = Target.position - Source.position;

            var rotation = Quaternion.LookRotation(IsInverseRotation ? -direction : direction);

            /*if (IsInverseRotation)
            {
                var direction = Target.position - transform.position;
                transform.LookAt(transform.position - direction);
            }
            else
            {
                transform.LookAt(Target);
            }*/

            var position = direction.normalized * DistanceFromSource + Source.position;

            transform.SetPositionAndRotation(position, rotation);
        }
    }
}