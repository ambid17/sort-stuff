using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public List<SortableObject> sortables;

    void Start()
    {
        //for (int i = 0; i < sortables.Count; i++)
        //{
        //    var sortableSO = sortables.FirstOrDefault(x => x.objectName == i);
        //    var sortableGO = Instantiate(sortableSO.prefab);
        //    Sortable sortable = sortableGO.AddComponent<Sortable>();
        //    sortable.Setup(sortableSO);
        //    sortableGO.transform.position = new Vector3(i*2, 0, 0);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
