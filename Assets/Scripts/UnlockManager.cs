using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    public SaveFile fileStateToSave;
    public List<Item> itemSOs;

    void Awake()
    {
        LoadUnlocks();
    }

    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public bool TryUnlock(Item item)
    {
        if (fileStateToSave.currency > item.cost)
        {
            fileStateToSave.currency -= item.cost;
            fileStateToSave.unlockedItemNames.Add(item.name);
            GameManager.Instance.unlockedSortables.Add(item.sortableObject);
            Save();
            return true;
        }

        return false;
    }

    public bool IsUnlocked(string itemName)
    {
        if(fileStateToSave.unlockedItemNames == null)
        {
            return false;
        }

        return fileStateToSave.unlockedItemNames.Contains(itemName);
    }

    public void AddCurrency(int currency)
    {
        fileStateToSave.currency += currency;
        UiManager.Instance.currencyText.text = $"{fileStateToSave.currency}";
    }

    public void Save()
    {
        try
        {
            var fileData = JsonConvert.SerializeObject(fileStateToSave);
            string filePath = Path.Combine(Application.persistentDataPath, "unlocks.json");

            File.WriteAllText(filePath, fileData);
        }catch(Exception e)
        {
            Debug.LogError($"Error saving: {e.Message}\n{e.StackTrace}");
        }
    }

    void LoadUnlocks()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, "unlocks.json");
            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);

                fileStateToSave = JsonConvert.DeserializeObject<SaveFile>(fileContents);

                if (fileStateToSave.unlockedItemNames == null)
                {
                    fileStateToSave.unlockedItemNames = new List<string>();
                }
                else
                {
                    AddUnlocksToUse();
                    
                }
            }
            else
            {
                fileStateToSave = new SaveFile
                {
                    unlockedItemNames = new List<string>(),
                    currency = 0
                };
            }

            UiManager.Instance.currencyText.text = $"{fileStateToSave.currency}";
            ShopController.Instance.currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading: {e.Message}\n{e.StackTrace}");
        }
    }

    void AddUnlocksToUse()
    {
        if (GameManager.Instance.unlockedSortables == null)
        {
            GameManager.Instance.unlockedSortables = new List<SortableObject>();
        }

        foreach (var unlock in fileStateToSave.unlockedItemNames)
        {
            var item = itemSOs.FirstOrDefault(i => i.itemName == unlock);
            if (item != null)
            {
                GameManager.Instance.unlockedSortables.Add(item.sortableObject);
            }
        }
    }
}
