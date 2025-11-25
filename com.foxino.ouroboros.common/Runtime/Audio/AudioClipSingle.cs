using System;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class AudioClipSingle : AudioClipBase
    {
        public AudioClipData AudioData = new AudioClipData();

        public override AudioClipData GetAudioData()
        {
            return AudioData;
        }

        public override AudioClipData GetAudioData(float normalizedArrayIndex)
        {
            return AudioData;
        }
    }
}