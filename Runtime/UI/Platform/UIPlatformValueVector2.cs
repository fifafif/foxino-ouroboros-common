using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [Serializable]
    public class UIPlatformValueVector2
    {
        [SerializeField] private Vector2 mobileValue;
        [SerializeField] private Vector2 standaloneValue;

        public Vector2 GetValue(UIPlatformType platformType)
        {
            if (platformType == UIPlatformType.Mobile)
            {
                return mobileValue;
            }

            return standaloneValue;
        }    

        public void SetValue(UIPlatformType platformType, Vector2 value)
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