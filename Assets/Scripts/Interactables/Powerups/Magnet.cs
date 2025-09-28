using CaosCreations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnet : Interactable
{
    List<Sortable> targetSortables;
    bool isDragging = false;

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGameRunning || !isDragging)
        {
            return;
        }

        foreach(var sortable in targetSortables)
        {
            var forceToApply = targetPosition - sortable.transform.position;
            sortable.myRigidbody.AddForce(forceToApply * Time.fixedDeltaTime * GameManager.Instance.interactableMover.forceMultiplier, ForceMode.Impulse);
        }
    }

    public override void OnPickup()
    {
        base.OnPickup();
        var targetName = GameManager.Instance.allSpawnedSortables
            .Where(x => !x.areAllCollected)
            .OrderBy(x => Random.Range(0, 1000))
            .Select(x=> x.sortableItem.prefab.name)
            .FirstOrDefault();

        targetSortables = GameManager.Instance.allSpawnedSortables
            .Where(sortable => sortable.touchingContainers.Count == 0 
            && sortable.sortableItem.prefab.name == targetName).ToList();
        isDragging = true;
    }

    public override void OnDrop()
    {
        base.OnDrop();
        isDragging = false;
        GameManager.EventService.Dispatch(new PowerupUsedEvent());
        Destroy(gameObject);
    }

    
}
