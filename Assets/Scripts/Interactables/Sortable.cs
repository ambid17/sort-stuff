using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum SortableType
{
    None,
    Cylinder,
    Sphere,
    Cube
}

public class Sortable : Interactable
{
    public SortableObject sortableObject;
    public readonly int IgnoreLayer = 2;
    public Container myContainer;

    public bool isMoving;
    private Vector3 shrunkSize => defaultSize * 0.3f;
    public HashSet<string> touchingContainers;
    private Scaling scalingStatus;
    public bool areAllCollected = false;
    enum Scaling
    {
        None, Growing, Shrinking
    }

    private void Awake()
    {
        touchingContainers = new HashSet<string>();
    }

    public void Setup(SortableObject sortableObject)
    {
        this.sortableObject = sortableObject;
        if (myRigidbody == null || myCollider == null)
        {
            Init();
        }
        TogglePhysics(false);
    }

    public void UpdateSpawn()
    {
        transform.position = GetSpawnPoint();
    }

    public void TogglePhysics(bool enabled)
    {
        myRigidbody.isKinematic = !enabled;
        myRigidbody.useGravity = enabled;
    }

    private void Update()
    {
        if (areAllCollected)
        {
            HandleShrinkToNothing();
            return;
        }

        if (myRigidbody.linearVelocity.magnitude > 0.1f || myRigidbody.angularVelocity.magnitude > 0.1f)
        {
            isMoving = true;
        }
        else
        {
            if (isMoving)
            {
                HandleStop();
            }
            isMoving = false;
        }
        
        HandleScale();
    }

    private void HandleScale()
    {
        if(scalingStatus == Scaling.Growing)
        {
            if (transform.localScale.magnitude < defaultSize.magnitude)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, defaultSize, Time.deltaTime);
            }else {
                scalingStatus = Scaling.None;
            }
        }
        else if(scalingStatus == Scaling.Shrinking)
        {
            if (transform.localScale.magnitude > shrunkSize.magnitude)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, shrunkSize, Time.deltaTime);
            }
            else
            {
                scalingStatus = Scaling.None;
            }
        }
    }

    private void HandleShrinkToNothing()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);
    }

    public override void Respawn()
    {
        base.Respawn();
        touchingContainers = new HashSet<string>();
    }

    public void Despawn()
    {
        Destroy(myRigidbody);
        Destroy(gameObject.GetComponent<MeshCollider>());
        areAllCollected = true;
    }

    private void HandleStop()
    {
        // if you're only touching 1 correct container when you stop, you're good
        if (touchingContainers.Count == 1 && touchingContainers.Contains(sortableObject.objectName))
        {
            GameManager.Instance.TryAddSorted(this);
        }
        else
        {
            var touchingMultipleContainers = touchingContainers.Count > 1;
            var touchingOneWrongContainer = touchingContainers.Count == 1
                && !touchingContainers.Contains(sortableObject.objectName);
            if(touchingMultipleContainers || touchingOneWrongContainer)
            {
                Respawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter 2");
        if (other.gameObject.layer != ContainerLayer)
        {
            return;
        }

        var otherContainer = other.gameObject.GetComponentInParent<Container>();

        if (GameManager.Instance.CanSetContainer(this) && otherContainer.SortableName == null)
        {
            otherContainer.SetType(sortableObject);
        }

        touchingContainers.Add(otherContainer.SortableName);

        // Bail if we already have a container
        if (myContainer != null)
        {
            return;
        }

        if (otherContainer.SortableName == sortableObject.objectName)
        {
            myContainer = otherContainer;
            gameObject.layer = IgnoreLayer;
            scalingStatus = Scaling.Shrinking;
            Debug.DrawLine(transform.position, otherContainer.transform.position, Color.green, 2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer != ContainerLayer)
        {
            return;
        }

        var otherContainer = other.gameObject.GetComponentInParent<Container>();
        touchingContainers.Remove(otherContainer.SortableName);

        if (myContainer == null || otherContainer.SortableName != sortableObject.objectName)
        {
            return;
        }

        gameObject.layer = RaycastLayer;
        gameObject.transform.localScale = defaultSize;
        GameManager.Instance.HandleContainerExit(this);
        scalingStatus = Scaling.Growing;
        Debug.DrawLine(transform.position, otherContainer.transform.position, Color.red, 2f);
        Debug.Log("Exiting container");

        // If we exit the container we were in, and that would make the container empty, clear it
        if (GameManager.Instance.sortedMapping[sortableObject.objectName].Count == 0)
        {
            myContainer.ClearType();
        }
        myContainer = null;
    }
}
