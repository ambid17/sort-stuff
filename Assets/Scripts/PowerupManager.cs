using CaosCreations;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Range(0, 1)]
    public float powerupSpawnChance;
    public Transform powerupContainer;

    void Start()
    {
        GameManager.EventService.Add<GameStartedEvent>(OnGameStarted);
        GameManager.EventService.Add<GameEndedEvent>(OnGameEnded);
        GameManager.EventService.Add<ItemSortedEvent>(OnItemSorted);
    }

    void OnGameStarted(GameStartedEvent e)
    {
        // delete old powerups
        while (powerupContainer.childCount > 0)
        {
            DestroyImmediate(powerupContainer.GetChild(0).gameObject);
        }
    }

    void OnItemSorted(ItemSortedEvent e)
    {
        if (UnlockManager.Instance.IsUpgradeUnlocked(GameplayUpgradeType.RainingMoney)
            // TODO: add config for this
            && RandomChance.PercentCheck(powerupSpawnChance)
            )
        {
            StartCoroutine(SpawnMoney("Raining money"));
        }

        TrySpawnPowerUp();
    }

    private void TrySpawnPowerUp()
    {
        if (UnlockManager.Instance.powerUpSOs.Count(so => so.isUnlocked) == 0)
        {
            return;
        }

        if (RandomChance.PercentCheck(powerupSpawnChance))
        {
            var unlockedPowerups = UnlockManager.Instance.powerUpSOs.Where(so => so.isUnlocked).ToList();
            var powerUpIndex = Random.Range(0, unlockedPowerups.Count());
            var powerupGO = Instantiate(unlockedPowerups[powerUpIndex].prefab);
            powerupGO.transform.parent = powerupContainer;
        }
    }

    void OnGameEnded(GameEndedEvent e)
    {
        if (UnlockManager.Instance.IsUpgradeUnlocked(GameplayUpgradeType.GoldInjection)
            // TODO: add config for this
            && RandomChance.PercentCheck(powerupSpawnChance)
            )
        {
            StartCoroutine(SpawnMoney("Gold Injection"));
        }
    }

    private IEnumerator SpawnMoney(string text)
    {
        // TODO: currently just gives 10 random sortables an extra completion
        // drop money all over with a "gold injection" text 
        StartCoroutine(UiManager.Instance.hudPanel.ShowBonusPopup(text, 3));
        foreach (var item in GameManager.Instance.allSpawnedSortables.OrderBy(x => Random.Range(0, 1000)).Take(10))
        {
            CurrencyController.Instance.OnItemSorted( new ItemSortedEvent(item));
        }
        yield return new WaitForSeconds(2f);
    }
}
