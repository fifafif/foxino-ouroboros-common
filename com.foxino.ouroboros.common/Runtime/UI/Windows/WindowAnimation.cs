using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(Window))]
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip openClip;
        [SerializeField] private AnimationClip initializedClip;

        private Animation animation;

        private void Awake()
        {
            animation = GetComponent<Animation>();

            var window = GetComponent<Window>();
            window.OnOpenAction += OnWindowOpen;
            window.OnInitializedAction += OnWindowInitialized;
        }

        private void OnWindowInitialized()
        {
            PlayAnim(initializedClip);
        }

        private void OnWindowOpen()
        {
            PlayAnim(openClip);
        }

        private void PlayAnim(AnimationClip clip)
        {
            if (clip == null) return;

            var clipName = clip.name;
            animation.Play(clipName);
            animation[clipName].time = 0f;
            animation.Sample();
        }
    }
}