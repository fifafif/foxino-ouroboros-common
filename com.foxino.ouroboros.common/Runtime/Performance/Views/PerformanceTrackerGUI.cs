using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class PerformanceTrackerGUI : PerformanceTrackerView
    {
        private string text;

        protected override void OnTrackerUpdate(string text)
        {
            this.text = text;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(5, 5, Screen.width * 0.33f, 20), text);
        }
    }
}