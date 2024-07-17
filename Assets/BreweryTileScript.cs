using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreweryTileScript : TileScript
{
  List<GameObject> myBarrels = new();
  /// <summary>
  /// Creates barrel and places it on the correct position on this tile
  /// </summary>
  GameObject CreateBarrel()
  {
    var barrelResource = HelpFunctions.LoadPrefabFromFile(Constants.barrelPath);

    var newBarrel = Instantiate(barrelResource) as GameObject;
    float barrelSizeX = newBarrel.GetComponent<BoxCollider2D>().size.x * newBarrel.transform.localScale.x;//For small shift, because two barrels should be visible

    newBarrel.transform.SetPositionAndRotation(builtOnSpace.transform.position + (-1 + 2 * myBarrels.Count) * new Vector3(barrelSizeX / 2, 0, 0), builtOnSpace.transform.rotation);
    return newBarrel;
  }

  public void AddBarrel()
  {
    if (industryType != INDUSTRY_TYPE.BREWERY) return;

    var newBarrel = CreateBarrel();
    myBarrels.Add(newBarrel);
  }

  public bool HasBarrel() => myBarrels.Count > 0;
  public int GetResourceCount() => myBarrels.Count;


  public void RemoveBarrel()
  {
    if (myBarrels.Count <= 0)
    {
      Debug.LogError("Tried to remove nonexistent barrel");
      return;
    }
    GameObject removedBarrel = myBarrels[myBarrels.Count - 1];
    removedBarrel.SetActive(false);
    myBarrels.Remove(removedBarrel);
    if (myBarrels.Count <= 0)
    {
      Upgrade();
    }
  }

  public override void BecomeBuilt()
  {
    alreadyBuilt = true;

    if (industryType == INDUSTRY_TYPE.BREWERY)
    {
      //Debug.Log("Creating barrel on built brewery");

      if (GameManager.currentEra == ERA.TRAIN && level > 1)
      {
        AddBarrel();
      }
      AddBarrel();
    }
    //BecomeUnclickable();
  }

  public override void BecomeUnbuilt()
  {
    base.BecomeUnbuilt();

    int barrelsToRemove = myBarrels.Count;


    for (int i = 0; i < barrelsToRemove; i++)
    {
      RemoveBarrel();
    }

  }

  public override void Remove()
  {
    int removeCount = myBarrels.Count;
    for (int i = 0; i < removeCount; i++)
      RemoveBarrel();


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
    myData.resourceCount = myBarrels.Count;

    sd.tileScriptData.Add(myData);
  }

  public override void LoadFromSaveData(SaveData sd)
  {
    //Debug.Log("BarrelTileScript load called");
    int barrelsToRemove = myBarrels.Count;
    for (int i = 0; i < barrelsToRemove; i++)
    {
      //Debug.Log($"Removing {i}th barrel");
      RemoveBarrel();
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
          AddBarrel();

        if (data.isUpgraded) Upgrade(false);
        else Downgrade(false);
        if (data.isDeveloped) Develop();
        else Undevelop();
        alreadyBuilt = data.alreadyBuilt;

        ownerPlayerIndex = data.ownerPlayerIndex;
      }
  }
}
