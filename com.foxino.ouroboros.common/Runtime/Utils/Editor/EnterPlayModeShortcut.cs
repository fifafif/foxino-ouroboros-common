using UnityEditor;

namespace Ouroboros.Common.Utils
{
    public static class EnterPlayModeShortcut
    {
        [MenuItem("Ouroboros/Play %g", priority=1000)]
        public static void EnterPlayMode()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
        }
    }
}