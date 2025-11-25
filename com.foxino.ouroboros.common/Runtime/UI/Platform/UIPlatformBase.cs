using System;
using Ouroboros.Common.Platform;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;
#endif



namespace Ouroboros.Common.UI.Platform
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)]
    public class UIPlatformBase : MonoBehaviour
    {
        private void Awake()
        {
            OnAwake();
            
            if (Application.isPlaying)
            {
                SetFromPlatform();
            }
        }

        protected virtual void OnAwake()
        {
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                SetFromPlatform();
            }
        }
#endif

        public void SetFromPlatform()
        {
            var platform = UIPlatformUtils.GetUIPlatformType();
            if (platform == UIPlatformType.Mobile)
            {
                SetMobile();
            }
            else if (platform == UIPlatformType.Tablet)
            {
                SetTablet();
            }
            else
            {
                SetStandalone();
            }

#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }

#endif
        }

        public virtual void SetMobile() { }
        public virtual void SetTablet() { }

        public virtual void SetStandalone() { }

        public virtual void SaveStandalone() { }
        public virtual void SaveTablet() { }

        public virtual void SaveMobile() { }
    }
}
