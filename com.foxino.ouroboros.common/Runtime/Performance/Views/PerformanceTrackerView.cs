using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class PerformanceTrackerView : MonoBehaviour
    {
        public PerformanceTracker.Types type;
        public bool isSimpleView;

        [Tooltip("{0} - Name, {1} - Current, {2} - Min, {3} - Max, {4} - Avg, {5} - Target")]
        public string format = "{0}: {1:0.0}({5:0.0}) [{2:0.0}/{3:0.0}/{4:0.0}]";
        public float updateInterval = 0.1f;

        private StatsTracker tracker;
        private float nextUpdateTime;
        
        private void Awake()
        {
            nextUpdateTime = Time.unscaledTime + nextUpdateTime;
        }

        private void Start()
        {
            if (tracker == null)
            {
                Init();
            }
        }

        private void OnEnable()
        {
            if (tracker == null)
            {
                Init();
            }
        }

        private void OnDestroy()
        {
            if (PerformanceTracker.Instance != null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
                PerformanceTracker.Instance.OnVisibilityChange -= OnVisibilityChange;
            }
        }

        private void Init()
        {
            if (PerformanceTracker.Instance == null)
            {
                return;
            }

            tracker = PerformanceTracker.Instance.GetTracker(type) as StatsTracker;

            if (tracker == null)
            {
                PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;
                PerformanceTracker.Instance.OnTrackerRegistered += TrackerRegistered;
            }

            PerformanceTracker.Instance.OnVisibilityChange += OnVisibilityChange;

            if (!PerformanceTracker.Instance.IsVisible)
            {
                OnVisibilityChange(false);
            }
        }

        private void OnVisibilityChange(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void TrackerRegistered(IStatsTrackable registeredTracker)
        {
            PerformanceTracker.Instance.OnTrackerRegistered -= TrackerRegistered;

            if (registeredTracker.Key == PerformanceTracker.GetKeyNameFromEnum(type))
            {
                tracker = registeredTracker as StatsTracker;
            }
        }

        private void Update()
        {
            if (Time.unscaledTime >= nextUpdateTime)
            {
                nextUpdateTime += updateInterval;

                UpdateLabel();
            }
        }

        protected virtual void OnTrackerUpdate(string text)
        {
            if (isSimpleView)
            {
                OnTrackerUpdate(string.Format(
                    format,
                    tracker.Key,
                    tracker.Current,
                    tracker.Lowest,
                    tracker.Highest,
                    tracker.Avg,
                    tracker.Target));
            }
        }

        private void UpdateLabel()
        {
            if (tracker == null) return;

            if (tracker.IsReady)
            {
                if (isSimpleView)
                {
                    OnTrackerUpdate(tracker.Current.ToString());
                }
                else
                {
                    OnTrackerUpdate(string.Format(
                        format,
                        tracker.Key,
                        tracker.Current,
                        tracker.Lowest,
                        tracker.Highest,
                        tracker.Avg,
                        tracker.Target));
                }
            }
            else
            {
                if (isSimpleView)
                {
                    OnTrackerUpdate("-");
                }
                else
                {
                    OnTrackerUpdate(string.Format(format, tracker.Key, "-", "-", "-", "-", tracker.Target));
                }
            }
        }
    }
}