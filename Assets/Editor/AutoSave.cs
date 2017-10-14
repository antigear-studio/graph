using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoSave {
    static AutoSave() {
        EditorApplication.playModeStateChanged += mode => {
            if (EditorApplication.isPlayingOrWillChangePlaymode && 
                !EditorApplication.isPlaying) {
                if (EditorSceneManager.SaveOpenScenes()) {
                    Debug.Log("Auto-save success!");
                }

                AssetDatabase.SaveAssets();
            }
        };
    }
}
