using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ouroboros.Common.Utils
{
    public static class AssetFinder
    {
        public static List<T> FindAssetsByType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset == null) continue;

                assets.Add(asset);
            }

            return assets;
        }

        public static List<T> FindAssetsByType<T>(Type withComponentType) where T : Object
        {
            var assets = new List<T>();
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset == null) continue;

                if ((asset is GameObject gameObject)
                    && gameObject.GetComponent(withComponentType) == null)
                {
                    continue;
                }

                assets.Add(asset);
            }

            return assets;
        }
    }
}