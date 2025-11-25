using Ouroboros.Common.Platform;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    public enum UIPlatformType
    {
        Mobile = 0,
        Standalone = 1,
        Tablet = 2
    }

    public static class UIPlatformUtils
    {
        public static UIPlatformType GetUIPlatformType()
        {
            if (PlatformUtils.IsMobile())
            {
                if (IsTablet())
                {
                    return UIPlatformType.Tablet;
                }
                else
                {
                    return UIPlatformType.Mobile;
                }
            }

            return UIPlatformType.Standalone;
        }   

        public static bool IsMobileOrTabled(this UIPlatformType type)
        {
            return type == UIPlatformType.Mobile || type == UIPlatformType.Tablet;
        }

        public static bool IsTablet()
        {
#if UNITY_EDITOR && OUROBOROS_MOBILE
    #if OUROBOROS_TABLET

            return true;

    #else

            return false;

    #endif
#endif

            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalSize = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);

            // Debug.Log($"Device Diagonal Size: {diagonalSize} inches");

            return diagonalSize >= 7f;
        }
    }
}
