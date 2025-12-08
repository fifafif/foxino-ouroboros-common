using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ouroboros.Common.Utils.UI
{
    public static class UIExtensions
    {
        public static void RemoveAndAddOnClickAction(this Button button, UnityAction onClickAction)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveListener(onClickAction);
            button.onClick.AddListener(onClickAction);
        }

        public static bool RectOverlaps(this RectTransform rectTrans1, RectTransform rectTrans2)
        {
            // Rect rect1 = new Rect(rectTrans1.position.x, rectTrans1.position.y, rectTrans1.rect.width, rectTrans1.rect.height);
            // Rect rect2 = new Rect(rectTrans2.position.x, rectTrans2.position.y, rectTrans2.rect.width, rectTrans2.rect.height);
            Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
            Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

            return rect1.Overlaps(rect2);
        }

        public static bool RectOverlaps(
            this RectTransform rectTrans1, RectTransform rectTrans2, Vector2 rectPos1, Vector2 rectPos2)
        {
            Rect rect1 = new Rect(rectPos1.x, rectPos1.y, rectTrans1.rect.width, rectTrans1.rect.height);
            Rect rect2 = new Rect(rectPos2.x, rectPos2.y, rectTrans2.rect.width, rectTrans2.rect.height);

            return rect1.Overlaps(rect2);
        }

        public static void ScrollToTopSafe(this ScrollRect scrollRect)
        {
            if (scrollRect == null) return;

            scrollRect.ScrollToTop();
        }

        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}