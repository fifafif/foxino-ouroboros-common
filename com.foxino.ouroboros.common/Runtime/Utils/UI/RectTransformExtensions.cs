using UnityEngine;

namespace Ouroboros.Common.Utils.UI
{
    public static class RectTransformExtension
    {
        public static Canvas GetCanvas(this RectTransform rt)
        {
            return rt.gameObject.GetComponentInParent<Canvas>();
        }

        public static float GetWidth(this RectTransform rt)
        {
            var w =
                (rt.anchorMax.x - rt.anchorMin.x) * Screen.width
                + rt.sizeDelta.x * rt.GetCanvas().scaleFactor;
            return w;
        }

        public static float GetHeight(this RectTransform rt)
        {
            var h =
                (rt.anchorMax.y - rt.anchorMin.y) * Screen.height
                + rt.sizeDelta.y * rt.GetCanvas().scaleFactor;
            return h;
        }
    }
}
