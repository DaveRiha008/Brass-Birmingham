using System.Collections.Generic;
using UnityEngine;

public static class SaveDataManager
{
  public static void SaveJsonData()
  {
    SaveData sd = new SaveData();
    GameManager.PopulateSaveDataStatic(sd);
    CardManager.PopulateSaveDataStatic(sd);
    ObjectManager.PopulateSaveDataStatic(sd);
    //foreach (var saveable in a_Saveables)
    //{
    //  saveable.PopulateSaveData(sd);
    //}

    foreach (SaveData.PlayerData playerData in sd.playerData)
    {
      Debug.Log($"PlayerName: {playerData.name}");
    }

    if (FileManager.WriteToFile(Constants.saveFileName, sd.ToJson()))
    {
      Debug.Log($"Saved {sd.ToJson()}");
      Debug.Log("Save successful");
    }
  }

  public static void LoadJsonData()
  {
    if (FileManager.LoadFromFile(Constants.saveFileName, out var json))
    {
      Debug.Log($"Loading from JSON: {json}");

      SaveData sd = new SaveData();
      sd.LoadFromJson(json);

      GameManager.LoadGameDataStatic(sd);
      CardManager.LoadFromSaveDataStatic(sd);
      ObjectManager.LoadFromSaveDataStatic(sd);

      //foreach (var saveable in a_Saveables)
      //{
      //  saveable.LoadFromSaveData(sd);
      //}

      Debug.Log("Load complete");
    }
  }
}