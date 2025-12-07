using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Ouroboros.Common.Logging;

namespace Ouroboros.Common.Audio
{
    public static class AudioStreamingLoader
    {
        /// <summary>
        /// Loads an audio clip from StreamingAssets path asynchronously.
        /// Compatible with all platforms including WebGL browsers.
        /// </summary>
        /// <param name="streamingAssetsPath">Relative path from StreamingAssets folder (e.g., "Music/track.mp3")</param>
        /// <param name="onComplete">Callback with loaded AudioClip or null if failed</param>
        /// <param name="audioType">Audio type (default: AudioType.MPEG for mp3)</param>
        public static IEnumerator LoadAudioClip(
            string streamingAssetsPath,
            Action<AudioClip> onComplete,
            AudioType audioType = AudioType.MPEG)
        {
            if (string.IsNullOrEmpty(streamingAssetsPath))
            {
                Logs.Error<AudioManager>("StreamingAssetsPath is null or empty");
                onComplete?.Invoke(null);
                yield break;
            }

            // Construct the full path to the audio file
            string path = GetStreamingAssetsPath(streamingAssetsPath);

            Logs.Debug<AudioManager>($"Loading audio from: {path}");

            // Use UnityWebRequestMultimedia for audio loading (works on all platforms including WebGL)
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
            {
                // Configure for streaming to reduce memory usage
                DownloadHandlerAudioClip downloadHandler = (DownloadHandlerAudioClip)www.downloadHandler;
                downloadHandler.streamAudio = true;

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Logs.Error<AudioManager>($"Error loading audio from {path}: {www.error}");
                    onComplete?.Invoke(null);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        clip.name = Path.GetFileNameWithoutExtension(streamingAssetsPath);
                        Logs.Debug<AudioManager>($"Successfully loaded audio: {clip.name}");
                        onComplete?.Invoke(clip);
                    }
                    else
                    {
                        Logs.Error<AudioManager>($"Failed to extract AudioClip from {path}");
                        onComplete?.Invoke(null);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the appropriate AudioType based on file extension.
        /// </summary>
        public static AudioType GetAudioTypeFromPath(string path)
        {
            string extension = Path.GetExtension(path).ToLower();

            switch (extension)
            {
                case ".mp3":
                    return AudioType.MPEG;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".wav":
                    return AudioType.WAV;
                case ".aiff":
                case ".aif":
                    return AudioType.AIFF;
                default:
                    Logs.Warning<AudioManager>($"Unknown audio extension {extension}, defaulting to MPEG");
                    return AudioType.MPEG;
            }
        }

        /// <summary>
        /// Constructs the full path to a file in StreamingAssets folder.
        /// Handles platform-specific path requirements (especially WebGL).
        /// </summary>
        private static string GetStreamingAssetsPath(string relativePath)
        {
            // Remove leading slash if present
            relativePath = relativePath.TrimStart('/', '\\');

            string fullPath = Path.Combine(AudioManager.GetStreamingAssetsPath(), relativePath);

            // On WebGL and Android, Application.streamingAssetsPath already returns a URL
            // On other platforms (Windows, Mac, Linux), we need to add file:// prefix
            #if UNITY_WEBGL && !UNITY_EDITOR
                return fullPath;
            #elif UNITY_ANDROID && !UNITY_EDITOR
                return fullPath;
            #else
                // For desktop platforms, UnityWebRequest needs file:// URI scheme
                return "file://" + fullPath;
            #endif
        }
    }
}
