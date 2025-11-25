using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    public class UIPlatformObjectActivator : UIPlatformBase
    {
        [SerializeField] private GameObject[] targets;
        [SerializeField] private UIPlatformType[] activeOnPlatform; 

        public override void SetMobile()
        {
            ActivateObjects(UIPlatformType.Mobile);
        }

        public override void SetStandalone()
        {
            ActivateObjects(UIPlatformType.Standalone);
        }

        private void ActivateObjects(UIPlatformType platformType)
        {
            foreach (var platform in activeOnPlatform)
            {
                if (platform == platformType)
                {
                    ActivateObjects(true);
                    return;
                }
            }

            ActivateObjects(false);
        }

        private void ActivateObjects(bool isActive)
        {
            foreach (var obj in targets)
            {
                obj.SetActive(isActive);
            }
        }
    }
}