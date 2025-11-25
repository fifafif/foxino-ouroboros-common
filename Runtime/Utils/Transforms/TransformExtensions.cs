using Ouroboros.Common.Utils.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public static class TransformExtensions
    {
        public static string PrintAllLocal(this Transform transform)
        {
            return $"{transform}, " +
                $"pos={transform.localPosition:F4}, " +
                $"rot={transform.localRotation.eulerAngles:F4}, " +
                $"scale={transform.localScale:F4}";
        }

        public static void MatchPositionAndRotation(this Transform transform, Transform other)
        {
            transform.SetPositionAndRotation(other.position, other.rotation);
        }
        
        public static void MatchPositionAndRotation2D(this Transform transform, Transform other)
        {
            var pos = other.position;
            pos.z = transform.position.z;

            var rot = transform.rotation.eulerAngles;
            rot.z = other.eulerAngles.z;

            transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
        }

        public static void Reset(this Transform transform)
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }
            
        public static void MatchTransform(this Transform transform, Transform other)
        {
            transform.SetPositionAndRotation(other.position, other.rotation);
            transform.localScale = other.localScale;
        }

        public static void LookAwayFrom(this Transform transform, Vector3 targetPosition)
        {
            transform.LookAt(2 * transform.position - targetPosition);
        }

        public static void LookAwayFrom(this Transform transform, Transform target)
        {
            transform.LookAwayFrom(target.position);
        }

        public static void LookAt2D(this Transform transform, Vector3 targetPosition)
        {
            transform.rotation = MathUtils.LookRotation2D(targetPosition);
        }

        public static T FindClosest<T>(this IList<T> list, Transform target) where T : Component
        {
            var closestDist = float.MaxValue;
            T closestObj = null;
            var pos = target.position;

            foreach (var obj in list)
            {
                var dist = obj.transform.position.DistanceSquared(pos);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestObj = obj;
                }
            }

            return closestObj;
        }

        public static float Distance(this Transform transform, Transform other)
        {
            return Vector3.Distance(transform.position, other.position);
        }

        public static float Distance2D(this Transform transform, Transform other)
        {
            return Vector2.Distance(transform.position, other.position);
        }

        public static float Distance(this Component component, Transform other)
        {
            return Vector3.Distance(component.transform.position, other.position);
        }

        public static float Distance2D(this Component component, Transform other)
        {
            return Vector2.Distance(component.transform.position, other.position);
        }

        public static float Distance(this Component component, Component other)
        {
            return Vector3.Distance(component.transform.position, other.transform.position);
        }

        public static float Distance(this Transform transform, Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        public static float Distance2D(this Transform transform, Vector2 position)
        {
            return Vector2.Distance(transform.position, position);
        }

        public static bool IsInRange2D(this Transform transform, Vector2 position, float range)
        {
            var dist = ((Vector2)transform.position - position).sqrMagnitude;
            return dist < range * range;
        }

        public static bool IsInRange2D(this Transform transform, Vector2 position, float maxRange, float minRange)
        {
            var dist = ((Vector2)transform.position - position).sqrMagnitude;
            return dist <= maxRange * maxRange
                && dist >= minRange * minRange;
        }

        public static Vector3 GetAxisDirection(this Transform transform, UnityAxis axis)
        {
            switch (axis)
            {
                case UnityAxis.Forward:
                    return transform.forward;

                case UnityAxis.Right:
                    return transform.right;

                case UnityAxis.Up:
                    return transform.up;

                case UnityAxis.Back:
                    return -transform.forward;

                case UnityAxis.Left:
                    return -transform.right;
                
                case UnityAxis.Down:
                    return -transform.up;

                default:
                    return Vector3.zero;
            }
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyImmediateAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyImmediateAllChildrenExcept(this Transform transform, GameObject keelAlive)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                var child = transform.GetChild(i);
                if (child.gameObject == keelAlive) continue;

                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyImmediateAllChildren(
            this Transform transform, Func<Transform, bool> isDestroyingPredicate)
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                if (!isDestroyingPredicate(transform.GetChild(i))) continue;

                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static Transform[] GetChildren(this Transform transform)
        {
            var children = new Transform[transform.childCount];
            for (var i = 0; i < children.Length; ++i)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }

        public static void MatchOther(this Transform transform, Transform other)
        {
            transform.SetPositionAndRotation(other.position, other.rotation);
        }

        public static float CalculateAxeUpAngle(this Transform transform, Vector3 axe)
        {
            var dir = transform.up;
            dir.z = 0;

            var angle = Vector3.Angle(axe, dir);

            if (dir.x < 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }
        
        public static void SetParentAndReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent, false);
            transform.Reset();
        }
    }
}
