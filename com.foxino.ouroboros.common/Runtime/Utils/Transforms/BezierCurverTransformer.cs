using Ouroboros.Common.Utils.Lerp;
using Ouroboros.Common.Utils.Math;
using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class BezierCurverTransformer : MonoBehaviour
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 P4;

        [SerializeField] private float duration = 1f;
        [SerializeField] private LerpMode lerpMode;

        private float elapsedTime;
        private bool isMoving;
        private Vector3 finalPosition;

        private void Start()
        {
            isMoving = true;
            finalPosition = transform.position;
        }

        private void Update()
        {
            if (isMoving)
            {
                Move();
            }
        }

        private void Move()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
            {
                elapsedTime = duration;
                isMoving = false;
            }

            var t = elapsedTime / duration;
            t = lerpMode.CalculateLerp(t);
            var p = CubicBezierCurve.GetPoint(P1, P2, P3, P4, t);

            transform.position = p;
        }
    }
}