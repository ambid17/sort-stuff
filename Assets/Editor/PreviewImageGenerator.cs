using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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

        if (GUILayout.Button("Create transparent"))
        {
            GenerateTransparentPrefabPreview(Selection.activeObject as GameObject);
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
        if (prefab == null)
        {
            Debug.LogError("No prefab selected for preview generation.");
            return;
        }

        var currentScene = EditorSceneManager.GetSceneAt(0);
        if(currentScene.name != "AssetRendering")
        {
            Debug.LogWarning("Current scene is not AssetRendering. Opening AssetRendering scene.");
            EditorSceneManager.OpenScene("Assets/Scenes/AssetRendering.unity");
        }

        Debug.Log($"Generating preview for: {prefab.name}");
        
        var go = Instantiate(prefab);
        go.transform.position = Vector3.zero;
        var bounds = GetObjectBounds(go);
        go.transform.localScale = Vector3.one / bounds.extents.magnitude;
        go.transform.position -= go.transform.TransformPoint(bounds.center);


        var previewSize = 1024;
        var mainCamera = Camera.main;
        var tempRenderTexture = new RenderTexture(previewSize, previewSize, 24, RenderTextureFormat.ARGB32);
        mainCamera.targetTexture = tempRenderTexture;
        mainCamera.Render();

        RenderTexture.active = tempRenderTexture;
        Texture2D texture = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, previewSize, previewSize), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        mainCamera.targetTexture = null;
        tempRenderTexture.Release();
        DestroyImmediate(tempRenderTexture);

        byte[] bytes;
        bytes = texture.EncodeToPNG();

        var assetName = FindFirstObjectByType<MeshRenderer>().gameObject.name;

        DestroyImmediate(go);
        WriteFile(bytes, prefab.name);
        ReimportTexture(prefab.name);
    }

    private static Bounds GetObjectBounds(GameObject obj)
    {
        var meshRenderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = meshRenderers[0].bounds;

        if (meshRenderers.Length > 1)
        {
            for (int i = 1; i < meshRenderers.Length; i++)
            {
                bounds.Encapsulate(meshRenderers[i].bounds);
            }
        }

        return bounds;
    }

    public static void WriteFile(byte[] data, string assetName)
    {
        string texturesFolder = Path.Combine(Application.dataPath, "Textures");
        string previewsFolder = Path.Combine(texturesFolder, "GeneratedPreviews");
        string fileName = $"{assetName}.png";
        string fullPath = Path.Combine(previewsFolder, fileName);
        if (File.Exists(fullPath))
        {
            Debug.Log($"Preview already exists for {assetName} at {fullPath}. Deleting");
            File.Delete(fullPath);
            return;
        }

        File.WriteAllBytes(fullPath, data);
        AssetDatabase.Refresh();
    }

    public static void ReimportTexture(string assetName)
    {
        Debug.Log($"Reimporting texture as sprite for {assetName}");
        string[] previewsFolder = new string[] { "Assets/Textures/GeneratedPreviews" };
        var assets = AssetDatabase.FindAssets($"{assetName}", previewsFolder);

        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"No assets found for {assetName} in {string.Join(',',previewsFolder)}");
            return;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
        Debug.Log($"Found asset: {assets[0]} at path: {assetPath}");
        Debug.Log($"Reformatting texture at {assetPath} to sprite");
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.SaveAndReimport();

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (sprite == null)
        {
            Debug.LogError($"Failed to convert texture at {assetPath} to sprite.");
            return;
        }

        Debug.Log($"Reimported texture at {assetPath} to sprite");
    }
}
