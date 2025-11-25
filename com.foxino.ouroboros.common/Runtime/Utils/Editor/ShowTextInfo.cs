using TMPro;
using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class ShowTextInfo
    {
        [MenuItem("Ouroboros/Utils/Show Text Info")]
        public static void FindInSelected()
        {
            var parent = Selection.activeGameObject;
            if (parent == null)
            {
                Debug.LogError($"Cannot shot text info. Please select a GameObject containing TextMeshProUGUI!");
            }
             
            var tmpro = parent.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpro == null)
            {
                Debug.LogError($"Cannot shot text info. Please select a GameObject containing TextMeshProUGUI!");
                return;
            }

            var textInfo = tmpro.textInfo;
            var charPerLine = textInfo.lineInfo[0].characterCount;
            Debug.Log($"First line character count: {charPerLine}");
            Debug.Log($"Character count: {textInfo.characterCount}");
            Debug.Log($"Word count: {textInfo.wordCount}");
            Debug.Log($"Line count: {textInfo.lineCount}");
        }
    }
}