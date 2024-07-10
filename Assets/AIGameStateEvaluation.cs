using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGameStateEvaluation : AIChooseRepeatedly
{

  const float incomeWeight = 0.5f;
  const float moneyWeight = 0.2f;
  const float victoryPointsWeight = 0.8f;
  const float playerMoneySpentThisTurnWeight = 0.05f;

  const float wildCardValue = 5;

  const float myCoalValue = 1f;
  const float otherCoalValue = 0.8f;

  const float myIronValue = 1f;
  const float otherIronValue = 0.8f;

  const float myBarrelValue = 1f;
  const float otherBarrelValue = 0.8f;

  const float connectedMerchantValue = 5;
  const float merchantRewardWeight = 0.5f;
  const float merchantDevelopRewardValue = 3;

  const float networkNeighborVPValue = 1;
  const float networkNeighborPotentialVPValue = 0.5f;

  const float nextTileLevelWeight = 0.2f;

  const float tileUpgradeValueWeight = 1;
  const float tilePotentialUpgradeValueWeight = 0.5f;


  public AIGameStateEvaluation(Player playerWithThisBehaviour) : base(playerWithThisBehaviour)
  {
  }


  float GetGameStateValue()
  {
    float gameStateValue = 0;
    gameStateValue += GetValueOfBuiltTiles();
    gameStateValue += GetValueOfNextTileLevels();
    gameStateValue += GetValueOfNetwork();
    gameStateValue += GetValueOfMerchantsConnected();
    gameStateValue += GetValueOfResources();
    gameStateValue += GetValueOfCards();
    gameStateValue += GetValueOfPlayerAttributes();


    return gameStateValue;
  }

  float GetValueOfBuiltTiles()
  {
    float value = 0;
    List<TileScript> builtTiles = ObjectManager.GetAllMyBuiltTiles(myAIPlayer.index);

    foreach(TileScript tile in builtTiles)
    {
      value += GetBuiltTileValue(tile);
    }

    return value;
  }

  float GetBuiltTileValue(TileScript tile)
  {
    float value = tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
    if (tile.isUpgraded) value *= tileUpgradeValueWeight;
    else value *= tilePotentialUpgradeValueWeight;


    return value;
  }

  float GetValueOfNextTileLevels()
  {
    float value = 0;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.BREWERY, out _) * nextTileLevelWeight;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.COALMINE, out _) * nextTileLevelWeight;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.COTTONMILL, out _) * nextTileLevelWeight;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.IRONWORKS, out _) * nextTileLevelWeight;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.MANUFACTURER, out _) * nextTileLevelWeight;
    value += ObjectManager.GetCurrentIndustryLevel(INDUSTRY_TYPE.POTTERY, out _) * nextTileLevelWeight;

    return value;
  }

  float GetValueOfNetwork()
  {
    float value = 0;

    List<NetworkSpace> myNetwork = ObjectManager.GetAllMyNetworkSpaces(myAIPlayer.index);

    foreach (NetworkSpace network in myNetwork)
      value += GetNetworkSpaceValue(network);

    return value;
  }

  float GetNetworkSpaceValue(NetworkSpace space)
  {


    float value = 0;

    //Add utility for each built tile on adjacent locations -> potential points for network
    foreach (LocationScript location in space.connectsLocations)
    {
      if (location.myType == LocationType.CITY)
        foreach (IndustrySpace indSpace in ObjectManager.GetAllIndustrySpacesInLocation(location))
        {
          if (indSpace.myTile is not null)
          {
            float tileNetworkReward = indSpace.myTile.upgradeNetworkVicPtsReward * networkNeighborVPValue;
            if (!indSpace.myTile.isUpgraded) tileNetworkReward = indSpace.myTile.upgradeNetworkVicPtsReward * networkNeighborPotentialVPValue;


            value += tileNetworkReward;
          }
        }
    }

    return value;

  }
  float GetValueOfMerchantsConnected()
  {
    float value = 0;

    List<LocationScript> myNetworkLocations = ObjectManager.GetMyNetworkLocations(myAIPlayer.index);

    List<MerchantTileSpace> allConnectedMerchants = new();

    foreach(LocationScript location in myNetworkLocations)
    {

      List<MerchantTileSpace> connectedMerchants = ObjectManager.GetAllConnectedMerchantTiles(location);
      foreach(MerchantTileSpace merchant in connectedMerchants)
      {
        if (!allConnectedMerchants.Contains(merchant)) allConnectedMerchants.Add(merchant);
      }
    }

    foreach(MerchantTileSpace merchant in allConnectedMerchants)
    {
      //If merchant isn't empty
      if(merchant.myTile.hasCot || merchant.myTile.hasMan || merchant.myTile.hasPottery)
      {
        value += connectedMerchantValue;
        if (merchant.myBarrelSpace.hasBarrel)
        {
          switch (merchant.myReward.type)
          {
            case MERCHANT_REW_TYPES.MONEY:
              value += moneyWeight * merchantRewardWeight * merchant.myReward.amount;
              break;
            case MERCHANT_REW_TYPES.VIC_POINTS:
              value += victoryPointsWeight * merchantRewardWeight * merchant.myReward.amount;
              break;
            case MERCHANT_REW_TYPES.INCOME:
              value += incomeWeight * merchantRewardWeight * merchant.myReward.amount;
              break;
            case MERCHANT_REW_TYPES.DEVELOP:
              value += merchantDevelopRewardValue;
              break;
            case MERCHANT_REW_TYPES.NONE:
              break;
            default:
              break;
          }
        }
      }
    }

    return value;
  }

  float GetValueOfResources()
  {
    float value = 0;
    value += GetValueOfCoal();
    value += GetValueOfIron();
    value += GetValueOfBarrels();

    return value;
  }
  float GetValueOfCoal()
  {
    float value = 0;
    List<CoalMineTileScript> coalMinesWithFreeCoal = ObjectManager.GetAllCoalMinesWithFreeCoal();

    foreach (CoalMineTileScript coalMine in coalMinesWithFreeCoal)
    {
      if (!ObjectManager.IsLocationPartOfMyNetwork(coalMine.builtOnSpace.myLocation, myAIPlayer.index)) continue;
      if (coalMine.ownerPlayerIndex == myAIPlayer.index) value += myCoalValue * coalMine.GetResourceCount();
      else value += otherCoalValue * coalMine.GetResourceCount();
    }

    return value;
  }

  float GetValueOfIron()
  {
    float value = 0;
    List<IronWorksTileScript> ironWorksWithFreeIron = ObjectManager.GetAllIronWorksWithFreeIron();

    foreach (IronWorksTileScript ironWorks in ironWorksWithFreeIron)
    {
      if (ironWorks.ownerPlayerIndex == myAIPlayer.index) value += myIronValue * ironWorks.GetResourceCount();
      else value += otherIronValue * ironWorks.GetResourceCount();
    }

    return value;
  }
  float GetValueOfBarrels()
  {
    float value = 0;
    List<BreweryTileScript> breweriesWithFreeBarrels = ObjectManager.GetAllBreweriesWithFreeBarrels();

    foreach (BreweryTileScript brewery in breweriesWithFreeBarrels)
    {
      if (brewery.ownerPlayerIndex == myAIPlayer.index) value += myBarrelValue * brewery.GetResourceCount();
      else value += otherBarrelValue * brewery.GetResourceCount();
    }

    return value;
  }

  float GetValueOfCards()
  {
    if (CardManager.PlayerHasWildCard(myAIPlayer.index)) return wildCardValue;
    else return 0;
  }


  float GetValueOfPlayerAttributes()
  {
    float value = 0;
    value += myAIPlayer.money * moneyWeight;
    value += myAIPlayer.moneySpentThisTurn * playerMoneySpentThisTurnWeight;
    value += myAIPlayer.income * incomeWeight;
    value += myAIPlayer.victoryPoints * victoryPointsWeight;
    return value;
  }






}
