using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class ScreenUtils
    {
        public static float ScreenSizeAvg 
        {
            get 
            {
                var screenSizeAvg = (Screen.width + Screen.height) * 0.5f;
                if (screenSizeAvg == 0f)
                {
                    return 1000f;
                }

                return screenSizeAvg;
            } 
        }

        public static float ScreenDPI 
        {
            get 
            {
                if (Screen.dpi == 0f)
                {
                    return 100f;
                }

                return Screen.dpi;
            } 
        }
    }
}