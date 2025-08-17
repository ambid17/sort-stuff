using UnityEngine;

public class Interactable : MonoBehaviour
{
    public readonly int RaycastLayer = 7;
    public Rigidbody myRigidbody;
    public Collider myCollider;
    protected Vector3 defaultSize;
    public Vector3 targetPosition;
    protected bool isMoving;
    private int touchingContainerCount;
    public readonly int ContainerLayer = 8;

    private void Awake()
    {
        if(myRigidbody == null || myCollider == null)
        {
            Init();
        }
    }

    protected virtual void Init()
    {
        transform.position = GetSpawnPoint();

        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.linearDamping = 0.5f;
        myRigidbody.angularDamping = 0.3f;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        gameObject.layer = RaycastLayer;

        myCollider = gameObject.GetComponent<Collider>();
        if(myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider>();
        }

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        var bounds = meshFilter.mesh.bounds;
        defaultSize = Vector3.one / bounds.extents.magnitude;
        transform.localScale = defaultSize;
    }

    private void Update()
    {
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
    }

    protected virtual void HandleStop()
    {
        // Override in subclasses if needed
        var isTouchingContainers = touchingContainerCount >= 1;
        if (isTouchingContainers)
        {
            Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == ContainerLayer)
        {
            touchingContainerCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == ContainerLayer)
        {
            touchingContainerCount--;
        }
    }

    public virtual void Respawn()
    {
        transform.position = GetSpawnPoint();
    }

    public virtual void OnPickup()
    {
        myRigidbody.linearDamping = 4;
    }

    public virtual void OnDrop()
    {
        myRigidbody.linearDamping = 0.5f;
    }

    protected Vector3 GetSpawnPoint()
    {
        var min = GameManager.Instance.spawnArea.bounds.min;
        var max = GameManager.Instance.spawnArea.bounds.max;

        var x = Random.Range(min.x, max.x);
        var y = Random.Range(min.y, max.y);
        var z = Random.Range(min.z, max.z);

        return new Vector3(x, y, z);
    }
}
