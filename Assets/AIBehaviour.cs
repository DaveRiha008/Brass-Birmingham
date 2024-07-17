using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour
{
  protected Player myAIPlayer;

  protected CardScript chosenCard;
  protected IndustrySpace chosenSpace;
  protected TileScript chosenTile;
  protected NetworkSpace chosenNetworkSpace;

  /// <summary>
  /// actions considered forbidden - don't even try to complete these
  /// </summary>
  protected List<ACTION> unFinishableActions = new();

  /// <summary>
  /// array through which AI goes in order - if AI is set to do only the first action, then this is important
  /// </summary>
  public ACTION[] actionsOrder = { ACTION.SELL, ACTION.BUILD, ACTION.NETWORK, ACTION.DEVELOP, ACTION.SCOUT, ACTION.LOAN };

  /// <summary>
  /// to stop infinite loops
  /// </summary>
  protected int startActionsDone = 0;

  /// <summary>
  /// to stop infinite loops
  /// </summary>
  protected int startTurnsDone = 0;


  public AIBehaviour(Player playerWithThisBehaviour)
  {
    myAIPlayer = playerWithThisBehaviour;
  }

  /// <summary>
  /// Called when AI first starts an action (not only when its turn begins)
  /// </summary>
  public virtual void StartTurn()
  {
    startActionsDone = 0;
    startTurnsDone++;
    //if (startTurnsDone > 2000) return;//Stack Overflow defense
    //Debug.Log($"Called StartTurn for {startTurnsDone}th time");
    unFinishableActions.Clear();
    //StartAction();
  }


  /// <summary>
  /// Called when AI should choose a new action to do -> can be called many times in one turn
  /// </summary>
  public virtual void StartAction()
  {
    chosenCard = null;
    chosenSpace = null;
    chosenTile = null;
    chosenNetworkSpace = null;


    startActionsDone++;
    //if (startActionsDone > 2000) return; //Stack Overflow defense

    //Try action based on given order
    foreach (ACTION action in actionsOrder)
    {
      //Don't try unfinishable actions
      if (unFinishableActions.Contains(action))
      {
        //Debug.Log($"{action} big bad");
        continue;
      }

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
          //Only action left is NONE -> end turn
          //Debug.Log("Calling EndTurn from AI -> NONE action in order");
          GameManager.EndTurn();
          break;
        default:
          break;
      }

      return;

    }
    //Debug.Log("AI didn't have any action to do");
    //Debug.Log("Calling EndTurn from AI -> no finishable action");

    //No finishable action left -> end turn
    GameManager.EndTurn();
  }


  /// <summary>
  /// Information for AI that an action is close to being successfully done -> used in more advanced versions of AI
  /// </summary>
  /// <param name="canceled">Output - whether action should be canceled in the last moment</param>
  public virtual void ActionAboutToBeDone(out bool canceled)
  {
    canceled = false;
  }

  protected virtual void CantChooseTile()
  {
    Debug.Log("AI couldnt choose Tile");
    CancelAction();
  }

  protected virtual void CantChooseSpace()
  {
    //Debug.Log("AI couldnt choose Space");

    CancelAction();
  }
  protected virtual void CantChooseBarrel()
  {
    //Debug.Log("AI couldnt choose Barrel");

    CancelAction();
  }
  protected virtual void CantChooseCard()
  {
    //Debug.Log("AI couldnt choose Card");

    CancelAction();
  }

  protected virtual void CantChooseCoal()
  {
    //Debug.Log("AI couldnt choose Coal");

    CancelAction();
  }
  protected virtual void CantChooseNetwork()
  {
    //Debug.Log("AI couldnt choose Network");

    CancelAction();
  }
  protected virtual void NotEnoughMoney(MISSING_MONEY_REASON reason)
  {
    //Debug.Log("AI had no Money");

    CancelAction();
  }
  protected virtual void NotEnoughIncome()
  {
    //Debug.Log("AI had not enough income");

    CancelAction();
  }


  protected virtual void Build()
  {
    ActionManager.DoAction(ACTION.BUILD);
    //Debug.Log("AI Called Build action!");
  }
  protected virtual void Sell()
  {
    ActionManager.DoAction(ACTION.SELL);
    //Debug.Log("AI Called Sell action!");
  }
  protected virtual void Loan()
  {
    if(myAIPlayer.income < Constants.loanIncomeCost)
    {
      ActionManager.currentAction = ACTION.LOAN; //Needed to know that this action is unfinishable -> can't call DoAction -> would cancel uncontroled by AI
      NotEnoughIncome(); return;  //Definitely unfinishable -> no point in trying any other options
    }
    ActionManager.DoAction(ACTION.LOAN);
    //Debug.Log("AI Called Loan action!");
  }
  protected virtual void Scout()
  {
    if (CardManager.PlayerHasWildCard(GameManager.activePlayerIndex))
    {
      ActionManager.currentAction = ACTION.SCOUT; //Needed to know that this action is unfinishable -> can't call DoAction -> would cancel uncontroled by AI
      CancelAction();
      return;
    }
    ActionManager.DoAction(ACTION.SCOUT);
    //Debug.Log("AI Called Scout action!");
  }
  protected virtual void Develop()
  {
    ActionManager.DoAction(ACTION.DEVELOP);
    //Debug.Log("AI Called Develop action!");
  }
  protected virtual void Network()
  {
    if ((GameManager.currentEra == ERA.TRAIN && GameManager.GetActivePlayer().money < Constants.train1Cost)
    || (GameManager.currentEra == ERA.BOAT && GameManager.GetActivePlayer().money < Constants.boatCost))
    { // Catch not enough money right away

      ActionManager.currentAction = ACTION.NETWORK; //Needed to know that this action is unfinishable -> can't call DoAction -> would cancel uncontroled by AI
      NotEnoughMoney(MISSING_MONEY_REASON.NETWORK_VEHICLE);
      return;
    }
    ActionManager.DoAction(ACTION.NETWORK);
    //Debug.Log("AI Called Network action!");
  }
  protected virtual void CancelAction()
  {
    chosenCard = null;
    chosenSpace = null;
    chosenTile = null;
    chosenNetworkSpace = null;

    if(ActionManager.currentAction != ACTION.NONE) //NONE is alaways finishable
      unFinishableActions.Add(ActionManager.currentAction);
    //Debug.Log($"AI added {ActionManager.currentAction} to unfinishableActions");
    ActionManager.CancelAction();
    //Debug.Log("AI Called Cancel action!");
  }



  public virtual void ChooseCard()
  {
    List<CardScript> possibleCards = GetPossibleCards();

    if(possibleCards.Count <= 0) { CantChooseCard(); return; }

    chosenCard = possibleCards[0];
    CardManager.ChooseCardFromHand(chosenCard);
  }
  public virtual void ChooseWildCard()
  {
    CardManager.ActivePlayerDrawCard(CardDeckType.WILD_LOCATION);
  }
  public virtual void ChooseTile()
  {
    List<TileScript> possibleTiles = GetPossibleTiles();
    if (possibleTiles.Count <= 0) {
      if ((ActionManager.currentAction == ACTION.SELL || ActionManager.currentAction == ACTION.DEVELOP) && ActionManager.IsActionFinishable())
      { DoneAction(); return; }
      else
      { CantChooseTile(); return; }
    }
    if (ActionManager.currentAction == ACTION.DEVELOP)
    {
      if (!IronCheck())
      {
        if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
        else { NotEnoughMoney(MISSING_MONEY_REASON.IRON); return; }
      }
    }
    chosenTile = possibleTiles[0];
    //Debug.Log($"AI choosing {chosenTile} for {ActionManager.currentAction}");
    ObjectManager.ChooseTile(chosenTile);
  }

  public virtual void ChooseIndustrySpace()
  {
    List<IndustrySpace> possibleSpaces = GetPossibleIndustrySpaces();
    
    //Debug.Log($"Possible spaces: {possibleSpaces.Count}");
    if (possibleSpaces.Count <= 0) { CantChooseSpace(); return; }
    chosenSpace = possibleSpaces[0];
    ObjectManager.ChooseSpace(chosenSpace);
  }

  public virtual void ChooseIron()
  {
    List<IronWorksTileScript> possibleSources = GetPossibleIronSources();
    if (possibleSources.Count <= 0)
    {
      if (myAIPlayer.money >= ObjectManager.GetIronStoragePrice())
        ObjectManager.ChoseIronStorage();
      else
      { NotEnoughMoney(MISSING_MONEY_REASON.IRON); return; }
    }
    else ObjectManager.ChooseTile(possibleSources[0]);
  }


  public virtual void ChooseCoal()
  {
    List<CoalMineTileScript> possibleSources;
    if (ActionManager.currentAction == ACTION.BUILD)
    {
      possibleSources = GetPossibleCoalSources(chosenSpace.myLocation);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenSpace.myLocation).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
          else { NotEnoughMoney(MISSING_MONEY_REASON.COAL); return; }
        }
        else { CantChooseCoal(); return; }
      }
      else ObjectManager.ChooseTile(possibleSources[0]);
    }

    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      possibleSources = GetPossibleCoalSources(chosenNetworkSpace);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenNetworkSpace).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
          else NotEnoughMoney(MISSING_MONEY_REASON.COAL);
        }
        else { CantChooseCoal(); return; }
      }
      else ObjectManager.ChooseTile(possibleSources[0]);
    }

  }

  public virtual void ChooseBarrel()
  {
    //Debug.Log("AI Choosing barrel");
    if (ActionManager.currentAction == ACTION.SELL)
    {
      List<BarrelSpace> merchantBarrels = ObjectManager.GetAllAvailableMerchantBarrels(chosenTile.builtOnSpace.myLocation, chosenTile); //Barrel from merchant
      if (merchantBarrels.Count > 0) { ObjectManager.BarrelSpaceClicked(merchantBarrels[0]); /*Debug.Log("AI Chose barrel succesfully");*/
        return; }

      List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenTile.builtOnSpace.myLocation); //Barrel from brewery
      if (breweriesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }
      //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");
      ObjectManager.ChooseTile(breweriesWithBarrels[0].myTile);

      //Debug.Log("AI Chose barrel succesfully");
    }
    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenNetworkSpace);
      if (breweriesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }
      //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");

      ObjectManager.ChooseTile(breweriesWithBarrels[0].myTile);
      //Debug.Log("AI Chose barrel succesfully");
    }
  }


  public virtual void ChooseNetwork()
  {
    if(GameManager.currentEra == ERA.TRAIN && chosenNetworkSpace is not null &&
      !GameManager.ActivePlayerHasEnoughMoney(Constants.train2Cost) && ActionManager.IsActionFinishable())
    { DoneAction(); return; }

    List<NetworkSpace> possibleNetworks = GetPossibleNetworkSpaces();

    if (possibleNetworks.Count<=0) 
    {
      if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
      else { CantChooseNetwork(); return; }
    }
    chosenNetworkSpace = possibleNetworks[0];
    ObjectManager.ChoseNetwork(chosenNetworkSpace);
  }

  protected virtual void DoneAction()
  {
    if (ActionManager.IsActionFinishable())
      ActionManager.CancelAction(true);
    else
      Debug.Log("AI tried to finish not done action");
  }

  protected virtual List<CardScript> GetPossibleCards()
  {
    return CardManager.GetPlayerCards(myAIPlayer.index);
  }
  protected virtual List<TileScript> GetPossibleTiles()
  {
    return ObjectManager.GetViableTilesForCurrentAction();
  }

  protected virtual List<IndustrySpace> GetPossibleIndustrySpaces()
  {
    if (chosenTile is not null)
      return ObjectManager.GetMyNetworkFreeSpacesForItemInHand(myAIPlayer.index);

    else
      return ObjectManager.GetAllViableBuildSpaces(chosenCard);
  }

  protected virtual List<IronWorksTileScript> GetPossibleIronSources()
  {
    return ObjectManager.GetAllIronWorksWithFreeIron();
  }


  protected virtual List<CoalMineTileScript> GetPossibleCoalSources(LocationScript location)
  {
    return ObjectManager.GetNearestCoalMinesWithFreeCoal(location);
  }
  protected virtual List<CoalMineTileScript> GetPossibleCoalSources(NetworkSpace networkSpace)
  {
    return ObjectManager.GetNearestCoalMinesWithFreeCoal(networkSpace);
  }
  protected virtual List<NetworkSpace> GetPossibleNetworkSpaces()
  {
    List<NetworkSpace> possibleNetworks = ObjectManager.GetMyNetworkNeighborConnections(myAIPlayer.index);

    if (GameManager.currentEra == ERA.TRAIN) //For trains check coal availability
    {
      List<NetworkSpace> possibleNetworksWithCoals = new();
      foreach (NetworkSpace space in possibleNetworks)
      {
        if (CoalCheck(space))
        {
          if (chosenNetworkSpace is null || BarrelCheckTiles(space)) //For second train check barrel
            possibleNetworksWithCoals.Add(space);
        }
      }

      possibleNetworks = possibleNetworksWithCoals;
    }

    return possibleNetworks;
  }


  protected bool CoalCheck(LocationScript location)
  {
    List<CoalMineTileScript> possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(location);
    if (possibleSources.Count <= 0)
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(location).Count > 0)
      {
        if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) return true;
        else { return false; }
      }
      else { return false; }
    }
    else return true;
  }
  protected bool IronCheck()
  {
    List<IronWorksTileScript> possibleSources = GetPossibleIronSources();
    if (possibleSources.Count <= 0)
    {
      if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) return true;
      else { return false; }
    }
    else return true;
  }

  protected bool CoalCheck(NetworkSpace space)
  {
    List<CoalMineTileScript> possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(space);
    if (possibleSources.Count <= 0)
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(space).Count > 0)
      {
        if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) return true;
        else { return false; }
      }
      else { return false; }
    }
    else return true;
  }
  protected bool BarrelCheckTiles(NetworkSpace space)
  {
    List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(space);
    if (breweriesWithBarrels.Count <= 0) { return false; }
    else return true;
  }
  protected bool BarrelCheckTiles(LocationScript location)
  {
    List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(location);
    if (breweriesWithBarrels.Count <= 0) { return false; }
    else return true;
  }
  protected bool BarrelCheckMerchants()
  {
    List<BarrelSpace> merchantBarrels = ObjectManager.GetAllAvailableMerchantBarrels(chosenTile.builtOnSpace.myLocation, chosenTile);
    if (merchantBarrels.Count > 0)
      return true;
    return false;
  }


  protected enum MISSING_MONEY_REASON { COAL, IRON, NETWORK_VEHICLE};
}

