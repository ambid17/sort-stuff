using UnityEditor;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [MenuItem("Caos Creations/Open Save Folder _F1")]
    public static void OpenSaveFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
