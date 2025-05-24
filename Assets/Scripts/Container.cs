using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Container : MonoBehaviour
{
    private string sortableName;
    public string SortableName => sortableName;
    public GameObject example;

    private void Awake()
    {
        sortableName = null;
    }

    public void SetType(SortableObject sortableObject)
    {
        sortableName = sortableObject.objectName;
        gameObject.name = $"{sortableObject.objectName} container";
    }

    public void ClearType()
    {
        sortableName = null;
        gameObject.name = "Empty container";
    }
}
