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
  protected List<ACTION> unFinishableActions = new();
  protected List<ACTION> actionsOrder = new() { ACTION.SELL, ACTION.BUILD, ACTION.NETWORK, ACTION.DEVELOP, ACTION.SCOUT, ACTION.LOAN, ACTION.NONE };
  protected int startActionsDone = 0;
  protected int startTurnsDone = 0;


  public AIBehaviour(Player playerWithThisBehaviour)
  {
    myAIPlayer = playerWithThisBehaviour;
  }


  public virtual void StartTurn()
  {
    startActionsDone = 0;
    startTurnsDone++;
    if (startTurnsDone > 2000) return;//Stack Overflow defense
    //Debug.Log($"Called StartTurn for {startTurnsDone}th time");
    unFinishableActions.Clear();
    //StartAction();
  }
  public virtual void StartAction()
  {
    chosenCard = null;
    chosenSpace = null;
    chosenTile = null;
    chosenNetworkSpace = null;


    startActionsDone++;
    if (startActionsDone > 2000) return; //Stack Overflow defense
    foreach (ACTION action in actionsOrder)
    {
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

    GameManager.EndTurn();
  }


  protected virtual void CantChooseTile()
  {
    //Debug.Log("AI couldnt choose Tile");
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
  protected virtual void NotEnoughMoney()
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
      unFinishableActions.Add(ACTION.LOAN); NotEnoughIncome(); return;
    }
    ActionManager.DoAction(ACTION.LOAN);
    //Debug.Log("AI Called Loan action!");
  }
  protected virtual void Scout()
  {
    if (CardManager.PlayerHasWildCard(GameManager.activePlayerIndex))
    {
      unFinishableActions.Add(ACTION.SCOUT);
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
      unFinishableActions.Add(ACTION.NETWORK);
      NotEnoughMoney();
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

    unFinishableActions.Add(ActionManager.currentAction);
    //Debug.Log($"AI added {ActionManager.currentAction} to unfinishableActions");
    ActionManager.CancelAction();
    //Debug.Log("AI Called Cancel action!");
  }

  public virtual void ChooseCard()
  {
    List<CardScript> possibleCards = CardManager.GetPlayerCards(myAIPlayer.index);

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
    List<TileScript> possibleTiles = ObjectManager.GetViableTilesForCurrentAction();
    if (possibleTiles.Count <= 0) {
      if ((ActionManager.currentAction == ACTION.SELL || ActionManager.currentAction == ACTION.DEVELOP) && ActionManager.IsActionFinishable())
      { DoneAction(); return; }
      else
      { CantChooseTile(); return; }
    }
    if (ActionManager.currentAction == ACTION.DEVELOP)
    {
      List<IronWorksTileScript> possibleIronSources = ObjectManager.GetAllIronWorksWithFreeIron();
      if (possibleIronSources.Count <= 0 && !GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetIronStoragePrice()))
      {
        if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
        else { NotEnoughMoney(); return; }
      }
    }
    chosenTile = possibleTiles[0];
    //Debug.Log($"AI choosing {chosenTile} for {ActionManager.currentAction}");
    ObjectManager.ChooseTile(chosenTile);
  }

  public virtual void ChooseIndustrySpace()
  {
    List<IndustrySpace> possibleSpaces;

    if (chosenTile is not null)
      possibleSpaces = ObjectManager.GetMyNetworkFreeSpacesForItemInHand(myAIPlayer.index);

    else
      possibleSpaces = ObjectManager.GetAllViableBuildSpaces(chosenCard);
    
    //Debug.Log($"Possible spaces: {possibleSpaces.Count}");
    if (possibleSpaces.Count <= 0) { CantChooseSpace(); return; }
    chosenSpace = possibleSpaces[0];
    ObjectManager.ChooseSpace(chosenSpace);
  }

  public virtual void ChooseIron()
  {
    List<IronWorksTileScript> possibleSources = ObjectManager.GetAllIronWorksWithFreeIron();
    if (possibleSources.Count <= 0)
    {
      if (myAIPlayer.money >= ObjectManager.GetIronStoragePrice())
        ObjectManager.ChoseIronStorage();
      else
      { NotEnoughMoney(); return; }
    }
    else ObjectManager.ChooseTile(possibleSources[0]);
  }

  public virtual void ChooseCoal()
  {
    List<CoalMineTileScript> possibleSources;
    if (ActionManager.currentAction == ACTION.BUILD)
    {
      possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(chosenSpace.myLocation);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenSpace.myLocation).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
          else { NotEnoughMoney(); return; }
        }
        else { CantChooseCoal(); return; }
      }
      else ObjectManager.ChooseTile(possibleSources[0]);
    }

    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(chosenNetworkSpace);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenNetworkSpace).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
          else NotEnoughMoney();
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
    List<IronWorksTileScript> possibleSources = ObjectManager.GetAllIronWorksWithFreeIron();
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
}
