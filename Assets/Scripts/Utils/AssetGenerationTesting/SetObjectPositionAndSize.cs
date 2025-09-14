using Unity.VisualScripting;
using UnityEngine;

public class SetObjectPositionAndSize : MonoBehaviour
{
    BoxCollider boxCollider;
    void Start()
    {

        // use the mesh renderer instead of the mesh filter, since renderer bounds are in world space
        var meshRenderers = GetComponentsInChildren<Renderer>();
        Bounds bounds = meshRenderers[0].bounds;

        if (meshRenderers.Length > 1)
        {
            for (int i = 1; i < meshRenderers.Length; i++)
            {
                bounds.Encapsulate(meshRenderers[i].bounds);
            }
        }

        // add a collider to visualize the bounds
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;

        // need to scale the object down to fit within a unit cube, then move it to the origin
        // since the bounds are in world space, we need to account for the current scale of the object, thus scale needs to be applied first
        Debug.Log($"center: {bounds.center}, collider center: {boxCollider.bounds.center}, bounds center: {transform.TransformPoint(bounds.center)}");
        transform.localScale = Vector3.one / bounds.extents.magnitude;
        Debug.Log($"after scale - center: {bounds.center}, collider center: {boxCollider.bounds.center}, bounds center: {transform.TransformPoint(bounds.center)}");
        transform.position = Vector3.zero;
        transform.position -= transform.TransformPoint(bounds.center);
    }

    
}
