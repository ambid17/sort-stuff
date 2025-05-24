using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<SortableObject> sortables;
    public LayerMask SortableLayerMask;
    public LayerMask WallLayerMask;
    public UiManager uiManager;
    public Container containerPrefab;
    public GameObject rightWall;
    public GameObject sortableParent;
    public float forceMultiplier = 4;
    public BoxCollider spawnArea;

    public const float CONTAINER_WIDTH = 3.33f;
    public const int MAX_CONTAINER_COUNT = 5;
    public const int MAX_COUNT_PER_TYPE = 40;

    public int TotalCount => TypeCount * CountPerType;

    [Header("Set in Game")]
    public int TypeCount = 3;
    public int CountPerType = MAX_COUNT_PER_TYPE;
    public int ContainerCount = MAX_CONTAINER_COUNT;

    public List<Sortable> allSortables;
    public int remainingCount;

    public List<Container> containers;
    private Camera mainCamera;

    private Vector3 forceToApply;
    public bool isDragging = false;
    public Sortable currentDrag;
    public bool isGameRunning = false;

    public Dictionary<string, List<Sortable>> sortedMapping;
    public List<SortableObject> unlockedSortables;

    protected override void Initialize()
    {
        sortedMapping = new Dictionary<string, List<Sortable>>();
        mainCamera = Camera.main;
        InitLevel();
    }

    void Update()
    {
        if(!isGameRunning)
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
                    currentDrag = selectedObject.GetComponent<Sortable>();
                    isDragging = true;
                }
            }
        }
        else
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // update position
            if (Physics.Raycast(ray, out RaycastHit rayHit, float.MaxValue, WallLayerMask))
            {
                var targetPosition = rayHit.point + new Vector3(0, 3.5f, 0);
                forceToApply = targetPosition - currentDrag.transform.position;
                currentDrag.myRigidbody.linearDamping = 4;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                currentDrag.myRigidbody.linearDamping = 0.5f;
                currentDrag = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isGameRunning)
        {
            return;
        }

        if (isDragging)
        {
            currentDrag.myRigidbody.AddForce(forceToApply * Time.fixedDeltaTime * forceMultiplier, ForceMode.Impulse);
        }
    }

    public bool CanSetContainer(Sortable sortable)
    {
        return !containers.Any(c => c.SortableName == sortable.sortableObject.objectName);
    }

    public void HandleContainerExit(Sortable sortable)
    {
        if (sortedMapping[sortable.sortableObject.objectName].Contains(sortable)){
            sortedMapping[sortable.sortableObject.objectName].Remove(sortable);
            remainingCount++;
            UiManager.Instance.SetRemaining();
        }
    }

    public void TryAddSorted(Sortable sortable)
    {
        var sortedList = sortedMapping[sortable.sortableObject.objectName];
        if (sortedList.Contains(sortable))
        {
            return;
        }

        sortedList.Add(sortable);
        remainingCount--;
        UiManager.Instance.SetRemaining();
        CurrencyController.Instance.SortComplete(sortable);

        // drain the container when full and allow it to be reused
        if (sortedList.Count == CountPerType)
        {
            var container = containers.FirstOrDefault(c => c.SortableName == sortable.sortableObject.objectName);
            container.ClearType();
            foreach (var toDespawn in sortedList)
            {
                toDespawn.Despawn();
            }
        }

        if (remainingCount == 0)
        {
            EndGame();
            uiManager.ShowWin();
            isGameRunning = false;
        }
    }

    public void StartGame()
    {
        // toggle on sortables that are spawned in during settings changes
        foreach (Sortable sortable in allSortables)
        {
            sortable.TogglePhysics(true);
        }

        isDragging = false;
        currentDrag = null;
        remainingCount = TotalCount;
        UiManager.Instance.SetRemaining();
        InitSortedMapping();

        foreach (var container in containers)
        {
            container.ClearType();
        }

        isGameRunning = true;
    }

    private void EndGame()
    {
        sortedMapping.Clear();
        InitLevel();
        UnlockManager.Instance.Save();
    }

    public void InitSortedMapping()
    {
        sortedMapping.Clear();
        foreach (var sortable in allSortables)
        {
            if (!sortedMapping.ContainsKey(sortable.sortableObject.objectName))
            {
                sortedMapping.Add(sortable.sortableObject.objectName, new List<Sortable>());
            }
        }
    }

    public void SetTypeCount(int count)
    {
        TypeCount = count;
        SpawnMaxSortables();
    }

    public void SetCountPerType(int count)
    {
        CountPerType = count;
        SpawnMaxSortables();
    }

    public void SetContainerCount(int count)
    {
        ContainerCount = count;

        // toggle containers
        for (int i = 0; i < MAX_CONTAINER_COUNT; i++)
        {
            containers[i].gameObject.SetActive(i < ContainerCount);
        }

        foreach (var sortable in allSortables)
        {
            sortable.UpdateSpawn();
        }
    }

    public void InitLevel()
    {
        if(containers != null && containers.Count > 0)
        {
            foreach (var container in containers)
            {
                Destroy(container.gameObject);
            }
        }
        // spawn containers
        containers = new List<Container>();
        for (int i = 0; i < MAX_CONTAINER_COUNT; i++)
        {
            var container = Instantiate(containerPrefab);
            container.transform.position = new Vector3(i * 3.33f, -0.55f, -6.6f);
            container.ClearType();
            containers.Add(container);
        }

        // spawn all sortables
        SpawnMaxSortables();
    }

    private void SpawnMaxSortables()
    {
        if (allSortables != null)
        {
            foreach (var sortable in allSortables)
            {
                Destroy(sortable.gameObject);
            }
        }

        allSortables = new List<Sortable>();

        var combinedSortables = sortables.Concat(unlockedSortables).ToList();

        var sortableNames = combinedSortables
           .OrderBy(x => Random.Range(0, 1000)) // sort randomly
           .Take(TypeCount) // take the number of types we want
           .Select(x => x.objectName)
           .ToList();

        if (sortableNames.Contains("Corndog"))
        {
            Debug.Log(":");
        }


        foreach (var sortableName in sortableNames)
        {
            for (int i = 0; i < CountPerType; i++)
            {
                var sortableSO = combinedSortables.FirstOrDefault(x => x.objectName == sortableName);
                var sortableGO = Instantiate(sortableSO.prefab);
                Sortable sortable = sortableGO.AddComponent<Sortable>();
                sortable.Setup(sortableSO);
                sortable.TogglePhysics(false);
                sortable.transform.parent = sortableParent.transform;
                allSortables.Add(sortable);
            }
        }
    }
}
