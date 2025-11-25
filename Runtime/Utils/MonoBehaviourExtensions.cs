using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class MonoBehaviourExtensions
    {
        public static T GetComponentInSiblings<T>(this GameObject gameObject)
        {
            var parent = gameObject.transform.parent;
            if (parent == null)
            {
                return default;
            }

            return parent.GetComponentInChildren<T>();
        }

        public static T GetComponentInSiblings<T>(this Component component)
        {
            var parent = component.transform.parent;
            if (parent == null)
            {
                return default;
            }

            return parent.GetComponentInChildren<T>();
        }

        public static List<T> GetComponentsInSiblings<T>(this Component component)
        {
            var components = new List<T>();

            var parent = component.transform.parent;
            if (parent == null)
            {
                return components;
            }

            foreach (Transform child in parent)
            {
                components.AddRange(child.GetComponents<T>());
            }

            return components;
        }

        public static void InvokeOnComponents<T>(this Component component, Action<T> action)
        {
            var comps = component.GetComponents<T>();
            foreach (var comp in comps)
            {
                action(comp);
            }
        }

        public static T GetComponent<T>(this Component component, Func<T, bool> predicate)
        {
            var comps = component.GetComponents<T>();
            for (int i = 0; i < comps.Length; ++i)
            {
                if (predicate(comps[i]))
                {
                    return comps[i];
                }
            }

            return default;
        }

        public static T GetComponentInChildren<T>(this Component component, Func<T, bool> predicate)
        {
            var comps = component.GetComponentsInChildren<T>();
            for (int i = 0; i < comps.Length; ++i)
            {
                if (predicate(comps[i]))
                {
                    return comps[i];
                }
            }

            return default;
        }
    }
}