using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.UI.Platform
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UIPlatformScaler : UIPlatformBase
    {
        [SerializeField] private Vector2 mobileResolution = new Vector2(800, 600);
        [SerializeField] private Vector2 tabletResolution = new Vector2(1000, 750);
        [SerializeField] private Vector2 standaloneResolution = new Vector2(1600, 1200);

        public override void SetMobile()
        {
            SetResolution(mobileResolution);
        }

        public override void SetTablet()
        {
            SetResolution(tabletResolution);
        }

        public override void SetStandalone()
        {
            SetResolution(standaloneResolution);
        }

        public void SetResolution(Vector2 resolution)
        {
            GetComponent<CanvasScaler>().referenceResolution = resolution;
        }
    }
}