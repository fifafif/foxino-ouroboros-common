using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [Serializable]
    public class UIPlatformValueFloat
    {
        [SerializeField] private float mobileValue = 1f;
        [SerializeField] private bool hasTabletValue;
        [SerializeField] private float tabletValue = 1f;
        [SerializeField] private float standaloneValue = 1f;

        public float GetValue(UIPlatformType platformType)
        {
            if (platformType == UIPlatformType.Standalone)
            {
                return standaloneValue;
            }

            if (platformType == UIPlatformType.Mobile
                || !hasTabletValue)
            {
                return mobileValue;
            }
            
            return tabletValue;
        }     
        
        public void SetValue(UIPlatformType platformType, float value)
        {
            if (platformType == UIPlatformType.Mobile)
            {
                mobileValue = value;
            }
            else if (platformType == UIPlatformType.Tablet)
            {
                tabletValue = value;
            }
            else
            {
                standaloneValue = value;
            }
        }  
    }
}