using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class AssetScreenshot : MonoBehaviour
{
    public RenderTexture renderTexture;

    private Camera mainCamera;
    public int previewSize = 1024; // Size of the preview image

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        CreatePreview();
    }

    public void CreatePreview()
    {
        mainCamera.aspect = 1.0f;
        mainCamera.targetTexture = renderTexture;
        mainCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, previewSize, previewSize), 0, 0);
        texture.Apply();

        //RenderTexture.active = null;

        byte[] bytes;
        bytes = texture.EncodeToPNG();

        var assetName = FindFirstObjectByType<MeshRenderer>().gameObject.name;

       // WriteFile(bytes, assetName);
    }

    private void WriteFile(byte[] data, string assetName)
    {
        string texturesFolder = Path.Combine(Application.dataPath, "Textures");
        string previewsFolder = Path.Combine(texturesFolder, "NewPreviews");
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
}