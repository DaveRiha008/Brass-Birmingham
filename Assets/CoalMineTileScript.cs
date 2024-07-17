using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalMineTileScript : TileScript
{
  List<GameObject> myCoals = new();
  /// <summary>
  /// Creates coal and places it on the correct position on this tile
  /// </summary>
  GameObject CreateCoal()
  {
    var coalResource = HelpFunctions.LoadPrefabFromFile(Constants.coalSmallPath);

    var newCoal = Instantiate(coalResource) as GameObject;
    float CoalSizeX = newCoal.GetComponent<BoxCollider2D>().size.x * newCoal.transform.localScale.x;//For small shift, because two barrels should be visible
    float CoalSizeY = newCoal.GetComponent<BoxCollider2D>().size.y * newCoal.transform.localScale.y;//For small shift, because two barrels should be visible


    newCoal.transform.SetPositionAndRotation(builtOnSpace.transform.position +  new Vector3((-1.5f + myCoals.Count%3) * CoalSizeX, CoalSizeY*(1 - (int)(myCoals.Count/3)), 0), builtOnSpace.transform.rotation);
    return newCoal;
  }

  public void AddCoal()
  {
    if (industryType != INDUSTRY_TYPE.COALMINE)
    {
      Debug.LogError("Tried to add Coal to non-CoalWorks tile");
      return;
    }


    var newCoal = CreateCoal();
    myCoals.Add(newCoal);
  }

  public bool HasCoal() => myCoals.Count > 0;

  public int GetResourceCount() => myCoals.Count;


  public void RemoveCoal()
  {
    if (myCoals.Count <= 0)
    {
      Debug.LogError("Tried to remove nonexistent barrel");
      return;
    }
    GameObject removedCoal = myCoals[myCoals.Count - 1];
    removedCoal.SetActive(false);
    myCoals.Remove(removedCoal);
    if (myCoals.Count <= 0)
    {
      Upgrade();
    }
  }

  public override void BecomeBuilt()
  {
    alreadyBuilt = true;

    for (int i = 0; i < Constants.coalMineCoalCount[level-1]; i++)
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(builtOnSpace.myLocation).Count>0 && ObjectManager.HasCoalStorageFreeSpace())
      {
        int moneyGained = ObjectManager.AddCoalToStorage();
        GameManager.PlayerGainMoney(ownerPlayerIndex, moneyGained);
      }
      else AddCoal();
    }
    if (myCoals.Count <= 0)
      Upgrade();

  }

  public override void BecomeUnbuilt()
  {
    base.BecomeUnbuilt();

    int coalsToRemove = myCoals.Count;
    int coalsToRemoveFromStorage = Constants.coalMineCoalCount[level - 1] - coalsToRemove;

    for (int i = 0; i < coalsToRemoveFromStorage; i++)
    {
      ObjectManager.GetCoalFromStorage(out _).SetActive(false);
    }

    for (int i = 0; i < coalsToRemove; i++)
    {
      RemoveCoal();
    }

    if (isUpgraded) Downgrade();

  }

  public override void Remove()
  {
    int removeCount = myCoals.Count;

    for (int i = 0; i < removeCount; i++)
      RemoveCoal();
    base.Remove();
  }

  public override void PopulateSaveData(SaveData sd)
  {
    SaveData.TileScriptData myData = new();
    myData.id = id;
    if (builtOnSpace is not null)
      myData.builtOnSpaceID = builtOnSpace.id;
    else
      myData.builtOnSpaceID = -1;
    myData.alreadyBuilt = alreadyBuilt;
    myData.isUpgraded = isUpgraded;
    myData.isDeveloped = isDeveloped;
    myData.ownerPlayerIndex = ownerPlayerIndex;
    myData.resourceCount = myCoals.Count;

    sd.tileScriptData.Add(myData);
  }

  public override void LoadFromSaveData(SaveData sd)
  {
    int coalToRemove = myCoals.Count;
    for (int i = 0; i < coalToRemove; i++)
    {
      //Debug.Log($"Removing {i}th coal");
      RemoveCoal();
    }
    foreach (SaveData.TileScriptData data in sd.tileScriptData)
      if (data.id == id)
      {
        if (data.builtOnSpaceID != -1)
        {
          IndustrySpace dataSpace = ObjectManager.GetIndustrySpaceByID(data.builtOnSpaceID);
          transform.position = dataSpace.transform.position;
          builtOnSpace = dataSpace;
          dataSpace.myTile = this;
          dataSpace.industryTypeOfBuiltTile = industryType;
        }

        else if (builtOnSpace is not null)
        {
          builtOnSpace.RemoveBuiltIndustry();
          builtOnSpace = null;
        }
        

        for (int i = 0; i < data.resourceCount; i++)
          AddCoal();


        if (data.isUpgraded) Upgrade(false);
        else Downgrade(false);
        if (data.isDeveloped) Develop();
        else Undevelop();
        alreadyBuilt = data.alreadyBuilt;

        ownerPlayerIndex = data.ownerPlayerIndex;
      }
  }
}
