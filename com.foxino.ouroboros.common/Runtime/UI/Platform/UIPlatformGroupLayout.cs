using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.UI.Platform
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public class UIPlatformGroupLayout : UIPlatformBase
    {
        [SerializeField] private float mobileSpacing = 10;
        [SerializeField] private float standaloneSpacing = 10;
        [SerializeField] private bool isUsingPadding;
        [SerializeField] private UIPlatformValueVector4 padding = new UIPlatformValueVector4();
        
        public override void SetMobile()
        {
            SetSpacing(mobileSpacing, UIPlatformType.Mobile);
        }

        public override void SetStandalone()
        {
            SetSpacing(standaloneSpacing, UIPlatformType.Standalone);
        }

        public override void SaveMobile()
        {
            var layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            mobileSpacing = layoutGroup.spacing;
            SavePadding(layoutGroup, UIPlatformType.Mobile);
        }

        public override void SaveStandalone()
        {
            var layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            standaloneSpacing = layoutGroup.spacing;
            SavePadding(layoutGroup, UIPlatformType.Standalone);
        }

        private void SavePadding(HorizontalOrVerticalLayoutGroup layoutGroup, UIPlatformType platformType)
        {
            if (isUsingPadding)
            {
                var paddingValue = new Vector4();
                paddingValue.x = layoutGroup.padding.left;
                paddingValue.y = layoutGroup.padding.right;
                paddingValue.z = layoutGroup.padding.top;
                paddingValue.w = layoutGroup.padding.bottom;

                padding.SetValue(platformType, paddingValue);
            }
        }

        private void SetSpacing(float mobileSpacing, UIPlatformType mobile)
        {
            var layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            layoutGroup.spacing = mobileSpacing;

            SetPadding(mobile, layoutGroup);
        }

        private void SetPadding(UIPlatformType mobile, HorizontalOrVerticalLayoutGroup layoutGroup)
        {
            if (!isUsingPadding) return;

            var paddingValues = padding.GetValue(mobile);
            layoutGroup.padding.left = (int)paddingValues.x;
            layoutGroup.padding.right = (int)paddingValues.y;
            layoutGroup.padding.top = (int)paddingValues.z;
            layoutGroup.padding.bottom = (int)paddingValues.w;
        }
    }
}