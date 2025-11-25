using Ouroboros.Common.Utils.Lerp;
using UnityEngine;
using UnityEngine.Events;

namespace Ouroboros.Common.Utils.Transforms
{
    public class ScaleTransformer : MonoBehaviour
    {
        public UnityLifecycleMode BeginEvent;
        public bool IsCustomStartScale;
        public Vector3 StartScale;
        public Vector3 EndScale = Vector3.one;
        public float Duration = 1f;
        public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool IsUsingCurve;
        public bool DestroyOnFinish;
        public UnityEvent OnFinish;
        public LerpMode LerpMode = LerpMode.Linear;

        private Vector3 startScale;
        private float elapsedTime;
        private bool hasFinished;
        private bool hasBegun;

        private void Awake()
        {
            if (BeginEvent == UnityLifecycleMode.Awake)
            {
                Begin();
            }
        }

        private void Start()
        {
            if (BeginEvent == UnityLifecycleMode.Start)
            {
                Begin();
            }
        }

        private void OnEnable()
        {
            if (BeginEvent == UnityLifecycleMode.OnEnable)
            {
                Begin();
            }
        }

        private void Update()
        {
            if (!hasBegun
                || hasFinished
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
            if (IsUsingCurve)
            {
                f = Curve.Evaluate(f);
            }
            else
            {
                f = LerpMath.CalculateLerp(f, LerpMode);
            }

            transform.localScale = Vector3.Lerp(startScale, EndScale, f);

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

        public void Begin()
        {
            hasBegun = true;
            hasFinished = false;
            elapsedTime = 0f;

            if (IsCustomStartScale)
            {
                transform.localScale = StartScale;
                startScale = StartScale;
            }
            else
            {
                startScale = transform.localScale;
            }
        }
    }
}