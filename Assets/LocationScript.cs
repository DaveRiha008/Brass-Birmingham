using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationScript : MonoBehaviour
{
  public string locationName = "";
  public LocationType myType = LocationType.CITY;

  /// <summary>
  /// Neighbor locations in boat era
  /// </summary>
  public List<LocationScript> neighborsBoats = new List<LocationScript>();
  /// <summary>
  /// Neighbor locations in boat era
  /// </summary>
  public List<LocationScript> neighborsTrains = new List<LocationScript>();
  public List<MerchantTileSpace> myMerchants = new();
  // Start is called before the first frame update
  void Start()
  {
    if(myType == LocationType.MERCHANT)
    {
      //Load merchant rewards
      MerchantReward myReward = null; //Should always be intantiated in the end - Mechant must have a reward as a child
      foreach (Transform child in transform)
      {
        if (!child.TryGetComponent(out MerchantReward reward)) continue;
        //Debug.Log("Succesfully found merchant reward");
        myReward = reward;
      }
      //Load merchant spaces
      foreach (Transform child in transform)
      {
        if (!child.TryGetComponent(out MerchantTileSpace merchant)) continue;
        //Debug.Log("Adding reward to barrel space");
        myMerchants.Add(merchant);
        merchant.myReward = myReward;
        merchant.Init();
      }
    }
  }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum LocationType { CITY, MERCHANT, NAMELESS_BREWERY }
