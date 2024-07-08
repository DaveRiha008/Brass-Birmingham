using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChooseBetterTile : AIBehaviour
{


  public AIChooseBetterTile(Player playerWithThisBehaviour) : base(playerWithThisBehaviour)
  {
  }

  //public override void ChooseCard()
  //{
  //  List<CardScript> possibleCards = CardManager.GetPlayerCards(myAIPlayer.index);

  //  if (possibleCards.Count <= 0) { CantChooseCard(); return; }

  //  switch (ActionManager.currentAction)
  //  {
  //    case ACTION.BUILD:
  //      ChooseCardBuild(possibleCards);
  //      break;
  //    case ACTION.SELL:
  //    case ACTION.LOAN:
  //    case ACTION.SCOUT:
  //    case ACTION.DEVELOP:
  //    case ACTION.NETWORK:
  //    case ACTION.NONE:   //Everything other tha Build will want to eliminate the same card
  //      ChooseCardNonBuild(possibleCards);
  //      break;
  //  }

  //  CardManager.ChooseCardFromHand(chosenCard);
  //}

  void ChooseCardBuild(List<CardScript> possibleCards)
  //Choose the card which gives us the most opportunities
  {
    //Debug.Log("Choosing better card for build");

    int maxPossibleSpaces = 0;
    int maxPossibleTiles = 0;
    CardScript bestCard = possibleCards[0];

    foreach (CardScript card in possibleCards)
    {
      //Wild cards are always the best
      if (card.myType == CARD_TYPE.WILD_LOCATION || card.myType == CARD_TYPE.WILD_INDUSTRY)
      {
        bestCard = card;
        break;
      }
      if (ObjectManager.GetAllViableBuildSpaces(card).Count + ObjectManager.GetAllViableTiles(card).Count > maxPossibleSpaces + maxPossibleTiles)
        bestCard = card;
    }

    chosenCard = bestCard;
  }

  void ChooseCardNonBuild(List<CardScript> possibleCards)
  //Choose the card reverse to the one when building -> choose the most useless one, so the best ones remain for building
  {
    int minPossibleSpaces = int.MaxValue;
    int minPossibleTiles = int.MaxValue;
    CardScript bestCard = possibleCards[0];

    foreach (CardScript card in possibleCards)
    {
      //Don't ever throw away WILD CARDS - they are too expensive
      if (card.myType == CARD_TYPE.WILD_LOCATION || card.myType == CARD_TYPE.WILD_INDUSTRY)
      {
        if (possibleCards.Count > 1)
          continue;
        else
          bestCard = card;
      }
      if (ObjectManager.GetAllViableBuildSpaces(card).Count + ObjectManager.GetAllViableTiles(card).Count < minPossibleSpaces + minPossibleTiles)
        bestCard = card;
    }

    chosenCard = bestCard;
  }

  public override void ChooseWildCard()
  {
    int wildLocPoss = ObjectManager.GetViableBuildSpacesWildLocationCard().Count + ObjectManager.GetViableTilesWildLocationCard().Count;
    int wildIndPoss = ObjectManager.GetViableBuildSpacesWildIndustryCard().Count + ObjectManager.GetViableTilesWildIndustryCard().Count;

    if (wildLocPoss > wildIndPoss)
      CardManager.ActivePlayerDrawCard(CardDeckType.WILD_LOCATION);
    else
      CardManager.ActivePlayerDrawCard(CardDeckType.WILD_INDUSTRY);
  }
  public override void ChooseTile()
  {
    List<TileScript> possibleTiles = ObjectManager.GetViableTilesForCurrentAction();
    if (possibleTiles.Count <= 0)
    {
      if ((ActionManager.currentAction == ACTION.SELL || ActionManager.currentAction == ACTION.DEVELOP) && ActionManager.IsActionFinishable())
      { DoneAction(); return; }
      else
      { CantChooseTile(); return; }
    }

    //If developing - forward check whether we can afford the iron - later the action can't be finished
    if (ActionManager.currentAction == ACTION.DEVELOP)
    {
      List<IronWorksTileScript> possibleIronSources = ObjectManager.GetAllIronWorksWithFreeIron();
      if (possibleIronSources.Count <= 0 && !GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetIronStoragePrice()))
      {
        if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
        else { NotEnoughMoney(); return; }
      }
    }

    switch (ActionManager.currentAction)
    {
      case ACTION.BUILD:
        ChooseTileBuild(possibleTiles);
        break;
      case ACTION.SELL:
        ChooseTileSell(possibleTiles);
        break;
      case ACTION.DEVELOP:
        ChooseTileDevelop(possibleTiles);
        break;
      default:
        Debug.Log("Can't choose tile for different action than BUILD, SELL or DEVELOP");
        return;
    }
    //Debug.Log($"AI choosing {chosenTile} for {ActionManager.currentAction}");
    ObjectManager.ChooseTile(chosenTile);
  }

  void ChooseTileBuild(List<TileScript> possibleTiles)
  {
    float maxUtility = 0;
    TileScript bestTile = possibleTiles[0];
    foreach (TileScript tile in possibleTiles)
    {
      float costFraction = tile.buildCost / (float)myAIPlayer.money;
      float utility = tile.upgradeNetworkVicPtsReward + tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
      utility *= 1 - costFraction; //The more expensive tile has lower utility
      if (utility > maxUtility)
      {
        maxUtility = utility;
        bestTile = tile;
      }
    }

    chosenTile = bestTile;
  }
  void ChooseTileSell(List<TileScript> possibleTiles)
  {
    int maxUtility = 0;
    TileScript bestTile = possibleTiles[0];
    foreach (TileScript tile in possibleTiles)
    {
      int utility = tile.upgradeNetworkVicPtsReward + tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
      if (utility > maxUtility)
      {
        maxUtility = utility;
        bestTile = tile;
      }
    }

    chosenTile = bestTile;
  }
  void ChooseTileDevelop(List<TileScript> possibleTiles)
  {
    float maxUtility = float.MinValue;
    TileScript bestTile = possibleTiles[0];
    foreach (TileScript tile in possibleTiles)
    {
      float costFraction = tile.buildCost / (float)myAIPlayer.money;
      float curUtility = tile.upgradeNetworkVicPtsReward + tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
      curUtility *= 1 - costFraction; //The more expensive tile has lower utility


      TileScript nextLevelTile = ObjectManager.GetTileOfNextLevel(tile);
      float nextLevelUtility;
      if (nextLevelTile is null)
        nextLevelUtility = curUtility;
      else
      {
        float nextLevelCostFraction = nextLevelTile.buildCost / (float)myAIPlayer.money;
        nextLevelUtility = nextLevelTile.upgradeNetworkVicPtsReward + nextLevelTile.upgradeVicPtsReward + nextLevelTile.upgradeIncomeReward;
        nextLevelUtility *= 1 - costFraction; //The more expensive tile has lower utility

        //If the next level tile can't be built in the current era -> its utility is lowered
        if ((!nextLevelTile.canBeBuiltInBoatEra && GameManager.currentEra == ERA.BOAT) || (!nextLevelTile.canBeBuiltInTrainEra && GameManager.currentEra == ERA.TRAIN))
          nextLevelUtility -= 20;
      }


      float utility = nextLevelUtility - curUtility;

      //If the developed tile can't be built in current era -> develop utility goes up
      if (!tile.canBeBuiltInTrainEra && GameManager.currentEra == ERA.TRAIN)
        utility += 20;

      if (utility > maxUtility)
      {
        maxUtility = utility;
        bestTile = tile;
      }
    }

    chosenTile = bestTile;
  }

  //public override void ChooseIndustrySpace()
  //{
  //  List<IndustrySpace> possibleSpaces = ObjectManager.GetAllViableBuildSpaces(chosenCard);
  //  //Debug.Log($"Possible spaces: {possibleSpaces.Count}");
  //  if (possibleSpaces.Count <= 0) { CantChooseSpace(); return; }
  //  chosenSpace = possibleSpaces[0];
  //  ObjectManager.ChooseSpace(chosenSpace);
  //}

  //public override void ChooseIron()
  //{
  //  List<IronWorksTileScript> possibleSources = ObjectManager.GetAllIronWorksWithFreeIron();
  //  if (possibleSources.Count <= 0)
  //  {
  //    if (myAIPlayer.money >= ObjectManager.GetIronStoragePrice())
  //      ObjectManager.ChoseIronStorage();
  //    else
  //    { NotEnoughMoney(); return; }
  //  }
  //  else ObjectManager.ChooseTile(possibleSources[0]);
  //}

  //public override void ChooseCoal()
  //{
  //  List<CoalMineTileScript> possibleSources;
  //  if (ActionManager.currentAction == ACTION.BUILD)
  //  {
  //    possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(chosenSpace.myLocation);
  //    if (possibleSources.Count <= 0)
  //    {
  //      if (ObjectManager.GetAllConnectedMerchantTiles(chosenSpace.myLocation).Count > 0)
  //      {
  //        if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
  //        else { NotEnoughMoney(); return; }
  //      }
  //      else { CantChooseCoal(); return; }
  //    }
  //    else ObjectManager.ChooseTile(possibleSources[0]);
  //  }

  //  else if (ActionManager.currentAction == ACTION.NETWORK)
  //  {
  //    possibleSources = ObjectManager.GetNearestCoalMinesWithFreeCoal(chosenNetworkSpace);
  //    if (possibleSources.Count <= 0)
  //    {
  //      if (ObjectManager.GetAllConnectedMerchantTiles(chosenNetworkSpace).Count > 0)
  //      {
  //        if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice()) ObjectManager.ChoseCoalStorage();
  //        else NotEnoughMoney();
  //      }
  //      else { CantChooseCoal(); return; }
  //    }
  //    else ObjectManager.ChooseTile(possibleSources[0]);
  //  }

  //}

  //public override void ChooseBarrel()
  //{
  //  //Debug.Log("AI Choosing barrel");
  //  if (ActionManager.currentAction == ACTION.SELL)
  //  {
  //    List<BarrelSpace> merchantBarrels = ObjectManager.GetAllAvailableMerchantBarrels(chosenTile.builtOnSpace.myLocation, chosenTile); //Barrel from merchant
  //    if (merchantBarrels.Count > 0)
  //    {
  //      ObjectManager.BarrelSpaceClicked(merchantBarrels[0]); /*Debug.Log("AI Chose barrel succesfully");*/
  //      return;
  //    }

  //    List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenTile.builtOnSpace.myLocation); //Barrel from brewery
  //    if (breweriesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }
  //    //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");
  //    ObjectManager.ChooseTile(breweriesWithBarrels[0].myTile);

  //    //Debug.Log("AI Chose barrel succesfully");
  //  }
  //  else if (ActionManager.currentAction == ACTION.NETWORK)
  //  {
  //    List<IndustrySpace> breweriesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenNetworkSpace);
  //    if (breweriesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }
  //    //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");

  //    ObjectManager.ChooseTile(breweriesWithBarrels[0].myTile);
  //    //Debug.Log("AI Chose barrel succesfully");
  //  }
  //}

  //public override void ChooseNetwork()
  //{
  //  if (GameManager.currentEra == ERA.TRAIN && chosenNetworkSpace is not null &&
  //    !GameManager.ActivePlayerHasEnoughMoney(Constants.train2Cost) && ActionManager.IsActionFinishable())
  //  { DoneAction(); return; }

  //  List<NetworkSpace> possibleNetworks = ObjectManager.GetMyNetworkNeighborConnections(myAIPlayer.index);

  //  if (GameManager.currentEra == ERA.TRAIN) //For trains check coal availability
  //  {
  //    List<NetworkSpace> possibleNetworksWithCoals = new();
  //    foreach (NetworkSpace space in possibleNetworks)
  //    {
  //      if (CoalCheck(space))
  //      {
  //        if (chosenNetworkSpace is null || BarrelCheckTiles(space)) //For second train check barrel
  //          possibleNetworksWithCoals.Add(space);
  //      }
  //    }

  //    possibleNetworks = possibleNetworksWithCoals;
  //  }

  //  if (possibleNetworks.Count <= 0)
  //  {
  //    if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
  //    else { CantChooseNetwork(); return; }
  //  }
  //  chosenNetworkSpace = possibleNetworks[0];
  //  ObjectManager.ChoseNetwork(chosenNetworkSpace);
  //}
}
