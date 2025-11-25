using TMPro;
using UnityEngine;

namespace Ouroboros.Common.Utils.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProAutoResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        
        private TextMeshProUGUI text;
        private RectTransform textRectTransform;
        private string lastContent;

        private void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            textRectTransform = GetComponent<RectTransform>();
            text = GetComponent<TextMeshProUGUI>();
        }


        private void Update()
        {
            var content = text.text;
            if (content == lastContent) return;

            var size = text.GetPreferredValues();
            var height = Mathf.Ceil(size.x / rectTransform.sizeDelta.x) * (size.y + 1);

            var sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = height;
            sizeDelta.y += textRectTransform.offsetMin.y - textRectTransform.offsetMax.y;
            rectTransform.sizeDelta = sizeDelta;

            lastContent = content;

            //Debug.Log($"size {size}, sizeDelta.x {rectTransform.sizeDelta.x}");
        }
    }
}