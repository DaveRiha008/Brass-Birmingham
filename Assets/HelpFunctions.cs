using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpFunctions
{
  static public List<T> GetShuffledList<T>(List<T> inputList)
  {
    List<T> inputCopy = new(inputList);
    List<T> tempList = new();
    for (int i = 0; i < inputList.Count; i++)
    {
      int randInt = Random.Range(0, inputCopy.Count);
      tempList.Add(inputCopy[randInt]);
      inputCopy.RemoveAt(randInt);
    }
    return tempList;
  }

  public static UnityEngine.Object LoadPrefabFromFile(string filename)
  {
    //Debug.Log("Trying to load LevelPrefab from file (" + filename + ")...");
    var loadedObject = Resources.Load(filename);
    if (loadedObject == null)
    {
      throw new System.Exception("...no file found - please check the configuration");
    }
    return loadedObject;
  }
}
