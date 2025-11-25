using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils.Lerp
{
    public class Lerping : MonoBehaviour
    {
        public class PositionLerp : LerpBase
        {
            public Vector3 DestinationPosition;
            public Quaternion DestinationRotation;
            public Vector3 DestinationScale;

            public override bool IsValid => true;

            public override Vector3 GetDestinationPosition()
            {
                return DestinationPosition;
            }

            public override Quaternion GetDestinationRotation()
            {
                return DestinationRotation;
            }

            public override Vector3 GetDestinationScale()
            {
                return DestinationScale;
            }
        }

        public class DestinationLerp : LerpBase
        {
            public override Vector3 GetDestinationPosition() 
            {
                return Destination.position; 
            }

            public override Vector3 GetDestinationScale()
            {
                return Destination.lossyScale;
            }
            public override Quaternion GetDestinationRotation()
            {
                return Destination.rotation;
            }

            public override bool IsValid => Destination != null;

            public Transform Destination;
        }

        public abstract class LerpBase
        {
            public abstract Vector3 GetDestinationPosition();
            public abstract Vector3 GetDestinationScale();
            public abstract Quaternion GetDestinationRotation();
            public virtual bool IsValid { get; }
            public bool IsMarkedForRemove { get; set; }

            public LerpMode LerpMode;
            public Transform Target;
            public float Duration;
            public float ElapsedTime;
            public Vector3 OriginPosition;
            public Quaternion OriginRotation;
            public Vector3 OriginScale;
            public Action OnFinish;
            public bool IsRotating = true;
        }

        public class LerpData
        {
            public Vector3 OriginPosition;
            public Quaternion OriginRotation;
            public Vector3 OriginScale;
            public LerpMode LerpMode;
            public Transform Target;
            public Transform Destination;
            public float Duration;
            public Action OnFinish;
            public bool IsRotating = true;
        }

        public class LerpOverTime
        {
            public bool IsDone;
            public Coroutine Coroutine;
        }

        private static Lerping instance;
        private static List<LerpBase> activeLerps = new List<LerpBase>();
        private static List<LerpBase> newLerps = new List<LerpBase>();
        private static List<int> lerpIndicesToRemove = new List<int>();

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public static void RemoveLerp(Transform target)
        {
            for (var i = activeLerps.Count - 1; i >= 0; --i)
            {
                if (activeLerps[i].Target == target)
                {
                    activeLerps[i].IsMarkedForRemove = true;
                }
            }
        }

        public static void RemoveLerp(LerpBase activeLerping)
        {
            activeLerping.IsMarkedForRemove = true;
        }

        private static void MarkActiveLerpIndexForRemove(int index)
        {
            if (index < 0
                || lerpIndicesToRemove.Contains(index))
            {
                return;
            }

            lerpIndicesToRemove.Add(index);
        }

        public static LerpBase StartLerp(
            Transform target, 
            Transform destination, 
            float duration, 
            LerpMode mode = LerpMode.Linear, 
            Action onFinish = null)
        {
            return StartLerp(new LerpData
            {
                LerpMode = mode,
                OriginPosition = target.position,
                OriginRotation = target.rotation,
                OriginScale = target.localScale,
                Target = target,
                Destination = destination,
                Duration = duration,
                OnFinish = onFinish
            });
        }

        public static LerpBase StartLerp(LerpData lerpData)
        { 
            AssureInstance();

            var lerp = new DestinationLerp
            {
                LerpMode = lerpData.LerpMode,
                OriginPosition = lerpData.Target.position,
                OriginRotation = lerpData.Target.rotation,
                OriginScale = lerpData.Target.localScale,
                Target = lerpData.Target,
                Destination = lerpData.Destination,
                Duration = lerpData.Duration,
                OnFinish = lerpData.OnFinish,
                IsRotating = lerpData.IsRotating
            };

            newLerps.Add(lerp);
            return lerp;
        }

        public static LerpBase StartLerp(
            Transform target,
            Vector3 destionationPosition,
            Quaternion destionationRotation,
            Vector3 destionationScale,
            float duration,
            LerpMode mode = LerpMode.Linear, 
            Action onFinish = null)
        {
            AssureInstance();

            var lerp = new PositionLerp
            {
                LerpMode = mode,
                OriginPosition = target.position,
                OriginRotation = target.rotation,
                OriginScale = target.localScale,
                Target = target,
                DestinationPosition = destionationPosition,
                DestinationRotation = destionationRotation,
                DestinationScale = destionationScale,
                Duration = duration,
                OnFinish = onFinish
            };

            newLerps.Add(lerp);
            return lerp;
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            for (var i = activeLerps.Count - 1; i >= 0; --i)
            {
                if (activeLerps[i].IsMarkedForRemove)
                {
                    activeLerps.RemoveAt(i);
                }

                if (UpdateLerp(activeLerps[i], dt))
                {
                    activeLerps[i].OnFinish?.Invoke();
                    activeLerps.RemoveAt(i);
                }
            }

            for (int i = 0; i < newLerps.Count; ++i)
            {
                activeLerps.Add(newLerps[i]);
            }

            newLerps.Clear();
        }

        private bool UpdateLerp(LerpBase lerp, float deltaTime)
        {
            if (lerp.Target == null
                || !lerp.IsValid)
            {
                return true;
            }

            lerp.ElapsedTime += deltaTime;

            float t;
            var isDone = false;
            
            if (lerp.ElapsedTime > lerp.Duration
                || lerp.Duration <= 0f)
            {
                t = 1f;
                isDone = true;
            }
            else
            {
                t = lerp.ElapsedTime / lerp.Duration;
                t = LerpMath.CalculateLerp(t, lerp.LerpMode);
            }

            var pos = Vector3.Lerp(lerp.OriginPosition, lerp.GetDestinationPosition(), t);
            
            if (lerp.IsRotating)
            {
                var rot = Quaternion.Slerp(lerp.OriginRotation, lerp.GetDestinationRotation(), t);
                lerp.Target.SetPositionAndRotation(pos, rot);
            }
            else
            {
                lerp.Target.position = pos;
            }

            var scale  = Vector3.Lerp(lerp.OriginScale, lerp.GetDestinationScale(), t);

            lerp.Target.localScale = scale;

            return isDone;
        }

        public static LerpOverTime StartLerp(float duration, Action<float> onProgress, bool useUnscaledTime = false, Action onFinish = null)
        {
            AssureInstance();

            var lerp = new LerpOverTime();

            lerp.Coroutine = instance.StartCoroutine(
                instance.LerpRoutine(lerp, duration, onProgress, useUnscaledTime, onFinish));

            return lerp;
        }

        public static void StopLerp(LerpOverTime lerpOverTime)
        {
            if (lerpOverTime == null) return;

            AssureInstance();

            lerpOverTime.IsDone = true;
            if (lerpOverTime.Coroutine != null)
            {
                instance.StopCoroutine(lerpOverTime.Coroutine);
            }
        }

        private IEnumerator LerpRoutine(LerpOverTime lerp, float duration, Action<float> onProgress, bool useUnscaledTime, Action onFinish)
        {
            if (duration <= 0f)
            {
                lerp.IsDone = true;
                onProgress?.Invoke(1f);
                onFinish?.Invoke();
                yield break;
            }

            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (useUnscaledTime)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                }
                else
                {
                    elapsedTime += Time.deltaTime;
                }

                onProgress(elapsedTime / duration);

                yield return null;
            }

            lerp.IsDone = true;
            onProgress(1f);
            onFinish?.Invoke();
        }

        private static void AssureInstance()
        {
            if (instance == null)
            {
                new GameObject("Lerping", typeof(Lerping));
            }
        }
    }
}
