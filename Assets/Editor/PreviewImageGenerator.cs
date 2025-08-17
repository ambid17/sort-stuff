using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

public class PreviewImageGenerator : EditorWindow
{
    [MenuItem("Tools/GeneratePreview")]
    private static void GeneratePreview()
    {
        GetWindow<PreviewImageGenerator>();
    }

    private void OnGUI()
    {
        GUILayout.Label("This creates a preview image");
        if (GUILayout.Button("Create"))
        {
            GeneratePrefabPreview(Selection.activeObject as GameObject);
        }
    }

    public static void GeneratePrefabPreview(GameObject prefab)
    {
        string texturesFolder = Path.Combine(Application.dataPath, "Textures");
        string previewsFolder = Path.Combine(texturesFolder, "GeneratedPreviews");
        string fileName = $"{prefab.name}.png";
        string fullPath = Path.Combine(previewsFolder, fileName);
        if(File.Exists(fullPath))
        {
            Debug.Log($"Preview already exists for {prefab.name} at {fullPath}");
            return;
        }

        Debug.Log($"Generating preview for: {prefab.name}");
        AssetPreview.SetPreviewTextureCacheSize(2);

        Texture2D preview = null;
        do
        {
            preview = AssetPreview.GetAssetPreview(prefab);
        }
        while (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()));

        if (preview != null)
        {
            preview.Apply();
            byte[] data = preview.EncodeToPNG();

            if (!File.Exists(fullPath))
            {
                File.WriteAllBytes(fullPath, data);
                AssetDatabase.Refresh();
            }
        }
    }

    public static void GenerateTransparentPrefabPreview(GameObject prefab)
    {
        var currentScene = EditorSceneManager.GetSceneAt(0);

        return;
        EditorSceneManager.OpenScene("Assets/Scenes/AssetRendering.unity");
        string texturesFolder = Path.Combine(Application.dataPath, "Textures");
        string previewsFolder = Path.Combine(texturesFolder, "GeneratedPreviews");
        string fileName = $"{prefab.name}.png";
        string fullPath = Path.Combine(previewsFolder, fileName);
        if (File.Exists(fullPath))
        {
            Debug.Log($"Preview already exists for {prefab.name} at {fullPath}");
            return;
        }

        Debug.Log($"Generating preview for: {prefab.name}");
        AssetPreview.SetPreviewTextureCacheSize(2);

        Texture2D preview = null;
        do
        {
            preview = AssetPreview.GetAssetPreview(prefab);
        }
        while (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()));

        if (preview != null)
        {
            preview.Apply();
            byte[] data = preview.EncodeToPNG();

            if (!File.Exists(fullPath))
            {
                File.WriteAllBytes(fullPath, data);
                AssetDatabase.Refresh();
            }
        }
    }
}
