using UnityEngine;
using UnityEngine.Events;

namespace Ouroboros.Common.Utils.Transforms
{
    public class TimeTransformer : MonoBehaviour
    {
        public Vector3 EndPosition;
        public float Duration;
        public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool DestroyOnFinish;
        public UnityEvent OnFinish;
        public bool IsManualStart;

        private Vector3 startPosition;
        private Vector3 endPositionInternal;
        private float elapsedTime;
        private bool hasFinished;

        private void Start()
        {
            ResetPositions();
        }
        
        public void ResetPositions()
        {
            startPosition = transform.localPosition;
            endPositionInternal = startPosition + EndPosition;
        }

        private void Update()
        {
            if (hasFinished
                || Duration <= 0f)
            {
                return;
            }

            if (elapsedTime > Duration)
            {
                elapsedTime = Duration;
                hasFinished = true;
            }

            var f = elapsedTime / Duration;
            transform.localPosition = Vector3.Lerp(startPosition, endPositionInternal, Curve.Evaluate(f));

            if (hasFinished)
            {
                OnFinish?.Invoke();

                if (DestroyOnFinish)
                {
                    Destroy(gameObject);
                }
            }

            elapsedTime += Time.deltaTime;
        }
    }
}