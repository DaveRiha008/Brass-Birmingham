using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChooseBetter : AIBehaviour
{


  public AIChooseBetter(Player playerWithThisBehaviour) : base(playerWithThisBehaviour)
  {
  }

  public override void ChooseCard()
  {
    List<CardScript> possibleCards = GetPossibleCards();

    if (possibleCards.Count <= 0) { CantChooseCard(); return; }

    switch (ActionManager.currentAction)
    {
      case ACTION.BUILD:
        ChooseCardBuild(possibleCards);
        break;
      case ACTION.SELL:
      case ACTION.LOAN:
      case ACTION.SCOUT:
      case ACTION.DEVELOP:
      case ACTION.NETWORK:
      case ACTION.NONE:   //Everything other tha Build will want to eliminate the same card
        ChooseCardNonBuild(possibleCards);
        break;
    }

    CardManager.ChooseCardFromHand(chosenCard);
  }

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
    List<TileScript> possibleTiles = GetPossibleTiles();
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
      if (!IronCheck())
      {
        if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
        else { NotEnoughMoney(MISSING_MONEY_REASON.IRON); return; }
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

  float GetTileBuildUtility(TileScript tile)
  {
    float costFraction = tile.buildCost / (float)myAIPlayer.money;
    float utility = tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
    utility *= 1 - costFraction; //The more expensive tile has lower utility

    return utility;
  }

  void ChooseTileBuild(List<TileScript> possibleTiles)
  {
    float maxUtility = 0;
    TileScript bestTile = possibleTiles[0];
    foreach (TileScript tile in possibleTiles)
    {
      float costFraction = tile.buildCost / (float)myAIPlayer.money;
      float utility = GetTileBuildUtility(tile);
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
    foreach(TileScript tile in possibleTiles)
    {
      int utility = tile.upgradeNetworkVicPtsReward/2 + tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
      if(utility > maxUtility)
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
      float curUtility = GetTileBuildUtility(tile);


      TileScript nextLevelTile = ObjectManager.GetTileOfNextLevel(tile);
      float nextLevelUtility;
      if (nextLevelTile is null)
        nextLevelUtility = curUtility;
      else
      {
        nextLevelUtility = GetTileBuildUtility(nextLevelTile);

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

  public override void ChooseIndustrySpace()
  {
    //Industry space is chosen only in build action -> no need to specialize
    List<IndustrySpace> possibleSpaces = GetPossibleIndustrySpaces();


    //Debug.Log($"Possible spaces: {possibleSpaces.Count}");
    if (possibleSpaces.Count <= 0) { CantChooseSpace(); return; }

    float maxUtility = float.MinValue;
    IndustrySpace bestSpace = possibleSpaces[0];

    foreach(IndustrySpace space in possibleSpaces)
    {
      float spaceUtility = GetIndustrySpaceUtility(space);
      if(spaceUtility > maxUtility)
      {
        maxUtility = spaceUtility;
        bestSpace = space;
      }
    }

    chosenSpace = bestSpace;
    ObjectManager.ChooseSpace(chosenSpace);
  }

  float GetIndustrySpaceUtility(IndustrySpace space)
  {
    const float merchConnectedBonus = 5;

    float utility = 0f;
    INDUSTRY_TYPE bestTileType = INDUSTRY_TYPE.NONE;

    //If we haven't chosen a tile yet -> consider all possible tiles
    if (chosenTile is null)
    {
      if (space.hasBrew)
      {
        float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.BREWERY);
        if (potentialUtility > utility)
        {
          utility = potentialUtility;
          bestTileType = INDUSTRY_TYPE.BREWERY;
        }

      }
      if (space.hasCoal)
      {
        float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.COALMINE);
        if(potentialUtility > utility){
          utility = potentialUtility;
          bestTileType= INDUSTRY_TYPE.COALMINE;
        }
      }
      if (space.hasCot)
      { 
        float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.COTTONMILL);
        if(potentialUtility > utility){
          utility = potentialUtility;
          bestTileType= INDUSTRY_TYPE.COTTONMILL;
        }
      }
      if (space.hasIron)
      { 
        float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.IRONWORKS);
        if(potentialUtility > utility){
          utility = potentialUtility;
          bestTileType= INDUSTRY_TYPE.IRONWORKS;
        }
      }
      if (space.hasMan)
      { 
        float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.MANUFACTURER);
        if(potentialUtility > utility){
          utility = potentialUtility;
          bestTileType= INDUSTRY_TYPE.MANUFACTURER;
        }
      }
      if (space.hasPot)
      { 
       float potentialUtility = GetIndustrySpacePotentialTileUtility(space, INDUSTRY_TYPE.POTTERY);
        if(potentialUtility > utility){
          utility = potentialUtility;
          bestTileType= INDUSTRY_TYPE.POTTERY;
        }
      }

    }
    else {
      utility = GetTileBuildUtility(chosenTile);
      bestTileType = chosenTile.industryType;
    }

    // If the best tile is tradeable -> add utility if merchant is connected
    //  Or coalMine -> gives access to storage through merchant
    if (bestTileType == INDUSTRY_TYPE.COTTONMILL || bestTileType == INDUSTRY_TYPE.COALMINE || bestTileType == INDUSTRY_TYPE.POTTERY || bestTileType == INDUSTRY_TYPE.MANUFACTURER) 
    {
      List<MerchantTileSpace> connectedMerchants = ObjectManager.GetAllConnectedMerchantTiles(space.myLocation);

      if (bestTileType == INDUSTRY_TYPE.COALMINE && connectedMerchants.Count > 0) utility += merchConnectedBonus;

      else
      {
        foreach (MerchantTileSpace merch in connectedMerchants)
        {
          if (merch.myTile is null) continue;
          if ((bestTileType == INDUSTRY_TYPE.COTTONMILL && merch.myTile.hasCot) ||
            (bestTileType == INDUSTRY_TYPE.MANUFACTURER && merch.myTile.hasMan) ||
            (bestTileType == INDUSTRY_TYPE.POTTERY && merch.myTile.hasPottery))
          {
            utility += merchConnectedBonus;
            break;
          }
          else continue;
        }
      }
    }
    

    //If we are overbuilding - lower utility if current tile is ours, add utility if it isn't
    if (space.myTile is not null)
    {
      if(space.myTile.ownerPlayerIndex == myAIPlayer.index)
      {
        utility -= GetOverBuiltTileUtility(space.myTile);
      }

      else
      {
        utility += GetOverBuiltTileUtility(space.myTile) / 4;
      }

    }

    return utility;
  }

  float GetIndustrySpacePotentialTileUtility(IndustrySpace space, INDUSTRY_TYPE type)
  {
    switch (type)
    {
      case INDUSTRY_TYPE.BREWERY:
        if (space.hasBrew)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.BREWERY, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.COALMINE:
        if (space.hasCoal)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.COALMINE, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.COTTONMILL:
        if (space.hasCot)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.COTTONMILL, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.IRONWORKS:
        if (space.hasIron)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.IRONWORKS, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.MANUFACTURER:
        if (space.hasMan)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.MANUFACTURER, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.POTTERY:
        if (space.hasPot)
        {
          TileScript potentialBuiltTile = ObjectManager.GetLowestLevelTileOfType(INDUSTRY_TYPE.POTTERY, myAIPlayer.index);
          if (potentialBuiltTile is not null)
            return GetTileBuildUtility(potentialBuiltTile);
        }
        return 0;
      case INDUSTRY_TYPE.NONE:
        break;
      default:
        break;
    }
    return 0;
  }

  float GetOverBuiltTileUtility(TileScript tile)
  {
    float value = tile.upgradeVicPtsReward + tile.upgradeIncomeReward;
    if (tile.isUpgraded) value *= 1;
    else value *= 0.5f;


    return value;
  }

  public override void ChooseIron()
  {
    //Choose mainly our own iron

    List<IronWorksTileScript> possibleSources = GetPossibleIronSources();
    if (possibleSources.Count <= 0)
    {
      if (myAIPlayer.money >= ObjectManager.GetIronStoragePrice())
      {
        ObjectManager.ChoseIronStorage();
        return;
      }
      else
      { NotEnoughMoney(MISSING_MONEY_REASON.IRON); return; }
    }


    List<IronWorksTileScript> mySources = new();
    List<IronWorksTileScript> otherSources = new();

    foreach (IronWorksTileScript source in possibleSources)
    {
      if (source.ownerPlayerIndex == myAIPlayer.index)
        mySources.Add(source);

      else
        otherSources.Add(source);
    }

    IronWorksTileScript bestSource = possibleSources[0];
    if (mySources.Count > 0) //If we have sources -> we want the emptied -> get the one with least resources
    {
      int minRes = int.MaxValue;
      foreach (IronWorksTileScript source in mySources)
        if(source.GetResourceCount() < minRes)
        {
          minRes = source.GetResourceCount();
          bestSource = source;
        }
    }
    else //If we don't have any source, we don't want to upgrade any other -> get the one with most resources
    {
      int maxRes = int.MinValue;
      foreach (IronWorksTileScript source in mySources)
        if (source.GetResourceCount() > maxRes)
        {
          maxRes = source.GetResourceCount();
          bestSource = source;
        }
    }

    ObjectManager.ChooseTile(bestSource);
  }

  public override void ChooseCoal()
  {
    List<CoalMineTileScript> possibleSources;
    if (ActionManager.currentAction == ACTION.BUILD)
    {
      possibleSources = GetPossibleCoalSources(chosenSpace.myLocation);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenSpace.myLocation).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice())
          {
            ObjectManager.ChoseCoalStorage();
            return;
          }
          else { NotEnoughMoney(MISSING_MONEY_REASON.COAL); return; }
        }
        else { CantChooseCoal(); return; }
      }
    }

    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      possibleSources = GetPossibleCoalSources(chosenNetworkSpace);
      if (possibleSources.Count <= 0)
      {
        if (ObjectManager.GetAllConnectedMerchantTiles(chosenNetworkSpace).Count > 0)
        {
          if (myAIPlayer.money >= ObjectManager.GetCoalStoragePrice())
          {
            ObjectManager.ChoseCoalStorage();
            return;
          }
          else { NotEnoughMoney(MISSING_MONEY_REASON.COAL); return; }
        }
        else { CantChooseCoal(); return; }
      }
    }

    else
    {
      Debug.LogError("AI can't choose coal in other action than build or network");
      return;
    }


    List<CoalMineTileScript> mySources = new();
    List<CoalMineTileScript> otherSources = new();

    foreach (CoalMineTileScript source in possibleSources)
    {
      if (source.ownerPlayerIndex == myAIPlayer.index)
        mySources.Add(source);

      else
        otherSources.Add(source);
    }

    CoalMineTileScript bestSource = possibleSources[0];
    if (mySources.Count > 0) //If we have sources -> we want the emptied -> get the one with least resources
    {
      int minRes = int.MaxValue;
      foreach (CoalMineTileScript source in mySources)
        if (source.GetResourceCount() < minRes)
        {
          minRes = source.GetResourceCount();
          bestSource = source;
        }
    }
    else //If we don't have any source, we don't want to upgrade any other -> get the one with most resources
    {
      int maxRes = int.MinValue;
      foreach (CoalMineTileScript source in mySources)
        if (source.GetResourceCount() > maxRes)
        {
          maxRes = source.GetResourceCount();
          bestSource = source;
        }
    }

    ObjectManager.ChooseTile(bestSource);

  }

  public override void ChooseBarrel()
  {
    //Debug.Log("AI Choosing barrel");
    List<BreweryTileScript> breweriesWithBarrels = new();
    if (ActionManager.currentAction == ACTION.SELL)
    {
      List<BarrelSpace> merchantBarrels = ObjectManager.GetAllAvailableMerchantBarrels(chosenTile.builtOnSpace.myLocation, chosenTile); //Barrel from merchant
      if (merchantBarrels.Count > 0)
      {
        ObjectManager.BarrelSpaceClicked(merchantBarrels[0]); /*Debug.Log("AI Chose barrel succesfully");*/
        return;
      }

      List<IndustrySpace> brewerySpacesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenTile.builtOnSpace.myLocation); //Barrel from brewery

      if (brewerySpacesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }


      foreach (IndustrySpace space in brewerySpacesWithBarrels)
      {
        if (space.myTile is not BreweryTileScript)
        {
          Debug.LogError("Found space with barrel which is not Brewery");
          return;
        }
        breweriesWithBarrels.Add((BreweryTileScript)space.myTile);
      }
      //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");

      //Debug.Log("AI Chose barrel succesfully");
    }
    else if (ActionManager.currentAction == ACTION.NETWORK)
    {
      List<IndustrySpace> brewerySpacesWithBarrels = ObjectManager.GetAllSpacesWithAvailableBarrels(chosenNetworkSpace);
      if (brewerySpacesWithBarrels.Count <= 0) { CantChooseBarrel(); return; }


      foreach (IndustrySpace space in brewerySpacesWithBarrels)
      {
        if (space.myTile is not BreweryTileScript)
        {

          Debug.LogError($"Found space {space} with barrel which is not Brewery but {space.myTile}");
          return;
        }
        breweriesWithBarrels.Add((BreweryTileScript)space.myTile);
      }
      //Debug.Log($"Choosing {breweriesWithBarrels[0]} as barrel source");

      //Debug.Log("AI Chose barrel succesfully");
    }
    else
    {
      Debug.LogError("AI can't choose barrel in other action than sell or network!");
      return;
    }


    List<BreweryTileScript> myBreweries = new();
    List<BreweryTileScript> otherBreweries = new();

    foreach (BreweryTileScript brewery in breweriesWithBarrels)
    {
      if (brewery.ownerPlayerIndex == myAIPlayer.index)
        myBreweries.Add(brewery);

      else
        otherBreweries.Add(brewery);
    }

    BreweryTileScript bestBrewery = breweriesWithBarrels[0];
    if (myBreweries.Count > 0) //If we have sources -> we want the emptied -> get the one with least resources
    {
      int minRes = int.MaxValue;
      foreach (BreweryTileScript brewery in myBreweries)
        if (brewery.GetResourceCount() < minRes)
        {
          minRes = brewery.GetResourceCount();
          bestBrewery = brewery;
        }
    }
    else //If we don't have any source, we don't want to upgrade any other -> get the one with most resources
    {
      int maxRes = int.MinValue;
      foreach (BreweryTileScript brewery in myBreweries)
        if (brewery.GetResourceCount() > maxRes)
        {
          maxRes = brewery.GetResourceCount();
          bestBrewery = brewery;
        }
    }

    ObjectManager.ChooseTile(bestBrewery);

  }

  public override void ChooseNetwork()
  {
    if (GameManager.currentEra == ERA.TRAIN && chosenNetworkSpace is not null &&
      !GameManager.ActivePlayerHasEnoughMoney(Constants.train2Cost) && ActionManager.IsActionFinishable())
    { DoneAction(); return; }


    List<NetworkSpace> possibleNetworks = GetPossibleNetworkSpaces();

    if (possibleNetworks.Count <= 0)
    {
      if (ActionManager.IsActionFinishable()) { DoneAction(); return; }
      else { CantChooseNetwork(); return; }
    }

    float maxUtility = float.MinValue;
    NetworkSpace bestNetworkSpace = possibleNetworks[0];

    foreach (NetworkSpace network in possibleNetworks)
    {
      float curUtility = GetNetworkSpaceUtility(network);
      if(curUtility > maxUtility)
      {
        maxUtility = curUtility;
        bestNetworkSpace = network;
      }
    }

    chosenNetworkSpace = bestNetworkSpace;
    ObjectManager.ChoseNetwork(chosenNetworkSpace);
  }

  float GetNetworkSpaceUtility(NetworkSpace space)
  {
    const float utilityRemovedForDistToMerchant = 0.1f;
    const float utilityForConnectingMerchant = 5;

    float utility = 0;

    //Add utility for each built tile on adjacent locations -> potential points for network
    foreach (LocationScript location in space.connectsLocations)
    {
      if (location.myType == LocationType.CITY)
        foreach (IndustrySpace indSpace in ObjectManager.GetAllIndustrySpacesInLocation(location))
        {
          if (indSpace.myTile is not null)
          {
            float tileUtility = indSpace.myTile.upgradeNetworkVicPtsReward;
            if (!indSpace.myTile.isUpgraded) tileUtility = tileUtility / 2f;


            //Add utility for new source of resources
            if (!ObjectManager.IsLocationPartOfMyNetwork(location, myAIPlayer.index))
            {
              if (indSpace.myTile.industryType == INDUSTRY_TYPE.BREWERY) tileUtility += 1; //Add utility for brewery -> new available barrel source
              else if (indSpace.myTile.industryType == INDUSTRY_TYPE.COALMINE) tileUtility += 0.5f; //Add utility for coalMine -> new available coal source
            }
            utility += tileUtility;
          }
        }
      else
        utility += utilityForConnectingMerchant;
    }


    //Remove a little utility for distance to nearest unconnected merchant -> network leading to merchant connection is better
    ObjectManager.GetNearestUnconnectedMerchant(space, out int distToNearestUnconnectedMerchant);

    utility -= distToNearestUnconnectedMerchant * utilityRemovedForDistToMerchant;

    return utility;

  }
}
