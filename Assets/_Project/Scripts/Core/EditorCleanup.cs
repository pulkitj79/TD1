#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EditorCleanup
{
    static EditorCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Find and destroy all DontDestroyOnLoad objects
            GameObject[] rootObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (var obj in rootObjects)
            {
                if (obj.scene.name == "DontDestroyOnLoad" || obj.name == "EventSystem")
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }
}
#endif