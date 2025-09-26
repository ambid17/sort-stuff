using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using EventService = Utils.EventService;
using CaosCreations;

public class GameManager : Singleton<GameManager>
{
    public List<SortableItem> defaultSortableItems;
    
    public UiManager uiManager;
    public Container containerPrefab;
    public GameObject containerSlotCoverPrefab;
    public GameObject rightWall;
    public GameObject sortableParent;
    public BoxCollider spawnArea;
    [Range(0, 1)]
    public float powerupSpawnChance;
    public Transform powerupContainer;
    public InteractableMover interactableMover;

    public const float CONTAINER_WIDTH = 3.33f;
    public const int MAX_CONTAINER_COUNT = 5;
    public const int MAX_COUNT_PER_TYPE = 40;

    public int TotalCount => TypeCount * CountPerType;

    [Header("Set in Game")]
    public int TypeCount = 3;
    public int CountPerType = MAX_COUNT_PER_TYPE;
    public int ContainerCount = MAX_CONTAINER_COUNT;

    public List<Sortable> allSpawnedSortables;
    public int remainingCount;

    public List<Container> containers;
    public List<GameObject> containerSlotCovers;
    
    public bool isGameRunning = false;

    public Dictionary<string, List<Sortable>> sortedMapping;

    private EventService _eventService;
    public static EventService EventService
    {
        get
        {
            if (Instance._eventService == null)
            {
                Instance._eventService = new EventService();
            }

            return Instance._eventService;
        }
    }

    protected override void Initialize()
    {
        sortedMapping = new Dictionary<string, List<Sortable>>();
        InitLevel();
    }

    public bool CanSetContainer(Sortable sortable)
    {
        return !containers.Any(c => c.SortableName == sortable.sortableItem.prefab.name);
    }

    public void HandleContainerExit(Sortable sortable)
    {
        if (sortedMapping[sortable.sortableItem.prefab.name].Contains(sortable))
        {
            sortedMapping[sortable.sortableItem.prefab.name].Remove(sortable);
            remainingCount++;
        }
    }

    public void TryAddSorted(Sortable sortable)
    {
        var sortedList = sortedMapping[sortable.sortableItem.prefab.name];
        if (sortedList.Contains(sortable))
        {
            return;
        }

        if (UnlockManager.Instance.IsUpgradeUnlocked(GameplayUpgradeType.RainingMoney))
        {
            StartCoroutine(TryRainMoney());
        }

        TrySpawnPowerUp();

        sortedList.Add(sortable);
        remainingCount--;
        CurrencyController.Instance.SortComplete(sortable);

        // drain the container when full and allow it to be reused
        if (sortedList.Count == CountPerType)
        {
            var container = containers.FirstOrDefault(c => c.SortableName == sortable.sortableItem.prefab.name);
            container.ClearType();
            foreach (var toDespawn in sortedList)
            {
                toDespawn.Despawn();
            }
        }

        if (remainingCount == 0)
        {
            StartCoroutine(EndCompletedRun());
        }
    }

    private void TrySpawnPowerUp()
    {
        if(UnlockManager.Instance.powerUpSOs.Count(so => so.isUnlocked) == 0)
        {
            return;
        }

        var random = Random.Range(0, 1f);
        if(random < powerupSpawnChance)
        {
            var unlockedPowerups = UnlockManager.Instance.powerUpSOs.Where(so => so.isUnlocked).ToList();
            var powerUpIndex = Random.Range(0, unlockedPowerups.Count());
            var powerupGO = Instantiate(unlockedPowerups[powerUpIndex].prefab);
            powerupGO.transform.parent = powerupContainer;
        }
    }

    public void StartGame()
    {
        // toggle on sortables that are spawned in during settings changes
        foreach (Sortable sortable in allSpawnedSortables)
        {
            sortable.TogglePhysics(true);
        }

        // delete old powerups
        while (powerupContainer.childCount > 0)
        {
            DestroyImmediate(powerupContainer.GetChild(0).gameObject);
        }

        EventService.Dispatch(new GameStartedEvent());
        remainingCount = TotalCount;
        InitSortedMapping();

        foreach (var container in containers)
        {
            container.ClearType();
        }

        isGameRunning = true;
    }

    private IEnumerator EndCompletedRun()
    {
        if (UnlockManager.Instance.IsUpgradeUnlocked(GameplayUpgradeType.GoldInjection))
        {
            var random = Random.Range(0, 100);
            if (random < 10)
            {
                yield return StartCoroutine(SpawnMoney("Gold Injection"));
            }
        }
        EndGame();
        uiManager.ShowPanel(UiPanelType.Win);
    }

    private IEnumerator TryRainMoney()
    {
        var random = Random.Range(0, 100);
        if (random < 10)
        {
            // TODO: drop some money above the sortable and show "raining money" text
            yield return StartCoroutine(SpawnMoney("Raining money"));
        }
    }

    private IEnumerator SpawnMoney(string text)
    {
        // TODO: currently just gives 10 random sortables an extra completion
        // drop money all over with a "gold injection" text 
        StartCoroutine(UiManager.Instance.hudPanel.ShowBonusPopup(text, 3));
        foreach (var item in allSpawnedSortables.OrderBy(x => Random.Range(0, 1000)).Take(10))
        {
            CurrencyController.Instance.SortComplete(item);
        }
        yield return new WaitForSeconds(2f);
    }

    public void EndGame()
    {
        sortedMapping.Clear();
        InitLevel();
        UnlockManager.Instance.Save();
        isGameRunning = false;
    }

    public void InitSortedMapping()
    {
        sortedMapping.Clear();
        foreach (var sortable in allSpawnedSortables)
        {
            if (!sortedMapping.ContainsKey(sortable.sortableItem.prefab.name))
            {
                sortedMapping.Add(sortable.sortableItem.prefab.name, new List<Sortable>());
            }
        }
    }

    public void SetTypeCount(int count)
    {
        TypeCount = count + 5;
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
            containerSlotCovers[i].SetActive(i >= ContainerCount);
        }

        foreach (var sortable in allSpawnedSortables)
        {
            sortable.UpdateSpawn();
        }
    }

    public void InitLevel()
    {
        if (containers != null && containers.Count > 0)
        {
            foreach (var container in containers)
            {
                Destroy(container.gameObject);
            }
        }
        // spawn containers
        containers = new List<Container>();
        containerSlotCovers = new List<GameObject>();
        for (int i = 0; i < MAX_CONTAINER_COUNT; i++)
        {
            var container = Instantiate(containerPrefab);
            container.transform.position = new Vector3(i * 3.685f, -2f, -6.6f);
            container.ClearType();
            containers.Add(container);

            var containerSlotCover = Instantiate(containerSlotCoverPrefab);
            containerSlotCover.SetActive(false);
            containerSlotCover.transform.position = new Vector3(i * 3.685f, -0.5f, -6.6f);
            containerSlotCovers.Add(containerSlotCover);
        }

        // spawn all sortables
        SpawnMaxSortables();
    }

    private void SpawnMaxSortables()
    {
        if (allSpawnedSortables != null)
        {
            foreach (var sortable in allSpawnedSortables)
            {
                Destroy(sortable.gameObject);
            }
        }

        allSpawnedSortables = new List<Sortable>();

        var unlockedSortables = UnlockManager.Instance.itemSOs.Where(i => i.isUnlocked).ToList();

        var combinedSortables = defaultSortableItems.Concat(unlockedSortables).ToList();

        var sortableNames = combinedSortables
           .OrderBy(x => Random.Range(0, 1000)) // sort randomly
           .Take(TypeCount) // take the number of types we want
           .Select(x => x.itemName)
           .ToList();

        foreach (var sortableName in sortableNames)
        {
            for (int i = 0; i < CountPerType; i++)
            {
                var sortableSO = combinedSortables.FirstOrDefault(x => x.itemName == sortableName);
                var sortableGO = Instantiate(sortableSO.prefab);
                sortableGO.name += $"{i}";
                Sortable sortable = sortableGO.AddComponent<Sortable>();
                sortable.Setup(sortableSO);
                sortable.transform.parent = sortableParent.transform;
                allSpawnedSortables.Add(sortable);
            }
        }
    }
}
