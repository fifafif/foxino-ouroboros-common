using System;
using Ouroboros.Common.UI.Platform;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPlatformRect : UIPlatformBase
{
    [Serializable]
    private struct RectTransformData
    {
        public Vector3 Position;
        public Vector2 AnchoredPosition;
        public Vector2 Size;
        public Vector2 AnchorsMin;
        public Vector2 AnchorsMax;
        public Vector2 Pivot;
        public bool HasData;
    }

    [SerializeField] private RectTransformData mobileRect;
    [SerializeField] private RectTransformData tabletRect;
    [SerializeField] private RectTransformData standaloneRect;

    public override void SetMobile()
    {
        SetData(GetComponent<RectTransform>(), mobileRect);
    }

    public override void SetTablet()
    {
        var data = tabletRect.HasData ? tabletRect : mobileRect;
        SetData(GetComponent<RectTransform>(), data);
    }

    public override void SetStandalone()
    {
        SetData(GetComponent<RectTransform>(), standaloneRect);
    }

    public override void SaveMobile()
    {
        mobileRect = GetData(GetComponent<RectTransform>());
    }

    public override void SaveStandalone()
    {
        standaloneRect = GetData(GetComponent<RectTransform>());
    }

    public override void SaveTablet()
    {
        tabletRect = GetData(GetComponent<RectTransform>());
    }

    private RectTransformData GetData(RectTransform rectTransform)
    {
        return new RectTransformData
        {
            Position = rectTransform.localPosition,
            AnchoredPosition = rectTransform.anchoredPosition,
            Size = rectTransform.sizeDelta,
            AnchorsMin = rectTransform.anchorMin,
            AnchorsMax = rectTransform.anchorMax,
            Pivot = rectTransform.pivot,
            HasData = true
        };
    }

    private void SetData(RectTransform rectTransform, RectTransformData data)
    {
        if (!data.HasData) return;

        rectTransform.anchorMin = data.AnchorsMin;
        rectTransform.anchorMax = data.AnchorsMax;
        rectTransform.sizeDelta = data.Size;
        rectTransform.pivot = data.Pivot;
        rectTransform.localPosition = data.Position;
        rectTransform.anchoredPosition = data.AnchoredPosition;
    }
}
