using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [CreateAssetMenu(fileName = "dat_audio.asset", menuName = "Ouroboros/Audio Database")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeReference]
        public List<AudioClipBase> Clips = new List<AudioClipBase>();

        public void Init()
        {
            foreach (var clip in Clips)
            {
                clip.Init();
            }
        }
    }
}