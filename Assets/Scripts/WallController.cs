using UnityEngine;

public class WallController : MonoBehaviour
{
    public BoxCollider myCollider;

    private void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 1, 0 , 0.5f);
        Gizmos.DrawCube(myCollider.bounds.center, myCollider.bounds.size);
    }
}
