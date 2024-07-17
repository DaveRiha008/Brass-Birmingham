using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGameStateEvaluation : AIChooseRepeatedly
{

  float incomeWeight = 0.5f;
  float moneyWeight = 0.05f;
  float victoryPointsWeight = 1f;
  float playerMoneySpentThisTurnWeight = 0.005f;

  float wildCardValue = 2;

  float myCoalWeightBoat = 1.5f;
  float otherCoalWeightBoat = 1.2f;
  float myCoalWeightTrain = 2.0f;
  float otherCoalWeightTrain = 1.6f;

  float myIronWeight = 1f;
  float otherIronWeight = 0.8f;

  float myBarrelValue = 1.5f;
  float otherBarrelValue = 1f;

  float connectedMerchantValue = 3;
  float merchantRewardWeight = 0.65f;
  float merchantDevelopRewardValue = 2;

  float networkNeighborVPValue = 1;
  float networkNeighborPotentialVPValue = 0.7f;

  float upgradedSellTileVicPointsWeight = 1;
  float potentialSellTileVicPointsWeight = 0.5f;
  float upgradedSellTileIncomeWeight = 0;
  float potentialSellTileIncomeWeight = 0.7f;

  float upgradedResourceTileVicPointsWeight = 1;
  float potentialResourceTileVicPointsWeight = 0.5f;
  float upgradedResourceTileIncomeWeight = 0;
  float potentialResourceTileIncomeWeight = 0.7f;


  float unbuiltResourceTileVicPointsWeight = 1;
  float unbuiltResourceTileIncomeWeight = 0.5f;
  float unbuiltSellTileVicPointsWeight = 1;
  float unbuiltSellTileIncomeWeight = 0.5f;




  /// <summary>
  /// actions that were not yet tried and evaluated and are possibly finishable
  /// </summary>
  List<ACTION> actionsToBeEvaluated = new();


  float stateValueBeforeAction = 0;

  /// <summary>
  /// So far the best action value
  /// </summary>
  float bestActionValue = -1;

  /// <summary>
  /// So far the best action
  /// </summary>
  ACTION bestAction = ACTION.NONE;



  public AIGameStateEvaluation(Player playerWithThisBehaviour) : base(playerWithThisBehaviour)
  {
  }

  /// <summary>
  /// Sets different weights and values to object based on a give strategy
  /// </summary>
  /// <param name="setting">Given strategy</param>
  public void SetValuesForStrategy(STATE_EVAL_SETTING setting)
  {
    switch (setting)
    {
      case STATE_EVAL_SETTING.BASIC:
        incomeWeight = 0.5f;
        moneyWeight = 0.05f;
        victoryPointsWeight = 1f;
        playerMoneySpentThisTurnWeight = 0.005f;

        wildCardValue = 2;

        myCoalWeightBoat = 1.5f;
        otherCoalWeightBoat = 1.2f;
        myCoalWeightTrain = 2.0f;
        otherCoalWeightTrain = 1.6f;

        myIronWeight = 1f;
        otherIronWeight = 0.8f;

        myBarrelValue = 1.5f;
        otherBarrelValue = 1f;

        connectedMerchantValue = 3;
        merchantRewardWeight = 0.65f;
        merchantDevelopRewardValue = 2;

        networkNeighborVPValue = 1;
        networkNeighborPotentialVPValue = 0.7f;

        upgradedSellTileVicPointsWeight = 1;
        potentialSellTileVicPointsWeight = 0.5f;
        upgradedSellTileIncomeWeight = 0;
        potentialSellTileIncomeWeight = 0.7f;

        upgradedResourceTileVicPointsWeight = 1;
        potentialResourceTileVicPointsWeight = 0.5f;
        upgradedResourceTileIncomeWeight = 0;
        potentialResourceTileIncomeWeight = 0.7f;


        unbuiltResourceTileVicPointsWeight = 1;
        unbuiltResourceTileIncomeWeight = 0.5f;
        unbuiltSellTileVicPointsWeight = 1;
        unbuiltSellTileIncomeWeight = 0.5f;
        break;
      case STATE_EVAL_SETTING.QUICK_SELL:
        incomeWeight = 0.5f;
        moneyWeight = 0.05f;
        victoryPointsWeight = 1f;
        playerMoneySpentThisTurnWeight = 0.005f;

        wildCardValue = 2;

        myCoalWeightBoat = 1.5f;
        otherCoalWeightBoat = 1.2f;
        myCoalWeightTrain = 2.0f;
        otherCoalWeightTrain = 1.6f;

        myIronWeight = 1f;
        otherIronWeight = 0.8f;

        myBarrelValue = 1.5f;
        otherBarrelValue = 1f;

        connectedMerchantValue = 6;
        merchantRewardWeight = 0.65f;
        merchantDevelopRewardValue = 2;

        networkNeighborVPValue = 1;
        networkNeighborPotentialVPValue = 0.7f;

        upgradedSellTileVicPointsWeight = 1.5f;
        potentialSellTileVicPointsWeight = 0.75f;
        upgradedSellTileIncomeWeight = 0;
        potentialSellTileIncomeWeight = 1f;

        upgradedResourceTileVicPointsWeight = 0.75f;
        potentialResourceTileVicPointsWeight = 0.35f;
        upgradedResourceTileIncomeWeight = 0;
        potentialResourceTileIncomeWeight = 0.5f;

        unbuiltResourceTileVicPointsWeight = 1;
        unbuiltResourceTileIncomeWeight = 0.5f;
        unbuiltSellTileVicPointsWeight = 1.2f;
        unbuiltSellTileIncomeWeight = 0.6f;
        break;
      case STATE_EVAL_SETTING.BUILD_MASTER:
        incomeWeight = 0.5f;
        moneyWeight = 0.05f;
        victoryPointsWeight = 1f;
        playerMoneySpentThisTurnWeight = 0.005f;

        wildCardValue = 2;

        myCoalWeightBoat = 1.5f;
        otherCoalWeightBoat = 1.2f;
        myCoalWeightTrain = 2.0f;
        otherCoalWeightTrain = 1.6f;

        myIronWeight = 1f;
        otherIronWeight = 0.8f;

        myBarrelValue = 1.5f;
        otherBarrelValue = 1f;

        connectedMerchantValue = 1;
        merchantRewardWeight = 0.65f;
        merchantDevelopRewardValue = 2;

        networkNeighborVPValue = 1;
        networkNeighborPotentialVPValue = 0.7f;

        upgradedSellTileVicPointsWeight = 0.75f;
        potentialSellTileVicPointsWeight = 0.35f;
        upgradedSellTileIncomeWeight = 0;
        potentialSellTileIncomeWeight = 0.5f;

        upgradedResourceTileVicPointsWeight = 1.5f;
        potentialResourceTileVicPointsWeight = 0.75f;
        upgradedResourceTileIncomeWeight = 0;
        potentialResourceTileIncomeWeight = 1f;

        unbuiltResourceTileVicPointsWeight = 1.2f;
        unbuiltResourceTileIncomeWeight = 0.6f;
        unbuiltSellTileVicPointsWeight = 1;
        unbuiltSellTileIncomeWeight = 0.5f;
        break;
      case STATE_EVAL_SETTING.NETWORK_MASTER:
        incomeWeight = 0.5f;
        moneyWeight = 0.05f;
        victoryPointsWeight = 1f;
        playerMoneySpentThisTurnWeight = 0.005f;

        wildCardValue = 0.5f;

        myCoalWeightBoat = 1.5f;
        otherCoalWeightBoat = 1.2f;
        myCoalWeightTrain = 3.0f;
        otherCoalWeightTrain = 2.1f;

        myIronWeight = 1f;
        otherIronWeight = 0.8f;

        myBarrelValue = 1.5f;
        otherBarrelValue = 1f;

        connectedMerchantValue = 3;
        merchantRewardWeight = 0.65f;
        merchantDevelopRewardValue = 2;

        networkNeighborVPValue = 2;
        networkNeighborPotentialVPValue = 1.4f;

        upgradedSellTileVicPointsWeight = 1;
        potentialSellTileVicPointsWeight = 0.5f;
        upgradedSellTileIncomeWeight = 0;
        potentialSellTileIncomeWeight = 0.7f;

        upgradedResourceTileVicPointsWeight = 1;
        potentialResourceTileVicPointsWeight = 0.5f;
        upgradedResourceTileIncomeWeight = 0;
        potentialResourceTileIncomeWeight = 0.7f;

        unbuiltResourceTileVicPointsWeight = 1;
        unbuiltResourceTileIncomeWeight = 0.5f;
        unbuiltSellTileVicPointsWeight = 1;
        unbuiltSellTileIncomeWeight = 0.5f;
        break;
      case STATE_EVAL_SETTING.QUCIK_DEV:
        incomeWeight = 0.5f;
        moneyWeight = 0.05f;
        victoryPointsWeight = 1f;
        playerMoneySpentThisTurnWeight = 0.005f;

        wildCardValue = 2;

        myCoalWeightBoat = 1.5f;
        otherCoalWeightBoat = 1.2f;
        myCoalWeightTrain = 2.0f;
        otherCoalWeightTrain = 1.6f;

        myIronWeight = 1f;
        otherIronWeight = 0.8f;

        myBarrelValue = 1.5f;
        otherBarrelValue = 1f;

        connectedMerchantValue = 3;
        merchantRewardWeight = 0.65f;
        merchantDevelopRewardValue = 2;

        networkNeighborVPValue = 1;
        networkNeighborPotentialVPValue = 0.7f;

        upgradedSellTileVicPointsWeight = 1;
        potentialSellTileVicPointsWeight = 0.5f;
        upgradedSellTileIncomeWeight = 0;
        potentialSellTileIncomeWeight = 0.7f;

        upgradedResourceTileVicPointsWeight = 1;
        potentialResourceTileVicPointsWeight = 0.5f;
        upgradedResourceTileIncomeWeight = 0;
        potentialResourceTileIncomeWeight = 0.7f;

        unbuiltResourceTileVicPointsWeight = 2f;
        unbuiltResourceTileIncomeWeight = 1f;
        unbuiltSellTileVicPointsWeight = 2f;
        unbuiltSellTileIncomeWeight = 1f;
        break;
      default:
        break;
    }
  }


  public override void StartTurn()
  {


    //Debug.Log("State eval AI starting turn");

    stateValueBeforeAction = GetGameStateValue();
    bestAction = ACTION.NONE;
    bestActionValue = -1; // Doing nothing brings negative outcome -> to block wasting time

    actionsToBeEvaluated.Clear();

    foreach (ACTION action in actionsOrder)
    {
      actionsToBeEvaluated.Add(action);
    }

    base.StartTurn();
  }

  /// <summary>
  /// Do given action 
  /// </summary>
  void DoAction(ACTION action)
  {
    //Debug.Log($"Would call {action} in {startActionsDone} iteration");
    switch (action)
    {
      case ACTION.BUILD:
        Build();
        break;
      case ACTION.SELL:
        Sell();
        break;
      case ACTION.LOAN:
        Loan();
        break;
      case ACTION.SCOUT:
        Scout();
        break;
      case ACTION.DEVELOP:
        Develop();
        break;
      case ACTION.NETWORK:
        Network();
        break;
      case ACTION.NONE:
        Debug.Log("SHOULDN'T HAPPEN - Calling EndTurn from AI -> NONE action in order");
        GameManager.EndTurn();
        break;
      default:
        break;
    }
  }

  public override void StartAction()
  {
    chosenCard = null;
    chosenSpace = null;
    chosenTile = null;
    chosenNetworkSpace = null;


    startActionsDone++;
    //if (startActionsDone > 2000) return; //Stack Overflow defense


    //If hand is empty -> we can't play any action -> skip right away
    if(CardManager.GetPlayerCards(myAIPlayer.index).Count <= 0)
    {
      Debug.Log("No card in hand -> skipping turn");
      GameManager.EndTurn();
      return;
    }

    //If we evaluated all actions -> call the best one
    if(actionsToBeEvaluated.Count <= 0)
    {
      //Debug.Log($"AI State eval - tried all actions - best is {bestAction}");

      if(bestAction == ACTION.NONE)
      {
        //No action with better state than the initial found -> EndingTurn 
        //Debug.Log("No action with better state than the initial found -> EndingTurn ");
        GameManager.EndTurn();
        return;
      }

      //Debug.Log($"Best action found: {bestAction}");

      DoAction(bestAction);
      return;
    }


    //Debug.Log($"AI State eval - trying action {actionsToBeEvaluated[0]}");
    DoAction(actionsToBeEvaluated[0]);

  }

  protected override void CancelAction()
  {
    actionsToBeEvaluated.Remove(ActionManager.currentAction);


    base.CancelAction();
  }


  public override void ActionAboutToBeDone(out bool canceled)
  {
    //Differ between evaluating action and actually trying to complete the action

    //Done action was already called as the best one
    if (actionsToBeEvaluated.Count <= 0){ canceled = false; return; }

    float newStateValue = GetGameStateValue();
    float actionValue = newStateValue - stateValueBeforeAction;
    //Debug.Log($"AI Value of action {ActionManager.currentAction} base on state is {actionValue}  \n BaseStateValue: {stateValueBeforeAction}  \t NewStateValue: {newStateValue}");

    if(actionValue > bestActionValue)
    {
      bestActionValue = actionValue;
      bestAction = ActionManager.currentAction;
    }

    actionsToBeEvaluated.Remove(ActionManager.currentAction);

    //Cancel action -> try other actions
    canceled = true;
    return;

  }


  float GetGameStateValue()
  {


    float gameStateValue = 0;
    float builtTilesValue = GetValueOfBuiltTiles();
    gameStateValue += builtTilesValue;

    float nextLevelTilesValue = GetValueOfNextLevelTiles();
    gameStateValue += nextLevelTilesValue;

    float networkValue = GetValueOfNetwork();
    gameStateValue += networkValue;

    float connectedMerchantsValue = GetValueOfMerchantsConnected();
    gameStateValue += connectedMerchantsValue;

    float resourcesValue = GetValueOfResources();
    gameStateValue += resourcesValue;

    float cardsValue = GetValueOfCards();
    gameStateValue += cardsValue;

    float playerAttValue = GetValueOfPlayerAttributes();
    gameStateValue += playerAttValue;


    //Debug.Log($"Evaluating state besed on action: {ActionManager.currentAction}");

    //Debug.Log($"BuiltTilesValue = {builtTilesValue}");

    //Debug.Log($"nextLevelTilesValue = {nextLevelTilesValue}");

    //Debug.Log($"networkValue = {networkValue}");

    //Debug.Log($"connectedMerchantsValue = {connectedMerchantsValue}");

    //Debug.Log($"resourcesValue = {resourcesValue}");

    //Debug.Log($"cardsValue = {cardsValue}");

    //Debug.Log($"playerAttValue = {playerAttValue}");


    return gameStateValue;
  }

  float GetValueOfBuiltTiles()
  {
    float value = 0;
    List<TileScript> builtTiles = ObjectManager.GetAllMyBuiltTiles(myAIPlayer.index);

    foreach(TileScript tile in builtTiles)
    {
      if (tile != ObjectManager.overBuiltTile)
        value += GetBuiltTileValue(tile);
    }

    return value;
  }

  float GetBuiltTileValue(TileScript tile)
  {
    float value;

    float upgradedTileIncomeWeight;
    float upgradedTileVicPointsWeight;
    float potentialTileIncomeWeight;
    float potentialTileVicPointsWeight;

    //If the tile is sellable
    if(tile.industryType == INDUSTRY_TYPE.COTTONMILL || tile.industryType == INDUSTRY_TYPE.MANUFACTURER || tile.industryType == INDUSTRY_TYPE.POTTERY)
    {
      upgradedTileIncomeWeight = upgradedSellTileIncomeWeight;
      upgradedTileVicPointsWeight = upgradedSellTileVicPointsWeight;
      potentialTileIncomeWeight = potentialSellTileIncomeWeight;
      potentialTileVicPointsWeight = potentialSellTileVicPointsWeight;
    }
    //If the tile is resource source
    else
    {
      upgradedTileIncomeWeight = upgradedResourceTileIncomeWeight;
      upgradedTileVicPointsWeight = upgradedResourceTileVicPointsWeight;
      potentialTileIncomeWeight = potentialResourceTileIncomeWeight;
      potentialTileVicPointsWeight = potentialResourceTileVicPointsWeight;

    }

    //Reduce value of not upgraded tile
    if (tile.isUpgraded)
      value = tile.upgradeIncomeReward * upgradedTileIncomeWeight + tile.upgradeVicPtsReward * upgradedTileVicPointsWeight;
    else
      value = tile.upgradeIncomeReward * potentialTileIncomeWeight + tile.upgradeVicPtsReward * potentialTileVicPointsWeight;

    return value;
  }
  /// <summary>
  /// Gets the value of all the tiles in order to be built
  /// </summary>
  /// <returns></returns>
  float GetValueOfNextLevelTiles()
  {
    float value = 0;

    TileScript firstUnbuiltBrewery = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.BREWERY, myAIPlayer.index);
    TileScript firstUnbuiltCoalMine = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.COALMINE, myAIPlayer.index);
    TileScript firstUnbuiltCottonMill = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.COTTONMILL, myAIPlayer.index);
    TileScript firstUnbuiltIronWorks = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.IRONWORKS, myAIPlayer.index);
    TileScript firstUnbuiltManufacturer = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.MANUFACTURER, myAIPlayer.index);
    TileScript firstUnbuiltPottery = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.POTTERY, myAIPlayer.index);

    if (firstUnbuiltBrewery is not null)
      value += GetUnbuiltTileValue(firstUnbuiltBrewery);
    if (firstUnbuiltCoalMine is not null)
      value += GetUnbuiltTileValue(firstUnbuiltCoalMine);
    if (firstUnbuiltCottonMill is not null)
      value += GetUnbuiltTileValue(firstUnbuiltCottonMill);
    if (firstUnbuiltIronWorks is not null)
      value += GetUnbuiltTileValue(firstUnbuiltIronWorks);
    if (firstUnbuiltManufacturer is not null)
      value += GetUnbuiltTileValue(firstUnbuiltManufacturer);
    if (firstUnbuiltPottery is not null)
      value += GetUnbuiltTileValue(firstUnbuiltPottery);


    return value;
  }

  float GetUnbuiltTileValue(TileScript tile)
  {
    float unbuiltTileVicPointsWeight;
    float unbuiltTileIncomeWeight;

    //If the tile is sellable
    if (tile.industryType == INDUSTRY_TYPE.COTTONMILL || tile.industryType == INDUSTRY_TYPE.MANUFACTURER || tile.industryType == INDUSTRY_TYPE.POTTERY)
    {
      unbuiltTileVicPointsWeight = unbuiltSellTileVicPointsWeight;
      unbuiltTileIncomeWeight = unbuiltSellTileIncomeWeight;
    }    
    //If the tile is resource source
    else
    {
      unbuiltTileVicPointsWeight = unbuiltResourceTileVicPointsWeight;
      unbuiltTileIncomeWeight = unbuiltResourceTileIncomeWeight;
    }

    float value = tile.upgradeVicPtsReward * unbuiltTileVicPointsWeight + tile.upgradeIncomeReward * unbuiltTileIncomeWeight;

    value -= tile.buildCost * moneyWeight;

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

        //Add value for merchant rewards
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

    float coalCost = ObjectManager.GetCoalStoragePrice();

    float myCoalWeight;
    float otherCoalWeight;

    //Differ coal value in different eras
    if(GameManager.currentEra == ERA.BOAT)
    {
      myCoalWeight = myCoalWeightBoat;
      otherCoalWeight = otherCoalWeightBoat;
    }
    else
    {
      myCoalWeight = myCoalWeightTrain;
      otherCoalWeight = otherCoalWeightTrain;
    }

    foreach (CoalMineTileScript coalMine in coalMinesWithFreeCoal)
    {
      if (!ObjectManager.IsLocationConnectedToMyNetwork(coalMine.builtOnSpace.myLocation, myAIPlayer.index)) continue;
      if (coalMine.ownerPlayerIndex == myAIPlayer.index) value += myCoalWeight * coalMine.GetResourceCount() * coalCost;
      else value += otherCoalWeight * coalMine.GetResourceCount() * coalCost;
    }

    return value;
  }

  float GetValueOfIron()
  {
    float value = 0;
    List<IronWorksTileScript> ironWorksWithFreeIron = ObjectManager.GetAllIronWorksWithFreeIron();

    float ironCost = ObjectManager.GetIronStoragePrice();

    foreach (IronWorksTileScript ironWorks in ironWorksWithFreeIron)
    {
      if (ironWorks.ownerPlayerIndex == myAIPlayer.index) value += myIronWeight * ironWorks.GetResourceCount() * ironCost;
      else value += otherIronWeight * ironWorks.GetResourceCount() * ironCost;
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
      if (!ObjectManager.IsLocationConnectedToMyNetwork(brewery.builtOnSpace.myLocation, myAIPlayer.index)) continue;
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

public enum STATE_EVAL_SETTING { BASIC, QUICK_SELL, BUILD_MASTER, NETWORK_MASTER, QUCIK_DEV };