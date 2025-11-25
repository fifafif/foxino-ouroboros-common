using System.Collections;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class StatsTrackerMulti : MonoBehaviour, IStatsTrackable
    {
        public virtual string Key { get; set; }

        public StatsTrackerData[] Data { get; protected set; }

        public bool IsTracking { get; protected set; }

        public void ResetStats()
        {
            if (Data == null)
            {
                return;
            }

            foreach (var d in Data)
            {
                d.ResetStats();
            }
        }

        public virtual void StartTracking()
        {
            IsTracking = true;
        }

        public virtual void StopTracking()
        {
            IsTracking = false;
        }
    }
}