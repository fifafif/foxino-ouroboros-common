using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [Serializable]
    public class UIPlatformValueBool
    {
        [SerializeField] private bool mobileValue;
        [SerializeField] private bool standaloneValue;

        public bool GetValue(UIPlatformType platformType)
        {
            if (platformType == UIPlatformType.Mobile)
            {
                return mobileValue;
            }

            return standaloneValue;
        }       

        public void SetValue(UIPlatformType platformType, bool value)
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