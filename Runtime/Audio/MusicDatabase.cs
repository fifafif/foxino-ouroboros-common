using System;
using System.Collections.Generic;
using UnityEngine;

#if ADDRESSABLES_ENABLED
using Cysharp.Threading.Tasks;
#endif

namespace Ouroboros.Common.Audio
{
    [CreateAssetMenu(fileName = "dat_music.asset", menuName = "Ouroboros/Music Database")]
    public class MusicDatabase : ScriptableObject
    {
        public List<MusicClipData> Clips = new List<MusicClipData>();

#if ADDRESSABLES_ENABLED
        public async UniTask PreloadAllAsync(IProgress<float> progress = null)
        {
            int total = Clips.Count;
            int loaded = 0;

            var tasks = new List<UniTask>();

            foreach (var clip in Clips)
            {
                if (clip.AddressableReference != null && clip.AddressableReference.RuntimeKeyIsValid())
                {
                    tasks.Add(PreloadClipAsync(clip, () =>
                    {
                        loaded++;
                        progress?.Report((float)loaded / total);
                    }));
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask PreloadClipAsync(MusicClipData clip, Action onComplete)
        {
            if (clip.AudioClip != null)
            {
                onComplete?.Invoke();
                return;
            }

            try
            {
                var handle = clip.AddressableReference.LoadAssetAsync<AudioClip>();
                var audioClip = await handle.ToUniTask();

                if (audioClip != null)
                    clip.AudioClip = audioClip;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Preload failed for {clip.Name}: {ex.Message}");
            }

            onComplete?.Invoke();
        }
#endif
    }
}