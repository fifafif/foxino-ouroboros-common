using Ouroboros.Common.Inputs;
using System;
using System.IO;
using UnityEngine;

namespace Ouroboros.Common.Utils.Capture
{
    [ExecuteInEditMode]
    public class ScreenShot : MonoBehaviour
    {
        public int resWidth = 1920;
        public int resHeight = 1080;
        public bool is360Equirect;
        public bool useTakeNumber;
        public string takeNumberFormat = "screenshot_{0}";
        public Camera camera;
        public bool useSingleCamera;
        public bool isTransparent;

        private bool takeHiResShot = false;
        private int takeNumber;

        public static string DirectoryPath => Path.Combine(Application.persistentDataPath, "screenshots");

        public string ScreenShotName(int width, int height)
        {
            string filename;
            if (useTakeNumber)
            {
                filename = string.Format(takeNumberFormat, takeNumber);
            }
            else
            {
                filename = string.Format(
                    "screenshot_{0}x{1}_{2}",
                    width,
                    height,
                    DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            }

            return Path.Combine(DirectoryPath, filename + ".png");
        }

        public void TakeScreenShot()
        {
            if (camera == null)
            {
                camera = Camera.main;
            }

            var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);

            if (is360Equirect)
            {
                var rt = RenderEquiRect(camera);

                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                screenShot.Apply();
                camera.targetTexture = null;
                RenderTexture.active = null;
            }
            else
            {
                if (isTransparent)
                {
                    screenShot = RenderTransparentTexture(camera, resWidth, resHeight);
                }
                else
                {
                    screenShot = RenderPerspectiveCamera(camera, resWidth, resHeight, Color.black);
                }
            }

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);

            var folder = Path.GetDirectoryName(filename);
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllBytes(filename, bytes);

            ++takeNumber;

            Debug.Log(string.Format("Took screenshot to: {0}", filename));
        }

        private Texture2D RenderPerspective(Camera targetCamera, Color bgColor)
        {
            var tex2d = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            var rt = RenderTexture.GetTemporary(resWidth, resHeight, 24, RenderTextureFormat.ARGB32);
           
            if (useSingleCamera)
            {
                targetCamera.backgroundColor = bgColor;
                targetCamera.targetTexture = rt;
                targetCamera.Render();
                targetCamera.targetTexture = null;

                RenderTexture.active = rt;
                tex2d.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                tex2d.Apply();
                targetCamera.targetTexture = null;
                RenderTexture.active = null;
            }
            else
            {
                foreach (Camera cam in Camera.allCameras)
                {
                    cam.targetTexture = rt;
                    cam.Render();
                    cam.targetTexture = null;
                }
            }

            RenderTexture.ReleaseTemporary(rt);

            return tex2d;
        }

        public static Texture2D RenderTransparentTexture(Camera targetCamera, int resWidth, int resHeight)
        {
            var blackTex = RenderPerspectiveCamera(targetCamera, resWidth, resHeight, Color.black);
            var whiteTex = RenderPerspectiveCamera(targetCamera, resWidth, resHeight, Color.white);
            return CalculateOutputTexture(whiteTex, blackTex, resWidth, resHeight);
        }

        public static Texture2D RenderPerspectiveCamera(Camera targetCamera, int resWidth, int resHeight, Color bgColor)
        {
            var tex2d = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            var rt = RenderTexture.GetTemporary(resWidth, resHeight, 24, RenderTextureFormat.ARGB32);
        
            targetCamera.backgroundColor = bgColor;
            targetCamera.targetTexture = rt;
            targetCamera.Render();
            targetCamera.targetTexture = null;

            RenderTexture.active = rt;
            tex2d.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            tex2d.Apply();
            targetCamera.targetTexture = null;
            RenderTexture.active = null;

            RenderTexture.ReleaseTemporary(rt);

            return tex2d;
        }

        private RenderTexture RenderEquiRect(Camera targetCamera)
        {
            var rt = new RenderTexture(resWidth, resWidth, 0, RenderTextureFormat.ARGB32);

            rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rt.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            rt.volumeDepth = 6;
            rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Trilinear;
            rt.enableRandomWrite = true;
            rt.isPowerOfTwo = true;

            targetCamera.RenderToCubemap(rt, 63);

            var equiRect = new RenderTexture(resWidth, resHeight, 24);
            equiRect.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;

            rt.ConvertToEquirect(equiRect);
            rt = equiRect;

            return rt;
        }

        private static Texture2D CalculateOutputTexture(Texture2D textureWhite, Texture2D textureBlack, int resWidth, int resHeight)
        {
            var textureTransparentBackground = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);

            Color color;
            for (int y = 0; y < textureTransparentBackground.height; ++y)
            {
                for (int x = 0; x < textureTransparentBackground.width; ++x)
                {
                    var alpha = textureWhite.GetPixel(x, y).r - textureBlack.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = textureBlack.GetPixel(x, y) / alpha;
                    }

                    color.a = alpha;
                    textureTransparentBackground.SetPixel(x, y, color);
                }
            }

            return textureTransparentBackground;
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            takeHiResShot |= InputWrapper.GetKeyDown(KeyCode.P);
#endif

            if (takeHiResShot)
            {
                takeHiResShot = false;
                TakeScreenShot();
            }
        }
    }
}