using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ouroboros.Common.Utils
{
    public static class EditorUpdater
    {
        private static Action<float> funcs;
        private static double lastUpdateTime;
        private static bool isUpdating;

        private struct Callback
        {
            public GameObject gameObject;
            public Func<float, bool> updateFunc;
        }

        private static readonly List<Callback> callbacks = new List<Callback>();

        public static void StopUpdating(Func<float, bool> update, GameObject owner)
        {
            for (int i = callbacks.Count - 1; i >= 0; --i)
            {
                if (callbacks[i].updateFunc == update)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        public static void StartUpdating(Func<float, bool> update, GameObject owner)
        {
            foreach (var callback in callbacks){
                if (callback.updateFunc == update)
                {
                    return;
                }
            }

            callbacks.Add(new Callback{
                gameObject = owner,
                updateFunc = update
            });

            // funcs -= updateFunc;
            // funcs += updateFunc;

            if (!isUpdating)
            {
                isUpdating = true;
                lastUpdateTime = EditorApplication.timeSinceStartup;
                EditorApplication.update -= UpdateEditor;
                EditorApplication.update += UpdateEditor;
            }
        }

        private static void UpdateEditor()
        {
            var dt = (float)(EditorApplication.timeSinceStartup - lastUpdateTime);
            lastUpdateTime = EditorApplication.timeSinceStartup;

            for (int i = callbacks.Count - 1; i >= 0; --i)
            {
                if (callbacks[i].updateFunc.Invoke(dt))
                {
                    callbacks.RemoveAt(i);
                }    
            }

            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}