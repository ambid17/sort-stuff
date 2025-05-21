using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int sortableId;
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
}
