using System.Collections;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class StatsTrackerData
    {
        public float Current { get; protected set; }
        public float Lowest { get; protected set; }
        public float Highest { get; protected set; }
        public float Avg { get; protected set; }
        public float Target { get; set; }
        public int SampleCount { get; protected set; }

        public bool IsReady { get { return SampleCount > 0; } }
        public bool IsTracking { get; protected set; }

        private float avgSum = 0;

        public void SetCurrent(float newCurrent)
        {
            ++SampleCount;

            Current = newCurrent;

            avgSum += Current;
            Avg = avgSum / SampleCount;

            if (Current < Lowest)
            {
                Lowest = Current;
            }

            if (Current > Highest)
            {
                Highest = Current;
            }
        }

        public void ResetStats()
        {
            Lowest = int.MaxValue;
            Highest = int.MinValue;
            Avg = 0;
            SampleCount = 0;
            avgSum = 0;
        }

        public void StartTracking()
        {
            IsTracking = true;
        }

        public void StopTracking()
        {
            IsTracking = false;
        }

        public override string ToString()
        {
            return string.Format("CUR:{0} AVG:{1} MIN:{2} MAX:{3}", Current, Avg, Lowest, Highest);
            
        }
    }
}