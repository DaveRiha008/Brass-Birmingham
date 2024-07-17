using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileScript : Clickable, ISaveable
{
  public int id = 0;

  public IndustrySpace builtOnSpace;

  public bool alreadyBuilt = false;


  public int buildCost = 0;
  public int buildIronReq = 0;
  public int buildCoalReq = 0;

  public int upgradeIncomeReward = 0;
  public int upgradeVicPtsReward = 0;
  public int upgradeNetworkVicPtsReward = 0;


  //Everything bellow should be set manually from inspector -> in prefab

  public Sprite basicSprite;
  public Sprite upgradedSprite;

  public Vector3 basicScale;
  public Vector3 upgradedScale;

  public INDUSTRY_TYPE industryType;
  public int level = 1;
  public bool isUpgraded = false;
  public bool isDeveloped = false;

  public bool canBeDeveloped = true;
  public bool canBeBuiltInBoatEra = true;
  public bool canBeBuiltInTrainEra = true;

  public int ownerPlayerIndex = -1;

  // Start is called before the first frame update
  void Start()
  {
    // Define cost based on constant values
    switch (industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        buildCost = Constants.breweryCosts[level-1];
        buildIronReq = Constants.breweryBuildIronReq[level - 1];
        buildCoalReq = Constants.breweryBuildCoalReq[level - 1];
        
        upgradeIncomeReward = Constants.breweryUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.breweryUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.breweryUpNetVicPtsRewards[level - 1];
        break;
      case INDUSTRY_TYPE.COALMINE:
        buildCost = Constants.coalMineCosts[level - 1];
        buildIronReq = Constants.coalMineBuildIronReq[level - 1];
        buildCoalReq = Constants.coalMineBuildCoalReq[level - 1];

        upgradeIncomeReward = Constants.coalMineUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.coalMineUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.coalMineUpNetVicPtsRewards[level - 1];

        break;
      case INDUSTRY_TYPE.COTTONMILL:
        buildCost = Constants.cottonMillCosts[level - 1];
        buildIronReq = Constants.cottonMillBuildIronReq[level - 1];
        buildCoalReq = Constants.cottonMillBuildCoalReq[level - 1];

        upgradeIncomeReward = Constants.cottonMillUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.cottonMillUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.cottonMillUpNetVicPtsRewards[level - 1];
        buildCoalReq = Constants.breweryBuildCoalReq[level - 1];

        break;
      case INDUSTRY_TYPE.IRONWORKS:
        buildCost = Constants.ironWorksCosts[level - 1];
        buildIronReq = Constants.ironWorksBuildIronReq[level - 1];
        buildCoalReq = Constants.ironWorksBuildCoalReq[level - 1];

        upgradeIncomeReward = Constants.ironWorksUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.ironWorksUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.ironWorksUpNetVicPtsRewards[level - 1];

        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        buildCost = Constants.manufacturerCosts[level - 1];
        buildIronReq = Constants.manufacturerBuildIronReq[level - 1];
        buildCoalReq = Constants.manufacturerBuildCoalReq[level - 1];

        upgradeIncomeReward = Constants.manufacturerUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.manufacturerUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.manufacturerUpNetVicPtsRewards[level - 1];

        break;
      case INDUSTRY_TYPE.POTTERY:
        buildCost = Constants.potteryCosts[level - 1];
        buildIronReq = Constants.potteryBuildIronReq[level - 1];
        buildCoalReq = Constants.potteryBuildCoalReq[level - 1];

        upgradeIncomeReward = Constants.potteryUpIncomeRewards[level - 1];
        upgradeVicPtsReward = Constants.potteryUpVicPtsRewards[level - 1];
        upgradeNetworkVicPtsReward = Constants.potteryUpNetVicPtsRewards[level - 1];

        break;
      case INDUSTRY_TYPE.NONE:
        break;
      default:
        break;
    }
  }

  // Update is called once per frame
  void Update()
  {
    
    if (builtOnSpace is not null && !isDeveloped && alreadyBuilt)
    {
      if (isDeveloped || !alreadyBuilt) builtOnSpace = null;
      else if (builtOnSpace.myTile is not null)
      {
        if (builtOnSpace.myTile != this)
        {
          Debug.Log($"My space has another :'( - I am {this}");
        }
      }
      else
        builtOnSpace.myTile = this;

    }
  }

  public override void OnClick()
  {
    if (canBeClicked)
    {
      Debug.Log("I was CLICKED!");
      ObjectManager.ChooseTile(this);
    }
  }

  public virtual void BecomeBuilt()
  {
    alreadyBuilt = true;

    
  }

  public virtual void BecomeUnbuilt()
  {
    alreadyBuilt = false;

  }

  public void Upgrade(bool gainIncome=true)
  {
    if (isUpgraded) return;

    if(gainIncome)
      GameManager.PlayerGainIncome(ownerPlayerIndex, upgradeIncomeReward);


    GetComponent<SpriteRenderer>().sprite = upgradedSprite;
    transform.localScale = upgradedScale;

    //Set collider size based on sprite
    GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
    isUpgraded = true;
  }

  public void Downgrade(bool loseIncome=true)
  {
    if (!isUpgraded) return;

    if(loseIncome)
      GameManager.PlayerLoseIncome(ownerPlayerIndex, upgradeIncomeReward);


    GetComponent<SpriteRenderer>().sprite = basicSprite;
    transform.localScale = basicScale;

    //Set collider size based on sprite
    GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().sprite.bounds.size;
    isUpgraded = false;
  }

  public void Develop()
  {
    GetComponent<SpriteRenderer>().enabled = false;
    GetComponent<BoxCollider2D>().enabled = false;
    //alreadyBuilt = true;
    isDeveloped = true;

  }
  public void Undevelop()
  {
    GetComponent<SpriteRenderer>().enabled = true;
    GetComponent<BoxCollider2D>().enabled = true;
    //alreadyBuilt = false;
    isDeveloped = false;
  }

  public virtual void Remove()
  {

    isDeveloped = true;
    alreadyBuilt = false;
    isUpgraded = false;

    GetComponent<SpriteRenderer>().enabled = false;
    GetComponent<BoxCollider2D>().enabled = false;
    gameObject.SetActive(false);
  }

  public virtual void PopulateSaveData(SaveData sd)
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

    sd.tileScriptData.Add(myData);
  }

  public virtual void LoadFromSaveData(SaveData sd)
  {
    foreach(SaveData.TileScriptData data in sd.tileScriptData)
      if(data.id == id)
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
        if (data.isUpgraded) Upgrade(false);
        else Downgrade(false);
        if (data.isDeveloped) Develop();
        else Undevelop();
        alreadyBuilt = data.alreadyBuilt;

        ownerPlayerIndex = data.ownerPlayerIndex;
      }
  }
}

public enum INDUSTRY_TYPE { BREWERY, COALMINE, COTTONMILL, IRONWORKS, MANUFACTURER, POTTERY, NONE }