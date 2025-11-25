using System;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class MusicClipData
    {
        public AudioClip AudioClip;
        public string Name;
        public string Author;
        public string Album;
        public string Description;
        public float BPM;
        public float FirstBeatOffset;
    }
}