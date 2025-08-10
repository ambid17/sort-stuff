using UnityEngine;

public class WallController : MonoBehaviour
{
    public BoxCollider collider;

    private void OnDrawGizmos()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
        Gizmos.color = Color.green;
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}
