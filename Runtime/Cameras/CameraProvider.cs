using UnityEngine;

namespace Ouroboros.Common.Cameras
{
    public class CameraProvider
    {
        public static Camera Main
        {
            get
            {
                if (mainCamera == null
                    || !mainCamera.isActiveAndEnabled)
                {
                    mainCamera = Camera.main;
                }

                return mainCamera;
            }
        }

        public static Camera UI
        {
            get
            {
                if (uiCamera == null
                    || !uiCamera.isActiveAndEnabled)
                {
                    var uiLayer = LayerMask.NameToLayer("UI");
                    
                    foreach (var camera in Camera.allCameras)
                    {
                        if (camera.gameObject.layer == uiLayer)
                        {
                            uiCamera = camera;
                            break;
                        }
                    }
                }

                return uiCamera;
            }
        }

        public static Vector3 MainPosition
        {
            get
            {
                var cam = Main;
                if (cam == null)
                {
                    return Vector3.zero;
                }

                return cam.transform.position;
            }
        }

        private static Camera mainCamera;
        private static Camera uiCamera;
    }
}