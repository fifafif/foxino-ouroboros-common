using System;

namespace Ouroboros.Common.Utils
{
    public class AverageValueCalculator
    {
        public int NumberOfValues { get; private set; }

        private float[] previousSpeeds;
        private int currentIndex;

        public AverageValueCalculator(int numberOfValues)
        {
            if (numberOfValues <= 0)
            {
                throw new ArgumentException("NumberOfValues must be greater than 0!");
            }
            this.NumberOfValues = numberOfValues;
            previousSpeeds = new float[numberOfValues];
        }

        public void AddValue(float value)
        {
            previousSpeeds[currentIndex] = value;

            currentIndex++;
            if (currentIndex >= NumberOfValues)
            {
                currentIndex = 0;
            }
        }

        public float CalculateAverageValue()
        {
            var sum = 0f;
            for (int i = 0; i < NumberOfValues; i++)
            {
                sum += previousSpeeds[i];
            }
            return sum / NumberOfValues;
        }
    }
}