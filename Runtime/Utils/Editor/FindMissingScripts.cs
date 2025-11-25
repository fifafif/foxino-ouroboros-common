using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class FindMissingScripts
    {
        [MenuItem("Ouroboros/Utils/Find Missing Scripts")]
        public static void FindInSelected()
        {
            var parent = Selection.activeTransform;
            var stack = new Stack<Transform>();
            stack.Push(parent);

            while (stack.Count > 0)
            {
                parent = stack.Pop();

                var comps = parent.GetComponents<Component>();
                foreach (var c in comps)
                {
                    if (c == null)
                    {
                        Debug.LogError("Missing component!", parent);
                    }
                }

                foreach (Transform child in parent)
                {
                    stack.Push(child);   
                }
            }
        }
    }
}