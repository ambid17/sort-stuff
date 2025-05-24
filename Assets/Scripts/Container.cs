using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Container : MonoBehaviour
{
    private int sortableId;
    public int SortableId => sortableId;
    public GameObject example;

    private void Awake()
    {
        sortableId = -1;
    }

    public void SetType(SortableObject sortableObject)
    {
        sortableId = sortableObject.id;
        gameObject.name = $"{sortableObject.prefab.name} container";
    }

    public void ClearType()
    {
        sortableId = -1;
        gameObject.name = "Empty container";
    }
}
