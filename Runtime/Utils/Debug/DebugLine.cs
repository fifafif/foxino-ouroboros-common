using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class DebugLine
    {
        public static GameObject Draw(Vector3 start, Vector3 direction, float length = 1f, float lifespan = 1f)
        {
            return Draw(start, start + direction * length, Color.red, lifespan);
        }

        public static GameObject Draw(Vector3 start, Vector3 end, Color color, float lifespan = 1f)
        {
            var debugLine = new GameObject("debugLine");
            var line = debugLine.AddComponent<LineRenderer>();
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.widthMultiplier = 0.1f;
            line.useWorldSpace = true;
            Object.Destroy(debugLine, lifespan);

            return debugLine;
        }
    }
}