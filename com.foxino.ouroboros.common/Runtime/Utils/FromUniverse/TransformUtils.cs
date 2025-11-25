using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    public static void TraverseChildrenDepthFirst(Transform transform, Func<Transform, bool> func)
    {
        var stack = new Stack<Transform>();
        stack.Push(transform);
        Transform current;

        while (stack.Count > 0)
        {
            current = stack.Pop();
            var result = func(current);
            if (!result)
            {
                continue;
            }

            for (int i = 0; i < current.childCount; ++i)
            {
                stack.Push(current.GetChild(i));
            }
        }
    }
}
