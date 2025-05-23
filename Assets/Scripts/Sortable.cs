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

public class Sortable : MonoBehaviour
{
    public SortableObject sortableObject;

    public readonly int IgnoreLayer = 2;
    public readonly int RaycastLayer = 7;
    public readonly int ContainerLayer = 8;
    public Rigidbody myRigidbody;
    public MeshCollider collider;
    private GameManager gameManager;
    public Container myContainer;

    public bool isMoving;
    private Vector3 defaultSize;
    private Vector3 shrunkSize => defaultSize * 0.3f;
    public HashSet<string> touchingContainers;
    private Scaling scalingStatus;
    private bool isCollected = false;
    enum Scaling
    {
        None, Growing, Shrinking
    }

    private void Awake()
    {
        gameManager = GameManager.Instance;
        touchingContainers = new HashSet<string>();
    }

    public void Setup(SortableObject sortableObject)
    {
        this.sortableObject = sortableObject;
        transform.position = GetSpawnPoint();

        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.linearDamping = 0.5f;
        myRigidbody.angularDamping = 0.3f;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        gameObject.layer = RaycastLayer;

        collider = gameObject.AddComponent<MeshCollider>();
        collider.convex = true;

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        var bounds = meshFilter.mesh.bounds;
        defaultSize = Vector3.one / bounds.extents.magnitude;
        transform.localScale = defaultSize;
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
        if (isCollected)
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

    public void Respawn()
    {
        transform.position = GetSpawnPoint();
        touchingContainers = new HashSet<string>();
    }

    public void Despawn()
    {
        Destroy(myRigidbody);
        Destroy(gameObject.GetComponent<MeshCollider>());
        isCollected = true;
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
        if(other.gameObject.layer != ContainerLayer)
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

        if (myContainer == null)
        {
            return;
        }

        if (otherContainer.SortableName == sortableObject.objectName)
        {
            myContainer.ClearType();
            myContainer = null;
            gameObject.layer = RaycastLayer;
            gameObject.transform.localScale = defaultSize;
            gameManager.HandleContainerExit(this);
            scalingStatus = Scaling.Growing;
            Debug.DrawLine(transform.position, otherContainer.transform.position, Color.red, 2f);
            Debug.Log("Exiting container");
        }
    }

    private Vector3 GetSpawnPoint()
    {
        var min = GameManager.Instance.spawnArea.bounds.min;
        var max = GameManager.Instance.spawnArea.bounds.max;

        var x = Random.Range(min.x, max.x);
        var y = Random.Range(min.y, max.y);
        var z = Random.Range(min.z, max.z);

        return new Vector3(x, y, z);
    }
}
