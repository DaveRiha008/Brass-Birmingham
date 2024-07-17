using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpFunctions
{

  /// <summary>
  /// Shuffles the given list randomly and return shuffled copy
  /// </summary>
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

  static public void HUDInfoShowMessage(INFO_MESSAGE mess)
  {
    MainInfoLabel info = GameObject.Find(Constants.HUDPath).transform.Find(Constants.mainInfoLabelPath).GetComponent<MainInfoLabel>();
    switch (mess)
    {
      case INFO_MESSAGE.PLAYER_CHANGED:
        info.PlayerChanged();
        break;
      case INFO_MESSAGE.ACTION_CANCELED:
        info.ActionCanceled();
        break;
      case INFO_MESSAGE.BUILD_SUCCESS:
        info.SuccesfulBuild();
        break;
      case INFO_MESSAGE.SELL_SUCCESS:
        info.SuccesfulSell();
        break;
      case INFO_MESSAGE.LOAN_SUCCESS:
        info.SuccesfulLoan();
        break;
      case INFO_MESSAGE.SCOUT_SUCCESS:
        info.SuccesfulScout();
        break;
      case INFO_MESSAGE.DEVELOP_SUCCESS:
        info.SuccesfulDevelop();
        break;
      case INFO_MESSAGE.NETWORK_SUCCESS:
        info.SuccesfulNetwork();
        break;
      default:
        break;
    }
  }

  static public void HUDProblemUpdate()
  {
    CurrentProblemLabelScript problemLabel = GameObject.Find(Constants.HUDPath).transform.Find(Constants.problemLabelName).GetComponent<CurrentProblemLabelScript>();
    problemLabel.ShowUpdate();
  }
}

public enum INFO_MESSAGE { PLAYER_CHANGED, ACTION_CANCELED, BUILD_SUCCESS, SELL_SUCCESS, LOAN_SUCCESS, SCOUT_SUCCESS, DEVELOP_SUCCESS, NETWORK_SUCCESS }