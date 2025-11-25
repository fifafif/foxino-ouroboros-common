using System.Collections.Generic;
using System.Diagnostics;

namespace Ouroboros.Common.Utils
{
    public static class PerformanceWatch
    {
        private static readonly Dictionary<string, Stopwatch> watchMap = new();

        public static void Start(string name)
        {
            var watch = new Stopwatch();
            watchMap[name] = watch;
            watch.Start();
        }

        public static long Stop(string name)
        {
            if (!watchMap.TryGetValue(name, out var watch))
            {
                return 0;
            }

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        public static double StopSeconds(string name)
        {
            return Stop(name) * 0.001;
        }
    }
}