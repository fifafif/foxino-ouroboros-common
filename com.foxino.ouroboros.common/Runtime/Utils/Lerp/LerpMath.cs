using UnityEngine;

namespace Ouroboros.Common.Utils.Lerp
{
    public static class LerpMath
    {
        public static float CalculateLerp(this LerpMode mode, float t)
        {
            return CalculateLerp(t, mode);
        }

        public static float CalculateLerp(float t, LerpMode mode)
        {
            t = Mathf.Clamp01(t);

            switch (mode)
            {
                case LerpMode.Linear:
                    return t;

                case LerpMode.EaseOut:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);

                case LerpMode.EaseIn:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

                case LerpMode.EaseInOut:
                    return EaseInOut(t);

                case LerpMode.Smoothstep:
                    return t * t * (3f - 2f * t);

                case LerpMode.Smootherstep:
                    return t * t * t * (t * (6f * t - 15f) + 10f);

                case LerpMode.EaseOutPower3:
                    t = 1 - t;
                    return 1 - t * t * t;

                case LerpMode.EaseOutPower5:
                    t = 1 - t;
                    return 1 - t * t * t * t * t;

                case LerpMode.EaseInPower3:
                    return t * t * t;

                case LerpMode.EaseInPower5:
                    return t * t * t * t * t;

                case LerpMode.EaseInOutPower3:
                    return EaseInOutPower3(t);

                case LerpMode.EaseInOutPower5:
                    return EaseInOutPower5(t);

                case LerpMode.EaseInElastic:
                    return EaseInElastic(t);

                case LerpMode.EaseOutElastic:
                    return EaseOutElastic(t);

                case LerpMode.EaseInOutElastic:
                    return EaseInOutElastic(t);

                case LerpMode.EaseInBounce:
                    return EaseInBounce(t);

                case LerpMode.EaseOutBounce:
                    return EaseOutBounce(t);

                case LerpMode.EaseInOutBounce:
                    return EaseInOutBounce(t);

                default:
                    return t;
            }
        }

        private static float EaseInElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;

            const float c4 = (2 * Mathf.PI) / 3;
            return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
        }

        private static float EaseOutElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;

            const float c4 = (2 * Mathf.PI) / 3;
            return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        private static float EaseInOutElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;

            const float c5 = (2 * Mathf.PI) / 4.5f;

            if (t < 0.5f)
            {
                return -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2;
            }
            else
            {
                return (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
            }
        }

        private static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }
            else if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / d1;
                return n1 * t * t + 0.984375f;
            }
        }

        private static float EaseInBounce(float t)
        {
            return 1 - EaseOutBounce(1 - t);
        }

        private static float EaseInOutBounce(float t)
        {
            if (t < 0.5f)
            {
                return (1 - EaseOutBounce(1 - 2 * t)) / 2;
            }
            else
            {
                return (1 + EaseOutBounce(2 * t - 1)) / 2;
            }
        }

        private static float EaseInOut(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        }

        private static float EaseInOutPower3(float t)
        {
            if (t < 0.5f)
            {
                return 4 * t * t * t;
            }
            else
            {
                float p = -2 * t + 2;
                return 1 - p * p * p / 2;
            }
        }

        private static float EaseInOutPower5(float t)
        {
            if (t < 0.5f)
            {
                return 16 * t * t * t * t * t;
            }
            else
            {
                float p = -2 * t + 2;
                return 1 - p * p * p * p * p / 2;
            }
        }

        public static float Power3(float t)
        {
            return t * t * t;
        }

        public static float Power5(float t)
        {
            return t * t * t * t * t;
        }

        public static float Lerp(float a, float b, float t, LerpMode lerpMode)
        {
            return Mathf.Lerp(a, b, lerpMode.CalculateLerp(t));
        }
    }
}