using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Ouroboros.Common.Utils
{
    public static class AnimationExtensions
    {
        public static void WaitForAnimation(this Animation animation, MonoBehaviour mono, Action onFinish = null)
        {
            mono.StartCoroutine(animation.WaitForAnimation(onFinish));
        }
        
        public static IEnumerator WaitForAnimationRoutine(this Animation animation, MonoBehaviour mono, Action onFinish = null)
        {
            yield return mono.StartCoroutine(animation.WaitForAnimation(onFinish));
        }
        
        public static IEnumerator PlayAndWaitForAnimationRoutine(this Animation animation, AnimationClip clip, MonoBehaviour mono, Action onFinish = null)
        {
            animation.Play(clip.name);
            yield return mono.StartCoroutine(animation.WaitForAnimation(onFinish));
        }

        public static IEnumerator WaitForAnimation(this Animation animation, Action onFinish = null)
        {
            do
            {
                yield return null;
            }
            while (animation.isPlaying);

            onFinish?.Invoke();
        }

        public static IEnumerator WaitForFinish(this PlayableDirector director, Action onFinish = null)
        {
            do
            {
                yield return null;
            }
            while (director.state == PlayState.Playing);

            onFinish?.Invoke();
        }
    }
}