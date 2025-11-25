using System.Collections;
using UnityEngine;

namespace Ouroboros.Common.Performance
{
    public class StatsTracker : MonoBehaviour, IStatsTrackable
    {
        public virtual string Key { get; set; }

        public StatsTrackerData Data { get; protected set; }

        public float Current { get { return Data.Current; } }
        public float Lowest { get { return Data.Lowest; } }
        public float Highest { get { return Data.Highest; } }
        public float Avg { get { return Data.Avg; } }
        public float Target
        {
            get { return Data.Target; }
            set { Data.Target = value; }
        }

        public int SampleCount { get { return Data.SampleCount; } }

        public bool IsReady { get { return SampleCount > 0; } }
        public bool IsTracking { get; protected set; }

        private Coroutine printCoroutine;

        protected virtual void Awake()
        {
            Data = new StatsTrackerData();
        }

        protected void SetCurrent(float newCurrent)
        {
            Data.SetCurrent(newCurrent);
        }

        public void ResetStats()
        {
            Data.ResetStats();
        }

        public virtual void StartTracking()
        {
            IsTracking = true;
        }

        public virtual void StopTracking()
        {
            IsTracking = false;
        }

        public void StartPrinting()
        {
            if (printCoroutine != null)
            {
                return;
            }

            printCoroutine = StartCoroutine(Print());
        }

        public void StopPrinting()
        {
            StopCoroutine(printCoroutine);

            printCoroutine = null;
        }

        private IEnumerator Print()
        {
            while (true)
            {
                Debug.Log(string.Format("KEY:{4} CUR:{0} AVG:{1} MIN:{2} MAX:{3}", Current, Avg, Lowest, Highest, Key));

                yield return new WaitForSeconds(1);
            }
        }
    }
}