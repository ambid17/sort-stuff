using UnityEngine;

public class WallController : MonoBehaviour
{
    public BoxCollider myCollider;

    private void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();
        Gizmos.color = Color.green;
        Gizmos.DrawCube(myCollider.bounds.center, myCollider.bounds.size);
    }
}
