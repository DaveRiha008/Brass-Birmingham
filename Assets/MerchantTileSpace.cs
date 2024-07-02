using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantTileSpace : MonoBehaviour, ISaveable
{
  public int id = -1;

  public int minPlayersToOpen = 1;
  public MerchantTileScript myTile = null;
  public BarrelSpace myBarrelSpace;
  public MerchantReward myReward;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
        
  }

  public void Init()
  {
    foreach (Transform child in transform)
    {
      if (!child.TryGetComponent(out BarrelSpace barrel)) continue;
      myBarrelSpace = barrel;
      barrel.myReward = myReward;
    }
  }

  public void PopulateSaveData(SaveData sd)
  {
    SaveData.MerchantTileData myData = new();
    myData.id = id;
    myData.myTileID = myTile.id;
    myData.hasBarrel = myBarrelSpace.hasBarrel;

    sd.merchantData.Add(myData);
  }

  public void LoadFromSaveData(SaveData sd)
  {
    if (myBarrelSpace.hasBarrel) myBarrelSpace.RemoveBarrel();
    foreach(SaveData.MerchantTileData myData in sd.merchantData)
      if(myData.id == id)
      {
        myTile = ObjectManager.GetMerchantTileScriptByID(myData.myTileID);
        myTile.transform.position = transform.position;
        if (myData.hasBarrel) myBarrelSpace.AddBarrel();
      }
  }

}
