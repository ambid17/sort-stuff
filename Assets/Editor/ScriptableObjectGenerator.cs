using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class ScriptableObjectGenerator : EditorWindow
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private int cost;
    [SerializeField] private string costInput;
    [SerializeField] private string outputFolder = "Assets/ScriptableObjects/ShopItems/Items";
    [SerializeField] private bool generatePreviews = true;
    private SerializedObject serializedObject;

    [MenuItem("Tools/ScriptableObjectGenerator")]
    private static void GeneratePreview()
    {
        GetWindow<ScriptableObjectGenerator>();
    }

    private void OnGUI()
    {
        ScriptableObject scriptableObj = this;
        SerializedObject serialObj = new SerializedObject(scriptableObj);
        SerializedProperty serialProp = serialObj.FindProperty("prefabs");

        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        generatePreviews = EditorGUILayout.Toggle("Generate Previews", generatePreviews);

        costInput = EditorGUILayout.TextField("Cost", costInput);
        if (int.TryParse(costInput, out int parsedCost))
        {
            cost = parsedCost;
        }

        if (GUILayout.Button("Create"))
        {
            foreach (var prefab in prefabs)
            {
                Debug.Log($"Generating ScriptableObject for {prefab.name} with cost {cost}");
                GenerateSo(prefab, cost);
            }
        }
    }

    private void GenerateSo(GameObject prefab, int cost)
    {
        if (generatePreviews)
        {
            PreviewImageGenerator.GeneratePrefabPreview(prefab);
        }

        var existingScriptableObjects = AssetDatabase.FindAssets($"{prefab.name}", new string[] { outputFolder });
        if (existingScriptableObjects != null && existingScriptableObjects.Length > 0)
        {
            Debug.LogWarning($"ScriptableObject already exists for {prefab.name} in {outputFolder}. Skipping creation.");
            return;
        }

        var scriptableObject = CreateInstance<SortableItem>();
        scriptableObject.cost = cost;
        scriptableObject.name = prefab.name;
        scriptableObject.itemName = prefab.name;
        scriptableObject.prefab = prefab;

        if (generatePreviews)
        {
            var itemIcon = GetSpriteFromAssets(prefab);

            if (itemIcon == null)
            {
                Debug.LogWarning($"No sprite found for {prefab.name}.");
                return;
            }

            scriptableObject.icon = itemIcon;
        }

        AssetDatabase.CreateAsset(scriptableObject, $"{outputFolder}/{prefab.name}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private Sprite GetSpriteFromAssets(GameObject prefab)
    {
        string[] previewsFolder = new string[] { "Assets/Textures/GeneratedPreviews" };
        var assets = AssetDatabase.FindAssets($"{prefab.name}", previewsFolder);

        if(assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"No assets found for {prefab.name} in {previewsFolder}");
            return null;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
        Debug.Log($"Found asset: {assets[0]} at path: {assetPath}");

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (sprite != null)
        {
            Debug.Log($"Loaded sprite for {prefab.name} at {assetPath}");
            return sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite not found for {prefab.name} at {assetPath}. Trying to find a texture, and convert to a sprite");
            TrySpriteReformat(assetPath);
        }

        return null;
    }

    private Sprite TrySpriteReformat(string assetPath)
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);

        Debug.Log($"Reformatting texture at {assetPath} to sprite");
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.SaveAndReimport();

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if(sprite == null)
        {
            Debug.LogError($"Failed to convert texture at {assetPath} to sprite.");
            return null;
        }

        Debug.Log($"Reimported texture at {assetPath} to sprite");
        return sprite;
    }

}
