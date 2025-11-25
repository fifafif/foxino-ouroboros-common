using TMPro;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PerformanceTrackerTMPro : PerformanceTrackerView
    {
        private TextMeshProUGUI label;

        private void Awake()
        {
            label = GetComponent<TextMeshProUGUI>();
        }

        protected override void OnTrackerUpdate(string text)
        {
            label.text = text;
        }
    }
}