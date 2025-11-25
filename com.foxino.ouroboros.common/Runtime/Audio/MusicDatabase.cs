using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [CreateAssetMenu(fileName = "dat_music.asset", menuName = "Ouroboros/Music Database")]
    public class MusicDatabase : ScriptableObject
    {
        public List<MusicClipData> Clips = new List<MusicClipData>();
    }
}