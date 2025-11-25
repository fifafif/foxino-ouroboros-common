using System;
using Ouroboros.Common.UI.Platform;
using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.UI.Windows
{
    public class Window : MonoBehaviour
    {
        public bool IsClosed { get; private set; }
        public Action OnCloseAction { get; set; }
        public Action OnOpenAction { get; set; }
        public Action OnInitializedAction { get; set; }

        public string Id;

        protected WindowsManager WindowsManager;

        [SerializeField] private bool isPlatformRestricted;
        [SerializeField] private UIPlatformType platformType;

        public bool IsOnCorrectPlatform(UIPlatformType platformType)
        {
            if (!isPlatformRestricted) return true;

            return this.platformType == platformType
                || this.platformType.IsMobileOrTabled() == platformType.IsMobileOrTabled();
        }

        public void Init(WindowsManager windowsManager, WindowConfig windowConfig)
        {
            WindowsManager = windowsManager;
            OnInit(windowConfig);
        }

        public void Open()
        {
            IsClosed = false;
            gameObject.SetActive(true);
            OnOpen();
            OnOpenAction?.Invoke();
        }

        public void Close()
        {
            if (IsClosed) return;

            IsClosed = true;
            OnClose();

            OnCloseAction?.Invoke();
            OnCloseAction = null;

            gameObject.SetActive(false);
            WindowsManager.CloseWindow(this);
        }

        public void LoseFocus()
        {
            gameObject.SetActive(false);
            OnFocusLost();
        }

        public void AcquireFocus()
        {
            gameObject.SetActive(true);
            OnFocusAcquired();
        }

        public virtual void OnInit(WindowConfig windowConfig)
        {
            OnInitializedAction?.Invoke();
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnFocusLost()
        {
        }

        protected virtual void OnFocusAcquired()
        {
        }

        public virtual void EnableInteractables(bool isEnabled)
        {
            var selectables = GetComponentsInChildren<Selectable>();
            foreach (var selectable in selectables)
            {
                selectable.interactable = isEnabled;
            }
        }
    }
}