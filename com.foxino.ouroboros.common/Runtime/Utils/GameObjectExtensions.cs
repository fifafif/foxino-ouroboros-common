using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class GameObjectExtensions
    {
        public static void SetActiveSafe(this GameObject gameObject, bool isActive)
        {
            if (gameObject == null) return;

            gameObject.SetActive(isActive);
        }

        public static void SetActiveSafe(this Component component, bool isActive)
        {
            if (component == null
                || component.gameObject == null) return;

            component.gameObject.SetActive(isActive);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return transform.gameObject.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static bool GetOrAddComponent<T>(this GameObject gameObject, out T component) 
            where T : Component
        {
            if (gameObject.TryGetComponent(out component))
            {
                return true;
            }

            component = gameObject.AddComponent<T>();

            return false;
        }

        public static bool GetOrAddComponent<T, U>(this GameObject gameObject, out T component)
            where U : Component, T
        {
            if (gameObject.TryGetComponent(out component))
            {
                return true;
            }

            component = gameObject.AddComponent<U>();

            return false;
        }

        public static void DestoyComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                Object.Destroy(component);
            }
        }

        public static void DestoyComponent<T>(this Component parent) where T : Component
        {
            parent.gameObject.DestoyComponent<T>();
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component, int maxDepth = int.MaxValue) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out component))
            {
                return true;
            }

            var parent = gameObject.transform.parent;

            while (parent != null
                   && maxDepth > 0)
            {
                --maxDepth;
                if (parent.TryGetComponent<T>(out component))
                {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }

        public static bool TryGetComponentInParentIgnoreThis<T>(
            this GameObject gameObject, out T component, int maxDepth = int.MaxValue) where T : Component
        {
            var parent = gameObject.transform.parent;

            while (parent != null
                   && maxDepth > 0)
            {
                --maxDepth;
                if (parent.TryGetComponent<T>(out component))
                {
                    return true;
                }

                parent = parent.parent;
            }

            component = default;
            return false;
        }

        public static bool HasComponent<T>(this GameObject gameObject)
        {
            return gameObject.TryGetComponent<T>(out var target);
        }

        public static bool HasComponent<T>(this Component component)
        {
            return component.TryGetComponent<T>(out var target);
        }

        public static void TryEnableComponent<T>(this Component component, bool isEnabled) 
            where T : MonoBehaviour
        {
            if (component.TryGetComponent<T>(out var target))
            {
                target.enabled = isEnabled;
            }
        }

        public static void TryEnableComponent<T>(this GameObject gameObject, bool isEnabled)
            where T : MonoBehaviour
        {
            if (gameObject.TryGetComponent<T>(out var target))
            {
                target.enabled = isEnabled;
            }
        }
    }
}