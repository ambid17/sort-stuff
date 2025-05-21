using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sortable = other.GetComponent<Sortable>();
        if (sortable != null)
        {
            sortable.Respawn();
        }
    }
}
