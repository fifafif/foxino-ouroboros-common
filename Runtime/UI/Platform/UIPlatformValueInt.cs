using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [Serializable]
    public class UIPlatformValueInt
    {
        [SerializeField] private int mobileValue;
        [SerializeField] private bool hasTabletValue;
        [SerializeField] private int tabletValue;
        [SerializeField] private int standaloneValue;

        public int GetValue(UIPlatformType platformType)
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

        public void SetValue(UIPlatformType platformType, int value)
        {
            if (platformType == UIPlatformType.Mobile)
            {
                mobileValue = value;
            }
            else
            {
                standaloneValue = value;
            }
        }
    }
}