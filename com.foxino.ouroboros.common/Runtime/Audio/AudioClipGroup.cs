using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class AudioClipGroup : AudioClipBase
    {
        [Serializable]
        public class Clip
        {
            public AudioClipData AudioData = new AudioClipData();
            public float Weight = 1f;
        }

        public float Volume = 1f;
        public List<Clip> Clips = new List<Clip>();

        private float totalWeight;
        private bool isInit;
        
        protected override void OnInit()
        {
            totalWeight = 0f;
            
            foreach (var clip in Clips)
            {
                totalWeight += clip.Weight;
                clip.AudioData.VolumeFactor = Volume;
            }

            isInit = true;
        }
        
        public override AudioClipData GetAudioData()
        {
#if UNITY_EDITOR

            if (!Application.isPlaying
                && !isInit)
            {
                Init();
            }
            
#endif
            
            if (Clips.Count <= 0)
            {
                return null;
            }

            var rnd = UnityEngine.Random.Range(0f, totalWeight);
            var weight = 0f;

            foreach (var data in Clips)
            {
                weight += data.Weight;
                if (weight >= rnd)
                {
                    return data.AudioData;
                }
            }

            return Clips[Clips.Count - 1].AudioData;
        }

        public override AudioClipData GetAudioData(float normalizedArrayIndex)
        {
            return Clips[Mathf.RoundToInt((Clips.Count - 1) * normalizedArrayIndex)].AudioData;
        }
    }
}