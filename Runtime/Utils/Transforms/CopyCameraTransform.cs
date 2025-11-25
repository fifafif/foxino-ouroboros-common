using UnityEngine;

namespace Ouroboros.Common.Utils.Transforms
{
    public class CopyCameraTransform : MonoBehaviour
    {
        public bool FreezeRotationY;

        private Transform cameraTransform;

        private void Start()
        {
            if (Camera.main == null) return;

            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (cameraTransform == null) return;

            Quaternion rotation;
            if (FreezeRotationY)
            {
                var forward = cameraTransform.forward;
                forward.y = 0f;

                rotation = Quaternion.LookRotation(forward);
            }
            else
            {
                rotation = cameraTransform.rotation;
            }

            transform.rotation = rotation;
            transform.position = cameraTransform.position;
        }
    }
}