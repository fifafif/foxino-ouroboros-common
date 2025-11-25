using UnityEngine;
using UnityEngine.XR;

namespace Ouroboros.Common.Performance.Trackers
{
    public class GameFPSTracker : FPSStatsTracker
    {
        public const string KeyName = "FPS";

        public override string Key { get { return KeyName; }}

        void Start()
        {
            Target = Mathf.RoundToInt(XRDevice.refreshRate);
        }
        
        void Update()
        {
            Tick();
        }
    }
}