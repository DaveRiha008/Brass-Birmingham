using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpace : Clickable
{
  public bool hasBarrel = false;
  public GameObject barrelObject = null;
  public MerchantReward myReward = null;
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
        
  }

  public override void OnClick()
  {
    if(canBeClicked)
    ObjectManager.BarrelSpaceClicked(this);
  }
  static public GameObject CreateBarrel()
  {
    var barrelResource = HelpFunctions.LoadPrefabFromFile(Constants.barrelPath);
    return Instantiate(barrelResource) as GameObject;
  }

  public void AddBarrel()
  {
    if (hasBarrel) return;
    barrelObject = CreateBarrel();
    barrelObject.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
    hasBarrel = true;
  }

  public void RemoveBarrel()
  {
    if (!hasBarrel) return;
    barrelObject.SetActive(false);
    barrelObject = null;
    hasBarrel = false;
  }

}
