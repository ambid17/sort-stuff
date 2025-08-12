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
        hasBeenDropped = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenDropped)
        {
            return;
        }

        foreach(var go in GameManager.Instance.allSortables)
        {
            go.myRigidbody.AddExplosionForce(10000f, transform.position, 20f);
        }

        Destroy(gameObject);
    }
}
