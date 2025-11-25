using Ouroboros.Common.Performance.Trackers;
using Ouroboros.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class PerformanceTracker : MonoBehaviour
    {
        public enum Types
        {
            GameFPS         = 0,
            CPULoad         = 1,
            CPUCoresLoad    = 2,
        }

        public static PerformanceTracker Instance { get; private set; }

        public bool IsTracking { get; private set; }
        public bool IsVisible { get; private set; }
        public List<IStatsTrackable> Trackers { get; private set; }
        public Action<IStatsTrackable> OnTrackerRegistered { get; set; }
        public Action<bool> OnVisibilityChange { get; set; }

        [SerializeField] private KeyCode startStopKey = KeyCode.T;
        [SerializeField] private KeyCode startStopKey2 = KeyCode.LeftControl;
        [SerializeField] private KeyCode startStopKey3 = KeyCode.LeftShift;

        [SerializeField] private bool addTrackersOnAwake = true;
        [SerializeField] private bool startTrackingOnStart = true;
        [SerializeField] private float startTrackingDelay = 2f;

        private Dictionary<string, IStatsTrackable> trackersMap;

        private void Awake()
        {
            if (Instance != null
                && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
            //DontDestroyOnLoad(gameObject);

            trackersMap = new Dictionary<string, IStatsTrackable>();
            Trackers = new List<IStatsTrackable>();

            var existingTrackers = GetComponentsInChildren<IStatsTrackable>();
            foreach (var tracker in  existingTrackers)
            {
                AddTracker(tracker.Key, tracker);
            }

            if (addTrackersOnAwake)
            {
                RegisterTrackers();
            }

            SetVisible(true);
        }

        private void OnEnable()
        {
            if (startTrackingOnStart && !IsTracking)
            {
                StartCoroutine(StartTrackingRoutine());
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
#if !OUROBOROS_PROD

            if (Input.GetKeyDown(startStopKey)
                && (startStopKey2 == KeyCode.None || Input.GetKey(startStopKey2))
                && (startStopKey3 == KeyCode.None || Input.GetKey(startStopKey3)))
            {
                if (IsTracking)
                {
                    StopTracking();
                }
                else
                {
                    StartTracking();
                }
            }

#endif
        }

        private IEnumerator StartTrackingRoutine()
        {
            yield return new WaitForSeconds(startTrackingDelay);

            StartTracking();
        }

        private void RegisterTrackers()
        {
            AddTracker(GameFPSTracker.KeyName, this.GetOrAddComponent<GameFPSTracker>());
            //AddTracker(CPUCoresTracker.KeyName, this.GetOrAddComponent<CPUCoresTracker>());
            //AddTracker(CPUTracker.KeyName, this.GetOrAddComponent<CPUTracker>());
        }

        public void AddTracker(string key, IStatsTrackable tracker)
        {
            trackersMap.Add(key, tracker);
            Trackers.Add(tracker);

            if (OnTrackerRegistered != null)
            {
                OnTrackerRegistered(tracker);
            }
        }

        public IStatsTrackable GetTracker(string key)
        {
            IStatsTrackable tracker;
            trackersMap.TryGetValue(key, out tracker);

            return tracker;
        }

        public IStatsTrackable GetTracker(Types type)
        {
            string key = GetKeyNameFromEnum(type);

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return GetTracker(key);
        }

        public static string GetKeyNameFromEnum(Types type)
        {
            switch (type)
            {
                case Types.GameFPS:
                    return GameFPSTracker.KeyName;

                case Types.CPULoad:
                    return CPUTracker.KeyName;

                case Types.CPUCoresLoad:
                    return CPUCoresTracker.KeyName;
            }

            return null;
        }

        public void StartTracking()
        {
            IsTracking = true;

            foreach (var tracker in Trackers)
            {
                tracker.ResetStats();
                tracker.StartTracking();
            }
        }

        public void StopTracking()
        {
            IsTracking = false;

            foreach (var tracker in Trackers)
            {
                tracker.StopTracking();
            }
        }

        public void SetVisible(bool isVisible)
        {
            IsVisible = isVisible;
            OnVisibilityChange?.Invoke(isVisible);
        }
    }
}