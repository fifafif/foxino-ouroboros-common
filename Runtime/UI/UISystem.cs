using Ouroboros.Common.Cameras;
using Ouroboros.Common.Inputs;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Services;
using Ouroboros.Common.UI.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ouroboros.Common.UI
{
    public class UISystem : MonoBehaviour
    {
        public WindowsManager WindowManager => windowManager;
        public Action OnMouseReleaseOnEmptySpace { get; set; }
        public Action OnMousePressOnEmptySpace { get; set; }

        [SerializeField] private WindowsManager windowManager;

        private static UISystem instance;
        private static readonly List<RaycastResult> uiRaycastResults = new();
        private RectTransform rectTransform;
        private Vector3 dragStartPos;
        private bool isPointerOverUIOnDown;
        private float dragThreshold = 5f;
        private bool isTouching;

        private void Awake()
        {
            instance = this;
            ServiceLocator.Register(this);
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Update()
        {
            ProcessWrapperInputTouch();
        }

        private void ProcessWrapperInputTouch()
        {
            if (InputWrapper.GetMouseOrTouchDown(0))
            {
                dragStartPos = InputWrapper.GetMouseOrTouchPosition();
                isTouching = true;
                isPointerOverUIOnDown = IsPointerOverUI();
                if (!isPointerOverUIOnDown)
                {
                    OnMousePressOnEmptySpace?.Invoke();
                }
            }
            else if (isTouching && InputWrapper.GetMouseOrTouchUp(0))
            {
                isTouching = false;
                var touchPos = InputWrapper.GetMouseOrTouchPosition();
                var isDragging = Vector3.Distance(touchPos, dragStartPos) > dragThreshold;
                if (!isDragging && !IsPointerOverUI() && !isPointerOverUIOnDown)
                {
                    Logs.Debug<UISystem>("OnMouseReleaseOnEmptySpace Invoke");
                    OnMouseReleaseOnEmptySpace?.Invoke();
                }
            }
        }

        public static Vector3 CalculateScreenPositionFromWorld(Vector3 worldPosition)
        {
            worldPosition.z = 0f;
            var pos = CameraProvider.Main.WorldToViewportPoint(worldPosition);
            var res = instance.rectTransform.sizeDelta;

            return new Vector2(pos.x * res.x - res.x * 0.5f, pos.y * res.y - res.y * 0.5f);
        }

        public static List<RaycastResult> GetUIRaycastHits()
        {
            if (EventSystem.current == null)
            {
                return uiRaycastResults;
            }

            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = InputWrapper.GetMouseOrTouchPosition();

            uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, uiRaycastResults);

            return uiRaycastResults;
        }

        public static bool IsClickOnEmptySpace(string tagToIgnore = null)
        {
            return InputWrapper.GetMouseButtonDown(0)
                && !IsPointerOverUI(tagToIgnore);
        }

        public static bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = InputWrapper.GetMouseOrTouchPosition();

            uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, uiRaycastResults);

            return uiRaycastResults.Count > 0;
        }

        public static bool IsPointerOverUI(string tagToIgnore)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = InputWrapper.GetMouseOrTouchPosition();

            uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, uiRaycastResults);

            if (string.IsNullOrEmpty(tagToIgnore))
            {
                return uiRaycastResults.Count > 0;
            }

            for (int i = 0; i < uiRaycastResults.Count; i++)
            {
                if (!uiRaycastResults[i].gameObject.CompareTag(tagToIgnore))
                {
                    return true;
                }
            }

            return false;
        }

        public static Vector3 WorldPositionToLocalRectPosition(Vector3 worldPosition, RectTransform rectTransform)
        {
            var camera = CameraProvider.Main;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);

            // Debug.Log($"XXX worldPos={worldPosition}, Sreeen point={screenPoint}, cma={camera.name}");

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPoint, null, out var localPos);

            // Debug.Log($"XXX Local pos={localPos.ToString("F4")}");
            return localPos;
        }

        public static bool IsPointerOverLayer(LayerMask layerMask)
        {
            // Debug.Log($"XXX {InputWrapper.GetMouseOrTouchPosition()}");
            var ray = CameraProvider.Main.ScreenPointToRay(InputWrapper.GetMouseOrTouchPosition());
            // return Physics.Raycast(ray, Mathf.Infinity, LayerMask.NameToLayer("Selectable"));
            return Physics.Raycast(ray, 1000, layerMask);
        }
    }
}
