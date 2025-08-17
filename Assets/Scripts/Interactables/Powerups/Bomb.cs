using System.Linq;
using UnityEngine;

public class Bomb : Interactable
{
    bool hasBeenDropped;

    void Start()
    {
        hasBeenDropped = false;
    }

    public override void OnDrop()
    {
        base.OnDrop();
        hasBeenDropped = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenDropped || collision.gameObject.layer == ContainerLayer)
        {
            return;
        }

        var activeSortables = GameManager.Instance.allSpawnedSortables.Where(so => !so.areAllCollected && so.touchingContainers.Count == 0);
        foreach (var go in activeSortables)
        {
            go.myRigidbody.AddExplosionForce(10000f, transform.position, 20f);
        }

        Destroy(gameObject);
    }
}
