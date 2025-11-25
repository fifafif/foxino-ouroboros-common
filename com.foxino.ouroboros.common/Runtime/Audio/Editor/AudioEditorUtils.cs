using Ouroboros.Common.Utils;
using System.Linq;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    public static class AudioEditorUtils
    {
        public static AudioClip FindAudioClip(string audioId)
        {
            var databases = AssetFinder.FindAssetsByType<AudioDatabase>();

            var audio = databases
                .Select(d => d.Clips.FirstOrDefault(c => c.Id == audioId))
                .FirstOrDefault(c => c != null);

            if (audio == null)
            {
                Debug.LogError($"No Audio found in databases! Id={audioId}");
                return null;
            }

            var data = audio.GetAudioData();
            if (data == null)
            {
                Debug.LogError($"No AudioData found! Id={audioId}");
                return null;
            }

            var clip = data.AudioClip;
            if (clip == null)
            {
                Debug.LogError($"No AudioClip set for AudioData! Id={audioId}");
                return null;
            }

            return clip;
        }

        public static void PlayClip(string audioId, bool isLooping = false)
        {
            if (string.IsNullOrEmpty(audioId)) return;

            var clip = FindAudioClip(audioId);
            if (clip == null) return;

            AudioEditorPlayer.StopClip(clip);
            AudioEditorPlayer.PlayClip(clip, isLooping);
        }
    }
}