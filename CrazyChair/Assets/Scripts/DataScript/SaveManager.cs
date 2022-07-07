using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public void OnSave()
    {
        GameData.SaveObject saveObject = GameData.Instance.GetSaveObject();
        StoreController.SaveObject storeSaveObject = StoreController.Instance.GetSaveObject();

        SaveData saveData = new SaveData
        {
            dataSaveObject = saveObject,
            storeSaveObject = storeSaveObject
        };


        string json = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString("SystemSave", json);
        SaveLoadSystem.Save("SystemSave", json, false);
        //Debug.Log("Saved!");
    }

    public void OnLoad()
    {
        string json = PlayerPrefs.GetString("SystemSave");
        json = SaveLoadSystem.Load("SystemSave");

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);


        GameData.Instance.SetSaveObject(saveData.dataSaveObject);
        StoreController.Instance.SetSaveObject(saveData.storeSaveObject);
        //Debug.Log("Load!");
    }

}

[System.Serializable]
public class SaveData
{
    public GameData.SaveObject dataSaveObject = null;
    public StoreController.SaveObject storeSaveObject = null;
}