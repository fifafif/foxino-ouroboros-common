using System;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public abstract class AudioClipBase
    {
        public long IdHash { get; set; }
        public double LastPlayAtTime { get; set; }
        
        public string Id;
        public float SpatialFactor = 0f;
        
        public abstract AudioClipData GetAudioData();
        public abstract AudioClipData GetAudioData(float normalizedArrayIndex);

        public void Init()
        {
            IdHash = Animator.StringToHash(Id);
            LastPlayAtTime = 0;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }
    }
}