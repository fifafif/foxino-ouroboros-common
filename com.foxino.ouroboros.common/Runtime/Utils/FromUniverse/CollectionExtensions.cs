using System.Collections.Generic;
using System.Text;

public static class CollectionExtensions
{
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

        // Remove ", " from the end.
        if (sb.Length > 0)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }
}