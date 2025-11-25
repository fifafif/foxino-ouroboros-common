#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class AudioEditorPlayer
{
    public static void PlayClip(AudioClip clip, bool isLooping = false)
    {
        var unityEditorAssembly = typeof(AudioImporter).Assembly;
        var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        var method = audioUtilClass.GetMethod("PlayPreviewClip");

        method.Invoke(null, new object[] { clip, 0, isLooping });
    }

	public static void StopClip(AudioClip clip)
	{
		var unityEditorAssembly = typeof(AudioImporter).Assembly;
		var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		var method = audioUtilClass.GetMethod("StopAllPreviewClips");

		method.Invoke(null, new object[] { });
		//method.Invoke(null, new object[] { clip });
	}
}

#endif