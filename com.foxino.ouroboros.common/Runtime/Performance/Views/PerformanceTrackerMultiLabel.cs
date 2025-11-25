using System.Text;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    /// <summary>
    /// Label for Peformance Stats as indirect reference to StatsTracker component.
    /// This object will grab reference to StatsTracker at runtime.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class PerformanceTrackerMultiLabel : MonoBehaviour
    {
        public PerformanceTracker.Types type;

        [Tooltip("{0} - Name, {1} - Current, {2} - Min, {3} - Max, {4} - Avg, {5} - Target, {6} - ID")]
        public string format = "{0}:[{6}] {1}({5}) [{2}/{3}/{4}]";
        public float updateInterval = 0.1f;

        private StatsTrackerMulti tracker;
        private float nextUpdateTime;
        private UnityEngine.UI.Text label;
        private StringBuilder sb = new StringBuilder();

        void Awake()
        {
            label = GetComponent<UnityEngine.UI.Text>();
            nextUpdateTime = Time.unscaledTime + nextUpdateTime;
        }

        void Start()
        {
            if (tracker == null)
            {
                GetTracker();
            }
        }

        void OnEnable()
        {
            if (tracker == null)
            {
                GetTracker();
            }
        }

        void OnDestroy()
        {
            if (PerformanceTracker.Instance != null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
            }
        }

        private void GetTracker()
        {
            if (PerformanceTracker.Instance == null)
            {
                return;
            }

            tracker = PerformanceTracker.Instance.GetTracker(type) as StatsTrackerMulti;

            if (tracker == null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
                PerformanceTracker.Instance.OnTrackerRegistered += TrackerRegistered;
            }
        }

        private void TrackerRegistered(IStatsTrackable registeredTracker)
        {
            PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;

            if (registeredTracker.Key == PerformanceTracker.GetKeyNameFromEnum(type))
            {
                tracker = registeredTracker as StatsTrackerMulti;
            }
        }

        void Update()
        {
            if (Time.unscaledTime >= nextUpdateTime)
            {
                nextUpdateTime += updateInterval;

                UpdateLabel();
            }
        }

        private void UpdateLabel()
        {
            if (tracker == null || tracker.Data == null)
            {
                label.text = "";

                return;
            }

            sb.Length = 0;

            int i = 1;
            
            foreach (var data in tracker.Data)
            {
                if (data.IsReady)
                {
                    sb.AppendFormat(format, tracker.Key, data.Current, data.Lowest, data.Highest, data.Avg, data.Target, i);
                    
                }
                else
                {
                    sb.AppendFormat(format, tracker.Key, "-", "-", "-", "-", data.Target, i);
                }

                sb.AppendLine();

                ++i;
            }

            label.text = sb.ToString();
        }
    }
}