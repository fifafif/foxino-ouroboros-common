using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [Serializable]
    public class UIPlatformValueVector4
    {
        [SerializeField] private Vector4 mobileValue;
        [SerializeField] private Vector4 standaloneValue;

        public Vector4 GetValue(UIPlatformType platformType)
        {
            if (platformType == UIPlatformType.Mobile)
            {
                return mobileValue;
            }

            return standaloneValue;
        }    

        public void SetValue(UIPlatformType platformType, Vector4 value)
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