using System;
using UnityEngine;

#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
#endif

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class MusicClipData
    {
        public AudioClip AudioClip;

#if ADDRESSABLES_ENABLED
        public AssetReferenceT<AudioClip> AddressableReference;
#endif

        public string StreamingAssetsPath;
        public string Name;
        public string Author;
        public string Album;
        public string Description;
        public float BPM;
        public float FirstBeatOffset;
    }
}