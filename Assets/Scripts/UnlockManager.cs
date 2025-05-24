using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    public SaveFile fileStateToSave;
    public List<Unlock> unlocks => fileStateToSave.unlocks;
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

    public bool TryUnlock(int itemId)
    {
        Item item = itemSOs.FirstOrDefault(i => i.id == itemId);
        if (item != null && fileStateToSave.currency > item.cost)
        {
            fileStateToSave.currency -= item.cost;
            fileStateToSave.unlocks.Add(new Unlock 
            { 
                itemId = itemId, 
                isUnlocked = true 
            });
            Save();
            return true;
        }

        return false;
    }

    public bool IsUnlocked(int itemId)
    {
        if(unlocks == null)
        {
            return false;
        }

        Unlock unlock = unlocks.FirstOrDefault(u => u.itemId == itemId);
        if (unlock != null)
        {
            return unlock.isUnlocked;
        }
        return false;
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
            var fileData = JsonUtility.ToJson(fileStateToSave);
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
                fileStateToSave = JsonUtility.FromJson<SaveFile>(fileContents);
            }
            else
            {
                fileStateToSave = new SaveFile
                {
                    unlocks = new List<Unlock>(),
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
}
