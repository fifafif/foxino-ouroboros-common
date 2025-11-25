#if !UNITY_STANDALONE
    #define DISABLESTEAMWORKS 
#endif

using Ouroboros.Common.Platform;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if OUROBOROS_VR

using Unity.XR.Oculus;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
//using UnityEngine.XR.OpenXR;

#endif

namespace Ouroboros.Common.Platform
{
    public class EnvironmentUtils
    {
        public const string ScriptDefineProd = "OUROBOROS_PROD";
        public const string ScriptDefineQA = "OUROBOROS_QA";
        public const string ScriptDefineDemo = "OUROBOROS_DEMO";
        public const string ScriptDefineVR = "OUROBOROS_VR";
        public const string ScriptDefineOculus = "OUROBOROS_OCULUS";
        public const string ScriptDefineSteam = "OUROBOROS_STEAM";
        public const string ScriptDefineForceLogs = "OUROBOROS_FORCE_LOGS";
        public const string ScriptDefineLog = "OUROBOROS_LOG";
        public const string ScriptDefineMobile = "OUROBOROS_MOBILE";
        public const string ScriptDefineTablet = "OUROBOROS_TABLET";
        public const string ScriptDefineNoAds = "OUROBOROS_NO_ADS";

        private const string MenuPath = "Ouroboros/Environment/";
        private const string MenuProd = MenuPath + "Prod";
        private const string MenuDev = MenuPath + "Dev";
        private const string MenuQA = MenuPath + "QA";
        private const string MenuDemo = MenuPath + "Demo";
        private const string MenuOculus = MenuPath + "Oculus";
        private const string MenuSteam = MenuPath + "Steam";
        private const string MenuMobile = MenuPath + "Mobile";
        private const string MenuStandalone = MenuPath + "Standalone";
        private const string MenuTablet = MenuPath + "Tablet";

        [MenuItem(MenuProd, priority = 1)]
        public static void SetPlatformProd()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineProd, group);
        }

        [MenuItem(MenuDev, priority = 1)]
        public static void SetPlatformDev()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineProd, group);
        }
        
        [MenuItem(MenuQA, priority = 1)]
        public static void SetPlatformQA()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineProd, group);
            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineQA, group);
        }

        [MenuItem(MenuProd, true)]
        public static bool SetPlatformProdValidate()
        {
            var isProd = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineProd);
            Menu.SetChecked(MenuProd, isProd);

            return true;
        }

        [MenuItem(MenuDev, true)]
        public static bool SetPlatformDevValidate()
        {
            var isProd = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineProd);
            Menu.SetChecked(MenuDev, !isProd);

            return true;
        }
        
        [MenuItem(MenuQA, true)]
        public static bool SetPlatformQAValidate()
        {
            var isQA = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineQA);
            Menu.SetChecked(MenuQA, !isQA);

            return true;
        }

        [MenuItem(MenuDemo, priority = 20)]
        public static void SetPlatformDemo()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            var hasDemo = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineDemo);
            if (hasDemo)
            {
                PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineDemo, group);
            }
            else
            {
                PlatformUtilsEditor.AddToScriptDefines(ScriptDefineDemo, group);
            }
        }

        [MenuItem(MenuDemo, true)]
        public static bool SetPlatformDemoValidate()
        {
            var hasDemo = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineDemo);
            Menu.SetChecked(MenuDemo, hasDemo);

            return true;
        }

        [MenuItem(MenuOculus, priority = 40)]
        public static void SetPlatformOculus()
        {
#if OUROBOROS_VR
        
            var group = PlatformUtils.GetCurrentBuildTargetGroup();

            SetupXREnvironment(group, true, true);
            SetPlatformOculus(group);

#endif
        }

        [MenuItem(MenuSteam, priority = 40)]
        public static void SetPlatformSteam()
        {
#if !UNITY_STANDALONE

            EditorUtility.DisplayDialog(
                "Platform Error", 
                "Cannot set Steam platform and not have Unity Standalone build target. Please switch to Standalone target in the build settings.", 
                "OK");
            return;

#endif
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();


#if OUROBOROS_VR
        
            SetupXREnvironment(group, true, false);
      
#endif
        
            SetPlatformSteam(group);
        }

        [MenuItem(MenuOculus, true)]
        public static bool SetPlatformOculusValidate()
        {
            var hasDefines = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineOculus);
            Menu.SetChecked(MenuOculus, hasDefines);

            return true;
        }

        [MenuItem(MenuSteam, true)]
        public static bool SetPlatformSteamValidate()
        {
            var hasDefines = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineSteam);
            Menu.SetChecked(MenuSteam, hasDefines);

            return true;
        }

        public static void SetPlatformSteam(BuildTargetGroup group)
        {
            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineOculus, group);
            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineSteam, group);
        }

        public static void SetPlatformOculus(BuildTargetGroup group)
        {
            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineSteam, group);
            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineOculus, group);
        }

        [MenuItem(MenuMobile, priority = 60)]
        public static void SetPlatformMobile()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineMobile, group);
            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineTablet, group);
        }

        [MenuItem(MenuTablet, priority = 61)]
        public static void SetPlatformTable()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.AddToScriptDefines(ScriptDefineMobile, group);

            var isTablet = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineTablet);
            if (isTablet)
            {
                PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineTablet, group);
            }
            else
            {
                PlatformUtilsEditor.AddToScriptDefines(ScriptDefineTablet, group);
            }
        }

        [MenuItem(MenuStandalone, priority = 62)]
        public static void SetPlatformStandalone()
        {
            var group = PlatformUtilsEditor.GetCurrentBuildTargetGroup();

            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineMobile, group);
            PlatformUtilsEditor.RemoveFromScriptDefines(ScriptDefineTablet, group);
        }

        [MenuItem(MenuStandalone, true)]
        public static bool SetPlatformStandaloneValidate()
        {
            UpdatePlatformMenuStatus();
            return true;
        }
        
        [MenuItem(MenuMobile, true)]
        public static bool SetPlatformMobileValidate()
        {
            UpdatePlatformMenuStatus();
            return true;
        }

        [MenuItem(MenuTablet, true)]
        public static bool SetPlatformTabletValidate()
        {
            UpdatePlatformMenuStatus();
            return true;
        }
        
        private static void UpdatePlatformMenuStatus()
        {
            var isTablet = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineTablet);
            Menu.SetChecked(MenuTablet, isTablet);

            var isMobile = PlatformUtilsEditor.ScriptDefinesContains(ScriptDefineMobile);
            Menu.SetChecked(MenuMobile, isMobile);
            Menu.SetChecked(MenuStandalone, !isMobile);
        }

#if OUROBOROS_VR
        
        public static void SetupXREnvironment(
            BuildTargetGroup targetGroup, bool isVREnabled, bool isOculus)
        {
            EditorBuildSettings.TryGetConfigObject(
                XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);

            var settings = buildTargetSettings.SettingsForBuildTarget(targetGroup);
            var loaders = new List<XRLoader>();

            settings.InitManagerOnStart = isVREnabled;

            if (isVREnabled)
            {
                if (isOculus)
                {
                    loaders.Add(GetXRLoader<OculusLoader>());
                    //loaders.Add(GetXRLoader<OpenXRLoader>());
                }
                else
                {
#if UNITY_STANDALONE

                    loaders.Add(GetXRLoader<Unity.XR.OpenVR.OpenVRLoader>());

#endif

                    // We don't have full support for OpenXR now.
                    //loaders.Add(GetXRLoader<OpenXRLoader>());
                    loaders.Add(GetXRLoader<OculusLoader>());
                }
            }
                    EditorUtility.SetDirty(buildTargetSettings);

            if (!settings.Manager.TrySetLoaders(loaders))
            {
                Debug.LogError("Failed to set the VR loader list!");
            }
        }

        private static XRLoader GetXRLoader<T>() where T : XRLoader
        {
            var loaders = AssetFinder.FindAssetsByType<T>();
            if (loaders.Count <= 0)
            {
                Debug.LogError($"[EnvironmentUtils] Cannot find XR Loader type={typeof(T)}. Please create one.");
                return null;
            }

            return loaders[0];
        }

#endif

    }
}
