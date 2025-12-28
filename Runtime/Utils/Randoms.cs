using System.Collections.Generic;
using Ouroboros.Common.Utils.Math;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class Randoms
    {
        public static float Variation(float value, float variation)
        {
            return value * (Random.Range(1 - variation, 1 + variation));
        }

        public static (float, float) VariationMinMax(float value, float variation)
        {
            var random = Random.Range(0, variation);

            return (value * (1 - random), value * (1 + random));
        }

        public static Vector3 Vector3(float maxValue)
        {
            return Vector3(0f, maxValue);
        }

        public static Vector3 Vector3(float minValue, float maxValue)
        {
            return new Vector3(
                Random.Range(minValue, maxValue),
                Random.Range(minValue, maxValue),
                Random.Range(minValue, maxValue));
        }

        public static Vector3 Vector3Uniform(float minValue, float maxValue)
        {
            var value = Random.Range(minValue, maxValue);
            return new Vector3(value, value, value);
        }

        public static Vector3 Vector3(Vector3 minValue, Vector3 maxValue)
        {
            return new Vector3(
                Random.Range(minValue.x, maxValue.x),
                Random.Range(minValue.y, maxValue.y),
                Random.Range(minValue.z, maxValue.z));
        }

        public static Vector3 Vector3(Vector3 maxValues)
        {
            return new Vector3(
                Random.Range(0f, maxValues.x),
                Random.Range(0f, maxValues.y),
                Random.Range(0f, maxValues.z));
        }

        public static Vector3 Vector3MinMax(Vector3 minMaxValues)
        {
            return new Vector3(
                Random.Range(-minMaxValues.x, minMaxValues.x),
                Random.Range(-minMaxValues.y, minMaxValues.y),
                Random.Range(-minMaxValues.z, minMaxValues.z));
        }

        public static Vector3 OnCircle(float spawnAroundRadius)
        {
            var angle = Random.Range(0f, Mathf.PI * 2);

            return new Vector3(
                Mathf.Sin(angle) * spawnAroundRadius,
                0f,
                Mathf.Cos(angle) * spawnAroundRadius);
        }

        public static Vector3 Vector3MinMax(float x, float y, float z)
        {
            return new Vector3(
                Random.Range(-x, x),
                Random.Range(-y, y),
                Random.Range(-z, z));
        }

        public static Vector3 Vector3MinMax(float minMaxValue)
        {
            return new Vector3(
                Random.Range(-minMaxValue, minMaxValue),
                Random.Range(-minMaxValue, minMaxValue),
                Random.Range(-minMaxValue, minMaxValue));
        }

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static Vector3 RandomOnSphereWithLatitudeLimit(float maxLatitude)
        {
            float theta = Random.Range(0f, Mathf.PI * 2f);

            var bell = MathUtils.ApproximateBellCurve(Random.value);
            var phi = Random.Range(bell * -maxLatitude, bell * maxLatitude) * Mathf.Deg2Rad;
            phi += Mathf.PI * 0.5f;

            float x = Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = Mathf.Cos(phi);
            float z = Mathf.Sin(phi) * Mathf.Sin(theta);

            return new Vector3(x, y, z);
        }
    }
}