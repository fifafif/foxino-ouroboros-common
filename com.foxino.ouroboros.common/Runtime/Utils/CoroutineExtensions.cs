using System;
using System.Collections;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class CoroutineExtensions
    {
        public static Coroutine CoroutineWait(this MonoBehaviour mono, float timeSeconds, Action onFinish)
        {
            return mono.StartCoroutine(CoroutineWait(timeSeconds, onFinish));
        }

        public static IEnumerator CoroutineWait(float timeSeconds, Action onFinish)
        {
            yield return new WaitForSeconds(timeSeconds);

            onFinish?.Invoke();
        }

        public static Coroutine CoroutineWaitRealtime(this MonoBehaviour mono, float timeSeconds, Action onFinish)
        {
            return mono.StartCoroutine(CoroutineWaitRealtime(timeSeconds, onFinish));
        }

        public static IEnumerator CoroutineWaitRealtime(float timeSeconds, Action onFinish)
        {
            yield return new WaitForSecondsRealtime(timeSeconds);

            onFinish?.Invoke();
        }

        public static Coroutine CoroutineWaitFrames(this MonoBehaviour mono, int frameCount, Action onFinish)
        {
            return mono.StartCoroutine(CoroutineWaitFrames(frameCount, onFinish));
        }

        public static IEnumerator CoroutineWaitFrames(int frameCount, Action onFinish)
        {
            while (frameCount > 0)
            {
                --frameCount;
                yield return null;
            }

            onFinish?.Invoke();
        }

        public static void CoroutineStop(this MonoBehaviour mono, Coroutine coroutine)
        {
            if (coroutine == null) return;

            mono.StopCoroutine(coroutine);
        }

        public static Coroutine RestartCoroutine
            (this MonoBehaviour mono, Coroutine pointer, IEnumerator function)
        {
            if (pointer != null)
            {
                mono.StopCoroutine(pointer);
            }

            return mono.StartCoroutine(function);
        }

        public static Coroutine CoroutineRepeatInterval(
            this MonoBehaviour mono, 
            Action<int> onUpdate, 
            Action onFinish, 
            float interval, 
            int repeatCount)
        {
            return mono.StartCoroutine(
                CoroutineRepeatInterval(onUpdate, onFinish, interval, repeatCount));
        }

        public static IEnumerator CoroutineRepeatInterval(
            Action<int> onUpdate, Action onFinish, float interval, int repeatCount)
        {
            while (repeatCount >= 0)
            {
                onUpdate?.Invoke(repeatCount);
                --repeatCount;

                yield return new WaitForSecondsRealtime(interval);
            }

            onFinish?.Invoke();
        }
    }
}