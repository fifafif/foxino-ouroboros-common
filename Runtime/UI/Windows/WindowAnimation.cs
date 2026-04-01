using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(Window))]
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip openClip;
        [SerializeField] private AnimationClip closeClip;

        private new Animation animation;
        
        private void Awake()
        {
            animation = GetComponent<Animation>();

            var window = GetComponent<Window>();
            window.OnOpenAction += OnWindowOpen;
        }

        private void OnWindowClose()
        {
            PlayAnim(closeClip);
        }

        private void OnWindowOpen()
        {
            PlayAnim(openClip);
        }

        public async UniTask CloseAsync()
        {
            await PlayAnimAsync(closeClip);
        }

        private async UniTask PlayAnimAsync(AnimationClip closeClip)
        {
            if (closeClip == null) return;
            
            PlayAnim(closeClip);
            await UniTask.WaitUntil(() => !animation.isPlaying);
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