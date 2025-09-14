using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class AssetScreenshot : MonoBehaviour
{
    public RenderTexture renderTexture;
    public GameObject prefabToTest;

    private Camera mainCamera;
    public int previewSize = 1024; // Size of the preview image

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        CreatePreview();
        SpawnTestObject();
    }

    public void CreatePreview()
    {
        mainCamera.aspect = 1.0f;
        mainCamera.targetTexture = renderTexture;
        mainCamera.Render();

        RenderTexture.active = renderTexture;
    }

    private void SpawnTestObject()
    {
        var prefabInstance = Instantiate(prefabToTest);
        prefabInstance.transform.position = Vector3.zero;
        var bounds = GetObjectBounds(prefabInstance);
        prefabInstance.transform.localScale = Vector3.one / bounds.extents.magnitude;
        prefabInstance.transform.position -= prefabInstance.transform.TransformPoint(bounds.center);
    }

    private Bounds GetObjectBounds(GameObject obj)
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
}