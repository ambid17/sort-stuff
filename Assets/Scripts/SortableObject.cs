using UnityEngine;

[CreateAssetMenu(fileName = "SortableObject", menuName = "Scriptable Objects/SortableObject")]
public class SortableObject : ScriptableObject
{
    public int id;
    public GameObject prefab;
}
