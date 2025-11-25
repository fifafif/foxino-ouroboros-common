using System;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class AudioClipData
    {
        public float FinalVolume => Volume * VolumeFactor;
        public float VolumeFactor { get; set; } = 1f;
        public long IdHash { get; set; }
        
        public AudioClip AudioClip;
        public float Volume = 1f;
        public float Pitch = 1f;

        public void GenerateIdHash(string id)
        {
            IdHash = Animator.StringToHash(id);
        }
    }
}