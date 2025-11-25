using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Utils.Visuals
{
    [ExecuteAlways]
    public class ProgressBar : MonoBehaviour
    {
        public enum Mode
        {
            Scale,  
            ImageFill,
            ImageSize,
            Color
        }

        [Serializable]
        public class BarColor
        {
            public float AmountMinValue;
            public Color Color = Color.red;
        }

        [Range(0f, 1f)]
        [SerializeField] private float fillAmount = 1f;
        [SerializeField] private Transform bar;
        [SerializeField] private Image barImage;
        [SerializeField] private Image colorImage;
        [SerializeField] private Mode mode;
        [SerializeField] private float imageSizeMax = 100f;
        [SerializeField] private BarColor[] colors;

        protected Color CurrentBarColor;

        private SpriteRenderer barSprite;
        private Color overrideColor;
        private float overrideColorFactor;
        private float overrideColorDuration;
        private float overrideColorRemainingTime;

        private void Awake()
        {
            if (bar != null)
            {
                barSprite = bar.GetComponentInChildren<SpriteRenderer>();
            }

            if (colorImage == null)
            {
                colorImage = barImage;
            }

            Array.Sort(colors, (a, b) => b.AmountMinValue.CompareTo(b.AmountMinValue));
        }

        private void Update()
        {
            Fill(fillAmount);

            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public virtual void Restore()
        {
            fillAmount = 1f;
        }

        public void SetFillAmount(float fillAmount)
        {
            this.fillAmount = Mathf.Clamp01(fillAmount);
        }

        public void SetOverrideColor(Color color, float factor)
        {
            overrideColor = color;
            overrideColorFactor = factor;
        }

        public void ResetOverrideColor()
        {
            overrideColorFactor = 0f;
        }

        public void SetOverrideColorDuration(Color color, float factor, float duration)
        {
            overrideColor = color;
            overrideColorFactor = factor;
            overrideColorDuration = duration;
            overrideColorRemainingTime = overrideColorDuration;
        }

        private void Fill(float amount)
        {
            UpdateSize(amount);
            UpdateColor(amount);
        }

        private void UpdateColor(float amount)
        {
            if (!TryFindColor(amount, out var color)) return;

            color = UpdateColorOverride(color);

            CurrentBarColor = color;

            if (barSprite != null)
            {
                barSprite.color = color;
            }
            else if (colorImage != null)
            {
                colorImage.color = color;
            }
        }

        private Color UpdateColorOverride(Color color)
        {
            if (overrideColorDuration > 0f)
            {
                if (overrideColorRemainingTime > 0f)
                {
                    var f = overrideColorRemainingTime / overrideColorDuration;
                    color = Color.Lerp(color, overrideColor, f);
                    overrideColorRemainingTime -= Time.deltaTime;
                }
                else
                {
                    overrideColorFactor = 0f;
                    overrideColorDuration = 0f;
                }

                return color;
            }

            return Color.Lerp(color, overrideColor, overrideColorFactor);
        }

        private void UpdateSize(float amount)
        {
            switch (mode)
            {
                case Mode.Scale:
                    if (bar == null) return;

                    var scale = bar.transform.localScale;
                    scale.x = amount;
                    bar.transform.localScale = scale;
                    break;

                case Mode.ImageFill:
                    if (barImage == null) return;

                    barImage.fillAmount = amount;
                    break;

                case Mode.ImageSize:
                    if (barImage == null) return;

                    var size = barImage.rectTransform.sizeDelta;
                    size.x = amount * imageSizeMax;
                    barImage.rectTransform.sizeDelta = size;
                    break;
            }
        }

        private bool TryFindColor(float amount, out Color color)
        {
            for (var i = 0; i < colors.Length; i++)
            {
                if (colors[i].AmountMinValue <= amount)
                {
                    color = colors[i].Color;
                    return true;
                }
            }

            color = Color.white;
            return false;
        }
    }
}
