using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Ouroboros.Common.Utils
{
    public static class ArrayExtensions
    {
        public static string[] DeepClone(this string[] array)
        {
            var clone = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                clone[i] = array[i];
            }

            return clone;
        }

        public static T Last<T>(this T[] array)
        {
            if (array.Length <= 0)
            {
                return default;
            }

            return array[array.Length - 1];
        }

        public static T Last<T>(this IList<T> list)
        {
            if (list.Count <= 0)
            {
                return default;
            }

            return list[list.Count - 1];
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            if (list.Count <= 0)
            {
                return default;
            }

            return list[Random.Range(0, list.Count)];
        }

        public static bool RemoveBackward<T>(this IList<T> list, T element)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].Equals(element))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                action.Invoke(array[i]);
            }
        }
    }
}
