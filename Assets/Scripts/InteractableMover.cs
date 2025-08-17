using CaosCreations;
using UnityEngine;

public class InteractableMover : MonoBehaviour
{
    private Camera mainCamera;
    public LayerMask SortableLayerMask;
    public LayerMask WallLayerMask;
    public float forceMultiplier = 25;

    [Header("Set in Game")]
    private Vector3 forceToApply;
    public bool isDragging = false;
    public Interactable currentDrag;
    

    void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        GameManager.EventService.Add<GameStartedEvent>(OnGameStartedEvent);
    }

    void Update()
    {
        if (!GameManager.Instance.isGameRunning)
        {
            return;
        }

        if (!isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit rayHit, float.MaxValue, SortableLayerMask))
                {
                    var selectedObject = rayHit.collider.gameObject;
                    currentDrag = selectedObject.GetComponent<Interactable>();
                    currentDrag.OnPickup();
                    isDragging = true;
                }
            }
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // update position
            if (Physics.Raycast(ray, out RaycastHit rayHit, float.MaxValue, WallLayerMask, QueryTriggerInteraction.Collide))
            {
                var targetPosition = rayHit.point + new Vector3(0, 3.5f, 0);
                forceToApply = targetPosition - currentDrag.transform.position;
                currentDrag.targetPosition = targetPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                currentDrag.OnDrop();
                currentDrag = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGameRunning)
        {
            return;
        }

        if (isDragging)
        {
            var distanceFromTarget = Vector3.Distance(currentDrag.transform.position, currentDrag.targetPosition);
            //currentDrag.myRigidbody.linearDamping = Mathf.Lerp(0, 1, distanceFromTarget / targetDisance);
            currentDrag.myRigidbody.AddForce(forceToApply * Time.fixedDeltaTime * forceMultiplier, ForceMode.Impulse);
        }
    }

    private void OnGameStartedEvent(GameStartedEvent e)
    {
        isDragging = false;
        currentDrag = null;
    }
}
