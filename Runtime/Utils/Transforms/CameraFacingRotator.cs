using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class CameraFacingRotator : MonoBehaviour
    {
        public Vector3 RotationOffset;

        private Transform cameraTransform;

        private void Start()
        {
            GetCameraTransform();
        }

        private void Update()
        {
            Rotate();
        }

        public void Rotate()
        {
            if (cameraTransform == null)
            {
                GetCameraTransform();

                if (cameraTransform == null) return;
            }

            transform.LookAt(cameraTransform, Vector3.up);
            transform.Rotate(RotationOffset);
        }

        private void GetCameraTransform()
        {
            if (Camera.main == null) return;

            cameraTransform = Camera.main.transform;
        }
    }
}