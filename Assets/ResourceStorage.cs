using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : Clickable, ISaveable
{
  List<ResourceStorageSpace> mySpaces = new();
  public RESOURCE_TYPE myType = RESOURCE_TYPE.COAL;
  public int maxPriceIfEmpty = 6;

  void Start()
  {
    LoadAllChildrenSpaces();
  }

  void LoadAllChildrenSpaces()
  {
    foreach (Transform child in transform)
    {
      ResourceStorageSpace space = child.gameObject.GetComponent<ResourceStorageSpace>();
      mySpaces.Add(space);
    }
  }


  void Update()
  {
        
  }

  public bool IsEmpty() => CurrentPrice() == maxPriceIfEmpty;

  private int NumberOfResources()
  {
    int count = 0;
    foreach (ResourceStorageSpace space in mySpaces)
      if (space.HasResource()) count++;
    return count;
  }
  public int CurrentPrice()
  {
    foreach (ResourceStorageSpace space in mySpaces)
    {
      if (!space.HasResource()) continue;
      return space.cost;
    }
    return maxPriceIfEmpty;
  }

  public GameObject GetCheapestResource(out int price)
  {
    foreach (ResourceStorageSpace space in mySpaces)
    {
      if (!space.HasResource()) continue;
      price = space.cost;
      GameObject returnResource = space.RemoveResource();
      return returnResource;
    }
    price = maxPriceIfEmpty;
    switch (myType)
    {
      case RESOURCE_TYPE.COAL:
        return ObjectManager.CreateCoal();
      case RESOURCE_TYPE.IRON:
        return ObjectManager.CreateIron();
      default:
        throw new System.Exception("Invalid resource type");
    }
  }

  public int AddMostExpensiveResource(GameObject inputObject = null)
  {

    GameObject addedObject;
    if (inputObject is null)
    {
      switch (myType)
      {
        case RESOURCE_TYPE.COAL:
          addedObject = ObjectManager.CreateCoal();
          break;
        case RESOURCE_TYPE.IRON:
          addedObject = ObjectManager.CreateIron();
          break;
        default:
          throw new System.Exception("Invalid resource type");
      }
    }
    else addedObject = inputObject;

    if (mySpaces[0].HasResource()) return 0;

    ResourceStorageSpace lastSpace = mySpaces[0];

    for (int i = 1; i < mySpaces.Count; i++)
    {
      if (mySpaces[i].HasResource()) break;
      lastSpace = mySpaces[i];
    }
    lastSpace.SetResource(addedObject);
    return lastSpace.cost;

  }
  public void RemoveAllResources()
  {
    foreach (ResourceStorageSpace space in mySpaces)
      if(space.HasResource()) space.DestroyResource();
  }
  public bool HasFreeSpace() => !mySpaces[0].HasResource();

  public override void OnClick()
  {
    if (canBeClicked)
    {
      if (myType == RESOURCE_TYPE.COAL)
        ObjectManager.ChoseCoalStorage();
      else if (myType == RESOURCE_TYPE.IRON)
        ObjectManager.ChoseIronStorage();

    }
  }

  public void PopulateSaveData(SaveData sd)
  {
    if (myType == RESOURCE_TYPE.COAL)
      sd.coalStorageData.countInStorage = NumberOfResources();
    else if (myType == RESOURCE_TYPE.IRON)
      sd.ironStorageData.countInStorage = NumberOfResources();

  }

  public void LoadFromSaveData(SaveData sd)
  {
    RemoveAllResources();
    int newCount = 0;
    if (myType == RESOURCE_TYPE.COAL) newCount = sd.coalStorageData.countInStorage;
    else if (myType == RESOURCE_TYPE.IRON) newCount = sd.ironStorageData.countInStorage;

    for (int i = 0; i < newCount; i++)
      AddMostExpensiveResource();
  }
}

public enum RESOURCE_TYPE { COAL, IRON, BARREL, NONE };