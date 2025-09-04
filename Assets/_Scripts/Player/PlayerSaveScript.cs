using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerSaveScript : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void Start()
    {
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        if (InventoryManagerScript.instance == null) return;

        SaveData saveData = InventoryManagerScript.instance.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Game saved to {savePath}");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            InventoryManagerScript.instance.FromSaveData(saveData);

            Debug.Log("Game loaded from save file");
        }
        else
        {
            Debug.Log("No save file found, starting fresh.");
        }
    }
}