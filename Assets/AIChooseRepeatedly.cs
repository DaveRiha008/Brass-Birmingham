using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChooseRepeatedly : AIChooseBetter
{
  protected List<CardScript> blockedCards = new();
  protected List<TileScript> blockedTiles = new();
  protected List<IndustrySpace> blockedIndSpaces= new();
  protected List<NetworkSpace> blockedNetSpaces = new();

  CardScript previouslyChosenCard = null;
  TileScript previouslyChosenTile = null;
  IndustrySpace previouslyChosenIndSpace = null;
  NetworkSpace previouslyChosenNetSpace = null;

  const int maxResets = 50;
  int resetCounter = 0;

  public AIChooseRepeatedly(Player playerWithThisBehaviour) : base(playerWithThisBehaviour)
  {
  }



  void BlockCardAndReset()
  {
    blockedCards.Add(chosenCard);
    chosenCard = null;
    ResetAction();
  }
  void BlockTileAndReset()
  {
    blockedTiles.Add(chosenTile);
    chosenTile = null;
    ResetAction();
  }
  void BlockIndSpaceAndReset()
  {
    blockedIndSpaces.Add(chosenSpace);
    chosenSpace = null;
    ResetAction();
  }
  void BlockNetSpaceAndReset()
  {
    blockedNetSpaces.Add(chosenNetworkSpace);
    chosenNetworkSpace = null;
    ResetAction();
  }

  protected override void CantChooseTile()
  {
    //Debug.Log("AI couldnt choose Tile");
    blockedTiles.Clear(); //Going up in recursion -> all tiles should be available in next try 

    //Tile restricted by space
    if (ActionManager.currentAction == ACTION.BUILD)
    {
      if (chosenSpace is not null)
      {
        BlockIndSpaceAndReset();
        return;
      }
      else
      {
        BlockCardAndReset();
        return;
      }
    }

    //No other action with tileChoosing can be finished if tile can't be chosen
      //Develop -> before only card - doesn't change anything
      //Sell -> same
    else
    {
      CancelAction();
      return;
    }
  }

  protected override void CantChooseSpace()
  {
    //Debug.Log("AI couldnt choose Space");
    blockedIndSpaces.Clear(); //Going up in recursion -> all spaces should be available in next try

    if (ActionManager.currentAction == ACTION.BUILD)
    {
      if(chosenTile is not null)
      {
        BlockTileAndReset();
        return;
      }

      else
      {
        BlockCardAndReset();
        return;
      }
    }

    //This should never occur -> space is chosen only in BUILD action
    else
    {
      Debug.Log("Choosing space for other action than build? Weird");
      CancelAction();
      return;
    }
  }
  protected override void CantChooseBarrel()
  {
    Debug.Log("AI couldnt choose Barrel");


    //This shouldn't occur -> tiles without available barrels are not considered viable
    if (ActionManager.currentAction == ACTION.SELL)
    {
      BlockTileAndReset();
      return;
    }

    //This also should never occur -> second network spaces should be checked before chosen -> second network without available barrel is never chosen
    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      BlockNetSpaceAndReset();
      return;
    }

    //This should never occur - barrels are required only in network and sell actions
    else
    {
      Debug.Log("Choosing barrel for other action than network or sell? Weird");
      CancelAction();
      return;
    }
  }
  //No need to override Card unavailability -> no action can be finished without card -> cards are the top level -> always CancelAction()

  protected override void CantChooseCoal()
  {
    //Debug.Log("AI couldnt choose Coal");

    if (ActionManager.currentAction == ACTION.BUILD)
    {
      //Differ between chosen cards -> they determine whether tile or space is chosen first

      //If space was chosen first
      if(chosenCard.myType == CARD_TYPE.LOCATION || chosenCard.myType == CARD_TYPE.WILD_LOCATION)
      {
        BlockTileAndReset();
        return;
      }

      //If tile was chosen first
      else
      {
        BlockIndSpaceAndReset();
        return;
      }
    }

    //This should never occur -> network in train era without available coal source is not considered possible
    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      BlockNetSpaceAndReset();
      return;
    }

    //This should never occur -> coal is chosen only in BUILD or NETWORK action

    else
    {
      Debug.Log("Choosing coal for other action than build or network? Weird");
      CancelAction();
      return;
    }
  }
  
  //No need to override Network unavailability -> if there is no network to be chosen, different card won't help


  protected override void NotEnoughMoney(MISSING_MONEY_REASON reason)
  {
    //Debug.Log("AI had no Money");

    switch (reason)
    {
      case MISSING_MONEY_REASON.COAL:
        CantChooseCoal();
        break;
      case MISSING_MONEY_REASON.IRON:
        if(ActionManager.currentAction == ACTION.BUILD)
        {
          // No need to differ between chosen cards -> we need different tile every time ->
          //  different space doesn't change anything since iron is not dependent on connectivity

          BlockTileAndReset();
        }

        //Iron is chosen only in build and develop -> and in develop we want to Cancel anyways
        else
        {
          CancelAction();
        }
        break;
      case MISSING_MONEY_REASON.NETWORK_VEHICLE:
        // This means that we don't have money on the first vehicle (2nd train is not chosen id we don't have money for it)
        // We can't do anything - Different card doesn't change anything and we haven't chosen anything else
        CancelAction();
        break;
      default:
        CancelAction();
        break;
    }

  }

  // No need to override NotEnoughIncome -> only called in loan action and we can't choose anything there


  protected override List<CardScript> GetPossibleCards()
  {
    List<CardScript> possibleCards = CardManager.GetPlayerCards(myAIPlayer.index);
    List<CardScript> viableCards = new();
    foreach (CardScript card in possibleCards)
      if (!blockedCards.Contains(card)) viableCards.Add(card);

    return viableCards;
  }
  protected override List<TileScript> GetPossibleTiles()
  {
    List<TileScript> possibleTiles = ObjectManager.GetViableTilesForCurrentAction();
    List<TileScript> viableTiles = new();
    foreach (TileScript tile in possibleTiles)
      if (!blockedTiles.Contains(tile)) viableTiles.Add(tile);

    return viableTiles;
  }

  protected override List<IndustrySpace> GetPossibleIndustrySpaces()
  {
    List<IndustrySpace> possibleSpaces;
    if (chosenTile is not null)
      possibleSpaces = ObjectManager.GetMyNetworkFreeSpacesForItemInHand(myAIPlayer.index);

    else
      possibleSpaces = ObjectManager.GetAllViableBuildSpaces(chosenCard);

    List<IndustrySpace> viableSpaces = new();
    foreach (IndustrySpace space in possibleSpaces)
      if (!blockedIndSpaces.Contains(space)) viableSpaces.Add(space);

    return viableSpaces;
  }

  //Resource sources don't need to be overriden ->
  //        By choosing different resource we won't change the finishibility of action

  protected override List<NetworkSpace> GetPossibleNetworkSpaces()
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

    List<NetworkSpace> viableNetworks = new();
    foreach (NetworkSpace network in possibleNetworks)
      if (!blockedNetSpaces.Contains(network)) viableNetworks.Add(network);

    return viableNetworks;
  }

  void ResetAction()
  {
    previouslyChosenCard = chosenCard;
    previouslyChosenTile = chosenTile;
    previouslyChosenIndSpace = chosenSpace;
    previouslyChosenNetSpace = chosenNetworkSpace;

    chosenCard = null;
    chosenSpace = null;
    chosenTile = null;
    chosenNetworkSpace = null;

    resetCounter++;

    //Debug.Log($"Reseting {ActionManager.currentAction} for {resetCounter}th time");
    if(resetCounter> maxResets)
    {
      CancelAction();
      return;
    }

    ActionManager.CancelAction();
  }

  //Override Choosing -> if we have restarted we can choose everything based on previous action
  public override void ChooseCard()
  {
    if (previouslyChosenCard is not null)
    {
      chosenCard = previouslyChosenCard;
      previouslyChosenCard = null;
      CardManager.ChooseCardFromHand(chosenCard);
    }
    else
      base.ChooseCard();
  }
  public override void ChooseTile()
  {
    if (previouslyChosenTile is not null)
    {
      chosenTile = previouslyChosenTile;
      previouslyChosenTile = null;
      ObjectManager.ChooseTile(chosenTile);
    }
    else
      base.ChooseTile();
  }

  public override void ChooseIndustrySpace()
  {
    if (previouslyChosenIndSpace is not null)
    {
      chosenSpace = previouslyChosenIndSpace;
      previouslyChosenIndSpace = null;
      ObjectManager.ChooseSpace(chosenSpace);
    }
    else
      base.ChooseIndustrySpace();
  }
  public override void ChooseNetwork()
  {
    if (previouslyChosenNetSpace is not null)
    {
      chosenNetworkSpace = previouslyChosenNetSpace;
      previouslyChosenNetSpace = null;
      ObjectManager.ChoseNetwork(chosenNetworkSpace);
    }
    else
      base.ChooseNetwork();
  }


  protected override void CancelAction()
  {
    ClearMemory();

    base.CancelAction();
  }

  protected override void DoneAction()
  {
    ClearMemory();

    base.DoneAction();
  }

  void ClearMemory()
  {
    blockedCards.Clear();
    blockedTiles.Clear();
    blockedIndSpaces.Clear();
    blockedNetSpaces.Clear();

    previouslyChosenCard = null;
    previouslyChosenTile = null;
    previouslyChosenIndSpace = null;
    previouslyChosenNetSpace = null;


    resetCounter = 0;

  }

}
