using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Events;

namespace Ouroboros.Common.Utils
{
    public static class FindReferences
    {
        [MenuItem("Ouroboros/Utils/Find References to Selected %#&f")]
        public static void Find()
        {
            var selected = Selection.activeGameObject;
            Find(selected);
        }

        [MenuItem("GameObject/Ouroboros/Utils/Find References in Scene", false, 0)]
        public static void FindInScene()
        {
            Find();
        }

        public static void Find(GameObject target)
        {
            bool foundAny = false;

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                foundAny |= FindInScene(scene, target);
            }

            // Find GameObject roots which are marked as DontDestroyOnLoad.
            if (Application.isPlaying)
            {
                var temp = new GameObject();
                Object.DontDestroyOnLoad(temp);
                var dontDestroyOnLoadScene = temp.scene;
                Object.DestroyImmediate(temp);

                foundAny |= FindInScene(dontDestroyOnLoadScene, target);
            }

            if (!foundAny)
            {
                Debug.LogWarning("No references found to " + target);
            }
        }

        private static bool FindInScene(Scene scene, GameObject target)
        {
            if (!scene.IsValid())
            {
                return false;
            }

            List<GameObject> rootObjects = new List<GameObject>(scene.rootCount + 1);

            scene.GetRootGameObjects(rootObjects);

            var refGos = new List<GameObject>();

            for (int i = 0; i < rootObjects.Count; ++i)
            {
                GameObject gameObject = rootObjects[i];
                if (gameObject != null)
                {
                    var comps = gameObject.GetComponentsInChildren<Component>();

                    for (int o = comps.Length - 1; o >= 0; --o)
                    {
                        //                    Debug.Log("##### COMP: " + comps[o]);

                        FieldInfo[] fields = comps[o].GetType().GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Static |
                            BindingFlags.Instance |
                            BindingFlags.DeclaredOnly);

                        foreach (FieldInfo f in fields)
                        {
                            var value = f.GetValue(comps[o]);
                            if (value == null)
                            {
                                continue;
                            }

                            var valueType = value.GetType();

                            //                        Debug.Log(comps[o] + ":" + f.Name + " = " + value + " type=" + valueType);

                            if (valueType == typeof(GameObject))
                            {
                                if ((value as GameObject) == target)
                                {
                                    Found(target, comps[o], refGos);
                                }
                            }
                            else if (value is Transform)
                            {
                                var valueTrans = value as Transform;
                                if (valueTrans == null)
                                {
                                    continue;
                                }

                                if (valueTrans.gameObject == target)
                                {
                                    Found(target, comps[o], refGos);
                                }
                                else if (valueTrans == target.transform)
                                {
                                    Found(target, comps[o], refGos);
                                }
                            }
                            else if (value is UnityEventBase)
                            {
                                var unityEvent = value as UnityEventBase;
                                for (int u = unityEvent.GetPersistentEventCount() - 1; u >= 0; --u)
                                {
                                    if (unityEvent.GetPersistentTarget(u) == target)
                                    {
                                        Found(target, comps[o], refGos);
                                    }
                                }
                            }
                            else if (value is MonoBehaviour)
                            {
                                var valueMono = value as MonoBehaviour;
                                if (valueMono != null
                                    && valueMono.gameObject == target)
                                {
                                    Found(target, comps[o], refGos);
                                }
                            }
                        }
                    }
                }
            }

            EditorGUIUtility.PingObject(target);

            if (refGos.Count > 0)
            {
                Selection.objects = refGos.ToArray();

                return true;
            }

            return false;
        }

        private static void Found(UnityEngine.Object target, Component comp, List<GameObject> holderList)
        {
            Debug.LogWarning("Found! Target=" + target + " on=" + comp, comp);

            holderList.Add(comp.gameObject);
        }
    }
}