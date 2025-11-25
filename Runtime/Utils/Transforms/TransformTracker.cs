using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class TransformTracker : MonoBehaviour
    {
        public UnityUpdateMode UpdateMode;
        public int BufferCount = 10;
        public bool UseLocalTransform;

        public bool IsVerbose;

        private float[] deltaTimes;
        private Vector3[] positions;
        private int index;
        private int totalSampleCount;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            positions = new Vector3[BufferCount];
            deltaTimes = new float[BufferCount];
        }

        private void Update()
        {
            if (UpdateMode != UnityUpdateMode.Update) return;

            StoreSample(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (UpdateMode == UnityUpdateMode.LateUpdate)
            {
                StoreSample(Time.deltaTime);
            }

            if (IsVerbose)
            {
                var distance = CalculateAverageDistance();
                var velocity = CalculateAverageVelocity();
                Debug.Log($"[TransformTracker] {name} dist={distance.magnitude}, velocty={velocity.magnitude}");
            }
        }

        private void FixedUpdate()
        {
            if (UpdateMode != UnityUpdateMode.FixedUpdate) return;

            StoreSample(Time.fixedDeltaTime);
        }

        public void Clear()
        {
            index = 0;
            totalSampleCount = 0;
        }

        public void SampleUpdate()
        {
            StoreSample(Time.deltaTime);
        }

        public void SamplePhysics()
        {
            StoreSample(Time.fixedDeltaTime);
        }

        public void SampleUpdateWithLastValue()
        {
            var lastPosition = GetLastPosition();
            StoreSample(Time.deltaTime, lastPosition);
        }

        public void SamplePhysicsWithLastValue()
        {
            var lastPosition = GetLastPosition();
            StoreSample(Time.fixedDeltaTime, lastPosition);
        }

        public Vector3 CalculateAverageDistance()
        {
            var sampleCount = GetSampleCount();
            if (sampleCount <= 0)
            {
                return Vector3.zero;
            }

            var total = Vector3.zero;
            var previous = positions[index];

            for (int i = index - 1, count = 0; count < sampleCount - 1; ++count, --i)
            {
                if (i < 0)
                {
                    i = BufferCount - 1;
                }

                var current = positions[i];
                total += previous - current;
                previous = current;
            }

            return total / sampleCount;
        }

        public Vector3 CalculateAverageVelocity()
        {
            var sampleCount = GetSampleCount();
            if (sampleCount <= 1)
            {
                return Vector3.zero;
            }

            var totalDistance = Vector3.zero;
            var totalDuration = 0f;

            var startIndex = index - 1;
            if (startIndex < 0)
            {
                startIndex = sampleCount + startIndex;
            }

            var previousPosition = positions[startIndex];

            for (int i = startIndex - 1, count = 0; count < sampleCount - 1; ++count, --i)
            {
                if (i < 0)
                {
                    i = sampleCount - 1;
                }

                var current = positions[i];
                totalDistance += previousPosition - current;
                previousPosition = current;

                totalDuration += deltaTimes[i];
            }

            if (totalDuration <= 0f)
            {
                return Vector3.zero;
            }

            return totalDistance / totalDuration;
        }

        public Vector3 CalculateAverageDirection()
        {
            var sampleCount = GetSampleCount();
            if (sampleCount <= 1)
            {
                return Vector3.forward;
            }

            var startIndex = index - 2;
            if (startIndex < 0)
            {
                startIndex = sampleCount + startIndex;
            }

            var previousPosition = positions[startIndex];
            var lastPosition = GetLastPosition();

            var direction = lastPosition - previousPosition;
            direction.Normalize();

            return direction;
        }

        private Vector3 GetLastPosition()
        {
            return positions[GetLastIndex()];
        }

        private void StoreSample(float deltaTime)
        {
            Vector3 position;
            if (UseLocalTransform)
            {
                position = transform.localPosition;
            }
            else
            {
                position = transform.position;
            }

            StoreSample(deltaTime, position);
        }

        private void StoreSample(float deltaTime, Vector3 position)
        {
            positions[index] = position;
            deltaTimes[index] = deltaTime;

            ++index;
            if (index >= BufferCount)
            {
                index = 0;
            }

            ++totalSampleCount;
        }

        private int GetLastIndex()
        {
            if (index <= 0)
            {
                return BufferCount - 1;
            }

            return index - 1;
        }

        private int GetLastIndex(int stepCount)
        {
            var lastIndex = index - stepCount;
            if (lastIndex < 0)
            {
                return BufferCount + lastIndex;
            }

            return lastIndex;
        }

        private int GetSampleCount()
        {
            return totalSampleCount > BufferCount ? BufferCount : totalSampleCount;
        }

        private void OnDrawGizmos()
        {
            var direction = CalculateAverageDirection();
            Gizmos.DrawLine(transform.position, transform.position + direction * 5f);
        }
    }
}