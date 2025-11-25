using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Utils
{
    public static class TextExtensions
    {
        public static void SetTextSafe(this TextMesh textMesh, string content)
        {
            if (textMesh == null) return;

            textMesh.text = content;
        }

        public static void SetTextSafe(this Text text, string content)
        {
            if (text == null) return;

            text.text = content;
        }
    }
}