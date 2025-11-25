using Ouroboros.Common.Utils.Lerp;
using Ouroboros.Common.Utils.Math;
using UnityEngine;
using UnityEngine.Events;

namespace Ouroboros.Common.Utils.Transforms
{
    public class AgentDestinationTransformer : MonoBehaviour
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 P4;

        [SerializeField] private float duration = 1f;
        [SerializeField] private LerpMode lerpMode;
        [SerializeField] private Transform destination;
        [SerializeField] private Vector3 destinationPosition;
        [SerializeField] private UnityEvent onReachDestination;

        private float elapsedTime;
        private bool isMoving;
        private Vector3 finalPosition;

        private void Start()
        {
            if (destination == null)
            {
                finalPosition = destinationPosition;
            }
            else
            {
                finalPosition = destination.position;
            }
        }

        public void Begin()
        {
            isMoving = true;
            SetCurveParams();
        }

        private void Update()
        {
            if (isMoving)
            {
                Move();
            }
        }

        private void SetCurveParams()
        {
            P1 = transform.position;
            P4 = finalPosition;

            var toDestination = P4 - P1;
            var dirToDestination = toDestination.normalized;
            var distToDestination = toDestination.magnitude;

            P2 = P1 + dirToDestination * distToDestination * 0.5f + P2;
            P3 = P1 + dirToDestination * distToDestination * 0.5f + P3;
        }

        private void Move()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
            {
                elapsedTime = duration;
                isMoving = false;
                onReachDestination?.Invoke();
            }

            var t = elapsedTime / duration;
            t = lerpMode.CalculateLerp(t);
            var p = CubicBezierCurve.GetPoint(P1, P2, P3, P4, t);

            transform.position = p;
        }
    }
}