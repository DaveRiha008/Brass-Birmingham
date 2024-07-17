using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustrySpace : Clickable
{
  public int id = 0;

  /// <summary>
  /// Name of Location on which this space is located
  /// </summary>
  public string townName = "";

  public INDUSTRY_TYPE industryTypeOfBuiltTile = INDUSTRY_TYPE.NONE;
  /// <summary>
  /// tile built on this space
  /// </summary>
  public TileScript myTile = null;
  /// <summary>
  /// Location on which this space is located
  /// </summary>
  public LocationScript myLocation;

  public bool hasBrew = false;
  public bool hasCoal = false;
  public bool hasCot = false;
  public bool hasIron = false;
  public bool hasMan = false;
  public bool hasPot = false;
  /// <summary>
  /// All types that can be built on this space
  /// </summary>
  [HideInInspector]
  public List<INDUSTRY_TYPE> myTypes = new();



  void Start()
  {
    bool[] hasIndustries = { hasBrew, hasCoal, hasCot, hasIron, hasMan, hasPot };
    int i = 0;
    foreach (bool hasIndustry in hasIndustries)
    {
      if (hasIndustry) myTypes.Add((INDUSTRY_TYPE)i);
      i++;
    }

    myLocation = transform.GetComponentInParent<LocationScript>();
  }

  // Update is called once per frame
  void Update()
  {
    //if (myTile is not null && !myTile.isActiveAndEnabled) RemoveBuiltIndustry();
  }

  public override void OnClick()
  {
    Debug.Log("Industry space clicked");
    ObjectManager.ChooseSpace(this);
    //else Debug.Log("No item in hand!");
  }

  public bool CanBuildIndustryType(INDUSTRY_TYPE type)
  {
    bool correct_type = false;
    switch (type)
    {
      case INDUSTRY_TYPE.BREWERY:
        correct_type = hasBrew;
        break;
      case INDUSTRY_TYPE.COALMINE:
        correct_type = hasCoal;
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        correct_type = hasCot;
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        correct_type = hasIron;
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        correct_type = hasMan;
        break;
      case INDUSTRY_TYPE.POTTERY:
        correct_type = hasPot;
        break;
      default:
        break;
    }
    return correct_type;
  }

  /// <summary>
  /// Space forgets built industry, but the industry remains an active object
  /// </summary>
  public void RemoveBuiltIndustry()
  {
    industryTypeOfBuiltTile = INDUSTRY_TYPE.NONE;
    myTile = null;
  }
}
