using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils.Math
{
    public class StraightLinePoints : MonoBehaviour
    {
        [SerializeField] private bool isClosed;
        [SerializeField] private List<Transform> points = new List<Transform>();

        private List<StraightLine> lines = new List<StraightLine>();

        private void Awake()
        {
            UpdateLine();
        }

        public void UpdateLine()
        {
            lines.Clear();

            for (var i = 0; i < points.Count - 1; ++i)
            {
                lines.Add(new StraightLine
                {
                    P1 = points[i].position,
                    P2 = points[i + 1].position
                });
            }

            if (isClosed
                && points.Count > 2)
            {
                lines.Add(new StraightLine
                {
                    P1 = points[points.Count - 1].position,
                    P2 = points[0].position
                });
            }
        }

        public void Clear()
        {
            points.Clear();
        }

        public void AddPoint(Transform point)
        {
            points.Add(point);
        }

        public Vector3 FindClosestPoint(Vector3 point)
        {
            var minDistance = float.MaxValue;
            var closest = Vector3.zero;

            foreach (var line in lines)
            {
                var d = line.CalculateDistance(point);
                if (d < minDistance)
                {
                    minDistance = d;
                    closest = line.FindClosestOnLine(point);
                }
            }

            return closest;
        }

        private void OnDrawGizmos()
        {
            if (lines == null) return;

            foreach (var line in lines)
            {
                Gizmos.DrawLine(line.P1, line.P2);
            }
        }
    }
}