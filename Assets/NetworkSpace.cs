using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSpace : Clickable, ISaveable
{
  public int id = 0;
  
  public List<LocationScript> connectsLocations = new();
  public ERA myEra = ERA.BOAT;


  Vehicle myVehicle;

  // Start is called before the first frame update
  void Start()
  {
    myVehicle = null;
    BecomeUnclickable();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public bool IsOccupied()
  {
    return (myVehicle is not null);
  }

  public void SetVehicle(Vehicle vehicle)
  {
    myVehicle = vehicle;
  }

  public Vehicle GetVehicle()
  {
    return myVehicle;
  }

  public override void OnClick()
  {
    //switch (myEra)
    //{
    //  case ERA.BOAT:
    //    Debug.Log("Network boat clicked!");
    //    break;
    //  case ERA.TRAIN:
    //    Debug.Log("Network train clicked!");
    //    break;
    //  default:
    //    break;
    //}
    if (myEra != GameManager.currentEra || IsOccupied()) return;
    ObjectManager.ChoseNetwork(this);

  }

  public void DestroyMyVehicle()
  {
    if (!IsOccupied()) return;
    //Debug.Log("Destroying vehicle!");
    myVehicle.gameObject.SetActive(false);
    myVehicle = null;
  }


  public void AddVehicle(int playerIndex)
  {
    string[] playerFolders = { Constants.redTilesPath, Constants.yellowTilesPath, Constants.whiteTilesPath, Constants.purpleTilesPath };
    string[] playerUpFolders = { Constants.redUpgradedTilesPath, Constants.yellowUpgradedTilesPath, Constants.whiteUpgradedTilesPath, Constants.purpleUpgradedTilesPath };

    Object vehicleResource;

    switch (myEra)
    {
      case ERA.BOAT:
        vehicleResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.boatName);
        break;
      case ERA.TRAIN:
        vehicleResource = HelpFunctions.LoadPrefabFromFile(playerUpFolders[playerIndex] + Constants.trainName);
        break;
      default:
        return;
    }

    //Debug.Log("Creating vehicle");
    Vehicle newVehicle = (Instantiate(vehicleResource) as GameObject).GetComponent<Vehicle>();
    newVehicle.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
    SetVehicle(newVehicle);
;
  }

  public void PopulateSaveData(SaveData sd)
  {

    if (IsOccupied())
    {
      SaveData.NetworkSpaceData myData = new();
      myData.id = id;
      myData.myEra = myEra;
      myData.occupied = true;
      myData.vehicleOwnerIndex = myVehicle.ownerPlayerIndex;
      sd.networkData.Add(myData);
    }
  }

  public void LoadFromSaveData(SaveData sd)
  {
    if (IsOccupied()) DestroyMyVehicle();
    foreach(SaveData.NetworkSpaceData myData in sd.networkData)
      if (myData.id == id && myData.myEra == myEra)
        AddVehicle(myData.vehicleOwnerIndex);

  }
}

