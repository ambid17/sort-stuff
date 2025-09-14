using UnityEngine;

public class SetObjectPositionAndSize : MonoBehaviour
{
    void Start()
    {

        var meshFilter = GetComponent<MeshFilter>();
        var bounds = meshFilter.mesh.bounds;
        var objectCenter = bounds.center;

        transform.position = Vector3.zero;
        transform.position -= bounds.center * 2;
        transform.localScale = Vector3.one / bounds.extents.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
