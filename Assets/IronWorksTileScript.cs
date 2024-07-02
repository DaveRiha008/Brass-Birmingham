using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronWorksTileScript : TileScript
{
  List<GameObject> myIrons = new();
  GameObject CreateIron()
  {
    if(builtOnSpace is null || builtOnSpace == null) //Double check since it happened :)
    {
      Debug.Log("Can't add iron to tile without space");
      return null;
    }
    var ironResource = HelpFunctions.LoadPrefabFromFile(Constants.ironSmallPath);

    var newIron = Instantiate(ironResource) as GameObject;
    float ironSizeX = newIron.GetComponent<BoxCollider2D>().size.x * newIron.transform.localScale.x;//For small shift, because two barrels should be visible
    float ironSizeY = newIron.GetComponent<BoxCollider2D>().size.y * newIron.transform.localScale.y;//For small shift, because two barrels should be visible


    newIron.transform.SetPositionAndRotation(builtOnSpace.transform.position +  new Vector3((-1.5f + myIrons.Count%3) * ironSizeX, ironSizeY*(1 - (int)(myIrons.Count/3)), 0), builtOnSpace.transform.rotation);
    return newIron;
  }

  public void AddIron()
  {
    if (industryType != INDUSTRY_TYPE.IRONWORKS)
    {
      Debug.LogError("Tried to add Iron to non-IronWorks tile");
      return;
    }

    if (myIrons.Count <= 0 && isUpgraded) Downgrade();

    var newIron = CreateIron();
    myIrons.Add(newIron);
  }

  public bool HasIron() => myIrons.Count > 0;

  public void RemoveIron()
  {
    if (myIrons.Count <= 0)
    {
      Debug.LogError("Tried to remove nonexistent iron");
      return;
    }
    GameObject removedIron = myIrons[myIrons.Count - 1];
    removedIron.SetActive(false);
    myIrons.Remove(removedIron);
    if (myIrons.Count <= 0)
    {
      Upgrade();
    }
  }

  public override void BecomeBuilt()
  {
    alreadyBuilt = true;

    if (industryType == INDUSTRY_TYPE.IRONWORKS)
    {
      //Debug.Log("Creating barrel on built brewery");

      for (int i = 0; i < Constants.ironWorksIronCount[level-1]; i++)
      {
        if (ObjectManager.HasIronStorageSpace())
        {
          int moneyGained = ObjectManager.AddIronToStorage();
          GameManager.PlayerGainMoney(ownerPlayerIndex, moneyGained);
        }
        else AddIron();
      }
      if (myIrons.Count <= 0)
        Upgrade();
    }
    //BecomeUnclickable();
  }

  public override void Remove()
  {
    for (int i = 0; i < myIrons.Count; i++)
      RemoveIron();
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
    myData.resourceCount = myIrons.Count;

    sd.tileScriptData.Add(myData);
  }

  public override void LoadFromSaveData(SaveData sd)
  {
    int ironToRemove = myIrons.Count;
    for (int i = 0; i < ironToRemove; i++)
    {
      //Debug.Log($"Removing {i}th iron");
      RemoveIron();
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
          dataSpace.builtIndustry = industryType;
        }

        for (int i = 0; i < data.resourceCount; i++)
        {
          //Debug.Log($"Adding {i}th iron");
          AddIron();
        }
        if (data.isUpgraded) Upgrade(false);
        else Downgrade(false);
        if (data.isDeveloped) Develop();
        else Undevelop();
        alreadyBuilt = data.alreadyBuilt;

        ownerPlayerIndex = data.ownerPlayerIndex;
      }
  }
}
