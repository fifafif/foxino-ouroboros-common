using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Performance
{
    /// <summary>
    /// Label for Peformance Stats as indirect reference to StatsTracker component.
    /// This object will grab reference to StatsTracker at runtime.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class PerformanceTrackerLabel : PerformanceTrackerView
    {
        private Text label;

        private void Awake()
        {
            label = GetComponent<Text>();
        }

        protected override void OnTrackerUpdate(string text)
        {
            label.text = text;
        }
    }
}