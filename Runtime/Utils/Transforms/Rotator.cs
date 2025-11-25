using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 Angles;
        public float Speed;
        public bool IsRandomAngles;
        public bool IsRandomStart;
        public UnityUpdateMode UpdateMode = UnityUpdateMode.Update;
        public bool IsResettingRotationOnEnable;

        private Vector3 rotationAngles;
        private Vector3 origRotation;

        private void Awake()
        {
            origRotation = transform.localEulerAngles;
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (IsRandomAngles)
            {
                rotationAngles = Random.onUnitSphere;
            }
            else
            {
                rotationAngles = Angles;
            }

            if (IsRandomStart)
            {
                Rotate(rotationAngles * Speed * Random.Range(0f, 360f));
            }
        }

        private void Update()
        {
            if (UpdateMode != UnityUpdateMode.Update) return;

            UpdateRotation(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (UpdateMode != UnityUpdateMode.LateUpdate) return;

            UpdateRotation(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (UpdateMode != UnityUpdateMode.FixedUpdate) return;

            UpdateRotation(Time.deltaTime);
        }

        public void UpdateRotation(float deltaTime)
        {
            Rotate(rotationAngles * Speed * deltaTime);
        }

        private void Rotate(Vector3 rotation)
        {
            transform.Rotate(rotation);
        }

        private void OnEnable()
        {
            if (!IsResettingRotationOnEnable) return;

            transform.localEulerAngles = origRotation;
        }
    }
}