using UnityEngine;

[CreateAssetMenu(fileName = "SortableObject", menuName = "Scriptable Objects/SortableObject")]
public class SortableObject : ScriptableObject
{
    public string objectName;
    public GameObject prefab;
}
