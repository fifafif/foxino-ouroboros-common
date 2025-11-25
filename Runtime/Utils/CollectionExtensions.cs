using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count <= 0) return default;

            return list[Random.Range(0, list.Count)];
        }

        public static string Print<T, U>(this Dictionary<T, U> dictionary)
        {
            var sb = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                sb.AppendLine($"{{ {kvp.Key} : {kvp.Value} }}");
            }

            return sb.ToString();
        }

        public static string PrintInline<T, U>(this Dictionary<T, U> dictionary)
        {
            var sb = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                sb.Append($"{{ {kvp.Key} : {kvp.Value} }}, ");
            }

            // Remove ", " from the end.
            if (sb.Length > 0)
            {
                sb.Length -= 2;
            }

            return sb.ToString();
        }

        public static string Print<T>(this HashSet<T> hashSet)
        {
            var sb = new StringBuilder();

            foreach (var kvp in hashSet)
            {
                sb.Append($"{kvp}, ");
            }

            CorrectStringBuilder(sb);

            return sb.ToString();
        }

        public static string Print<T>(this T[] array)
        {
            if (array == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (var kvp in array)
            {
                sb.Append($"{kvp}, ");
            }

            CorrectStringBuilder(sb);

            return sb.ToString();
        }

        public static string Print<T>(this List<T> list)
        {
            var sb = new StringBuilder();

            foreach (var kvp in list)
            {
                sb.Append($"{kvp}, ");
            }

            CorrectStringBuilder(sb);

            return sb.ToString();
        }

        private static void CorrectStringBuilder(StringBuilder sb)
        {
            // Remove ", " from the end.
            if (sb.Length > 0)
            {
                sb.Length -= 2;
            }
        }

        public static string GetStringFromObjectMap(
            this Dictionary<string, object> map, string key, string defaultValue = "")
        {
            if (!map.TryGetValue(key, out var valueObj))
            {
                return defaultValue;
            }

            return valueObj.ToString();
        }
    }
}