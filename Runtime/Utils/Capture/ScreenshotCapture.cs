using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ouroboros.Utils.Capture
{
    [ExecuteInEditMode]
    public class ScreenshotCapture : MonoBehaviour
    {
        public bool IsMultiCapture => CaptureCount > 1;

        public int ResolutionWidth = 1920;
        public int ResolutionHeight = 1080;

        [Header("Camera Settings")]
        public Camera Camera;
        public int FOV = 60;
        public bool UseFOV;
        public bool UseSingleCamera;
        public bool IsTransparent;
        public bool Is360Equirect;
        
        [Header("Capture")]
        public int CaptureCount = 1;
        public int CaptureIntervalMs = 100;
        public int CaptureStartDelayMs = 0;
        public bool SaveFilesOnExit;
        public bool UseTakeNumber;
        public string TakeNumberFormat = "screenshot_{0}";

        [Header("Trigger")]
        public bool UseKeyboard;
        public KeyCode KeyCode = KeyCode.Space;

        private int sequenceTakeNumber;
        private int totalTakeNumber;
        private bool isCapturing;
        private List<Texture2D> capturedTextures;
        private float timeToNextCapture;

        public static string DirectoryPath => Path.Combine(Application.persistentDataPath, "screenshots");


#if UNITY_EDITOR

        private void LateUpdate()
        {
            if (UseKeyboard
                && Input.GetKeyDown(KeyCode))
            {
                StartCapture();
            }

            ProcessMultiCapture();
        }

        private void OnApplicationQuit()
        {
            if (SaveFilesOnExit)
            {
                SaveCapturesIntoFiles();
            }
        }

#endif

        public void StartCapture()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
            }

            if (IsMultiCapture)
            {
                if (!SaveFilesOnExit
                    || capturedTextures == null)
                { 
                    capturedTextures = new List<Texture2D>();
                }

                sequenceTakeNumber = 0;
                timeToNextCapture += CaptureStartDelayMs * 0.001f;
                isCapturing = true;
            }
            else
            {
                if (SaveFilesOnExit
                    && capturedTextures == null)
                {
                    capturedTextures = new List<Texture2D>();
                }

                CaptureScreenshot();
            }
        }

        private void ProcessMultiCapture()
        {
            if (!isCapturing) return;

            if (timeToNextCapture <= 0f)
            {
                timeToNextCapture += CaptureIntervalMs * 0.001f;
                CaptureScreenshot();

                if (sequenceTakeNumber >= CaptureCount)
                {
                    isCapturing = false;

                    if (!SaveFilesOnExit)
                    {
                        SaveCapturesIntoFiles();
                    }
                }
            }

            timeToNextCapture -= Time.deltaTime;
        }

        private void SaveCapturesIntoFiles()
        {
            if (capturedTextures == null) return;

            for (int i = 0; i < capturedTextures.Count; i++)
            {
                ExportTextureToFile(capturedTextures[i], i);
            }
        }

        private void CaptureScreenshot()
        {
            Texture2D texture;

            if (Is360Equirect)
            {
                texture = RenderEquiRect(Camera);
            }
            else if (IsTransparent)
            {
                var blackTex = RenderPerspective(Camera, Color.black);
                var whiteTex = RenderPerspective(Camera, Color.white);
                texture = CalculateOutputTexture(whiteTex, blackTex);
            }
            else
            {
                texture = RenderPerspective(Camera, Color.black);
            }

            if (IsMultiCapture
                || SaveFilesOnExit)
            {
                capturedTextures.Add(texture);
            }
            else
            {
                ExportTextureToFile(texture, totalTakeNumber);
            }

            ++sequenceTakeNumber;
            ++totalTakeNumber;
        }

        private void ExportTextureToFile(Texture2D texture, int takeNumber)
        {
            var bytes = texture.EncodeToPNG();
            var filename = GetFilename(takeNumber);

            var folder = Path.GetDirectoryName(filename);
            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllBytes(filename, bytes);

            Debug.Log($"[ScreenshotCapture] Screenshot saved into a file: {filename}");
        }

        private Texture2D RenderPerspective(Camera camera, Color bgColor)
        {
            var renderTexture = RenderTexture.GetTemporary(
                ResolutionWidth, ResolutionHeight, IsTransparent ? 24 : 0, RenderTextureFormat.DefaultHDR);

            if (UseSingleCamera)
            {
                if (IsTransparent)
                {
                    camera.backgroundColor = bgColor;
                }

                RenderCamera(camera, renderTexture);
            }
            else
            {
                foreach (Camera cam in Camera.allCameras)
                {
                    RenderCamera(cam, renderTexture);
                }
            }

            return ConvertRenderTextureToTexture2D(renderTexture);
        }

        private void RenderCamera(Camera camera, RenderTexture renderTexture)
        {
            var origFOV = camera.fieldOfView;

            if (UseFOV)
            {
                camera.fieldOfView = FOV;
            }

            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = null;

            if (UseFOV)
            {
                camera.fieldOfView = origFOV;
            }
        }

        private Texture2D RenderEquiRect(Camera targetCamera)
        {
            var rt = RenderTexture.GetTemporary(ResolutionWidth, ResolutionWidth, 0, RenderTextureFormat.ARGB32);

            rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rt.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            rt.volumeDepth = 6;
            rt.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Trilinear;
            rt.enableRandomWrite = true;
            rt.isPowerOfTwo = true;

            targetCamera.RenderToCubemap(rt, 63);

            var equiRect = new RenderTexture(ResolutionWidth, ResolutionHeight, 24);
            equiRect.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            rt.ConvertToEquirect(equiRect);

            return ConvertRenderTextureToTexture2D(equiRect);
        }

        private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            var tex2D = new Texture2D(ResolutionWidth, ResolutionHeight, TextureFormat.ARGB32, false);
            tex2D.ReadPixels(new Rect(0, 0, ResolutionWidth, ResolutionHeight), 0, 0);
            tex2D.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);

            return tex2D;
        }

        private Texture2D CalculateOutputTexture(Texture2D textureWhite, Texture2D textureBlack)
        {
            var textureTransparentBackground = new Texture2D(ResolutionWidth, ResolutionHeight, TextureFormat.ARGB32, false);

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

        private string GetFilename(int takeNumber)
        {
            string filename;
            if (UseTakeNumber)
            {
                filename = string.Format(TakeNumberFormat, takeNumber);
            }
            else
            {
                filename = string.Format(
                    "screenshot_{0}x{1}_{2}{3}",
                    ResolutionWidth,
                    ResolutionHeight,
                    DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                    IsMultiCapture || SaveFilesOnExit ? $"_{takeNumber}" : "");
            }

            return Path.Combine(DirectoryPath, filename + ".png");
        }
    }
}