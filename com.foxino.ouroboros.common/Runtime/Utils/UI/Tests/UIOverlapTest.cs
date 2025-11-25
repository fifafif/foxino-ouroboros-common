using UnityEngine;

namespace Ouroboros.Common.Utils.UI.Tests
{
    public class UIOverlapTest : MonoBehaviour
    {
        [SerializeField] private RectTransform rect1;
        [SerializeField] private RectTransform rect2;

        private void Update()
        {
            if (rect1.RectOverlaps(rect2))
            {
                Debug.Log("Overlaps");
            }
            else
            {
                Debug.Log("No overlaps");
            }
        }
    }
}