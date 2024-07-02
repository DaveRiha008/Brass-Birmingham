using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
  public static ActionManager instance;

  public static ACTION currentAction = ACTION.NONE;
  public static ACTION_STATE currentState = ACTION_STATE.NONE;
  public static ACTION_MISSING_RESOURCE curMisRes = ACTION_MISSING_RESOURCE.NONE;



  //TODO: Actions and their logical system
  static public int vicPtsBeforeAction = 0;
  static public int incomeBeforeAction = 0;
  static public int moneyBeforeAction = 0;
  static public int moneySpentThisTurnBeforeAction = 0;

  static public int actionsPlayed = 0;

  static public int developActionsToDo = 0;


  static int cardsChosen = 0;

  void Start()
  {

  }

  private void Awake()
  {
    CreateSingleton();
  }

  void CreateSingleton()
  {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);


    DontDestroyOnLoad(gameObject);
  }




  static public void DoAction(ACTION action)
  {
    if (PlayerDoneAllActions()) return;
    CancelAction();
    switch (action)
    {
      case ACTION.BUILD:
        currentAction = ACTION.BUILD;
        BuildAction();
        break;
      case ACTION.SELL:
        currentAction = ACTION.SELL;
        SellAction();
        break;
      case ACTION.LOAN:
        currentAction = ACTION.LOAN;
        LoanAction();
        break;
      case ACTION.SCOUT:
        currentAction = ACTION.SCOUT;
        ScoutAction();
        break;
      case ACTION.DEVELOP:
        currentAction = ACTION.DEVELOP;
        DevelopAction();
        break;
      case ACTION.NETWORK:
        currentAction = ACTION.NETWORK;
        NetworkAction();
        break;
      case ACTION.NONE:
        currentAction = ACTION.NONE;
        break;
      default:
        break;
    }
  }

  static public void ChooseCardForAction()
  {
    CardManager.canChooseCard = true;
    List<CardScript> possibleCards = CardManager.GetPlayerCards(GameManager.activePlayerIndex);

    if (possibleCards.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.CARD;
    
    CardManager.HighlightCards(possibleCards);
    //Debug.Log("Setting action state to CHOOSING_CARD");
    currentState = ACTION_STATE.CHOOSING_CARD;


    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToCardPreviewHand();

  }

  static public void ChoseCard(CardScript card)
  {
    currentState = ACTION_STATE.NONE;
    CardManager.DestroyAllBorders();
    CardManager.canChooseCard = false;
    if (currentAction == ACTION.SCOUT)
    {
      cardsChosen++;
      if (cardsChosen < 3)
      {
        ChooseCardForAction();
        return;
      }
    }
    cardsChosen = 0;
    switch (currentAction)
    {
      case ACTION.BUILD:
        BuildWithCard(card);
        break;
      case ACTION.SELL:
        SellAfterCard();
        break;
      case ACTION.LOAN:
        LoanAfterCard();
        break;
      case ACTION.SCOUT:
        ScoutAfterCards();
        break;
      case ACTION.DEVELOP:
        DevelopAfterCard();
        break;
      case ACTION.NETWORK:
        NetworkAfterCard();
        break;
      case ACTION.NONE:
        //Debug.Log("Card discarded for no action");
        CancelAction(true);
        if (GameManager.GameRunning())
        {
          Debug.Log("Calling EndTurn from empty action -> after discard");
          GameManager.EndTurn();

        }
        break;
      default:
        break;
    }
  }

  static void BuildAction()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToPersonalBoard();

    ChooseCardForAction();
  }

  static void BuildWithCard(CardScript card)
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();

    if (card.myType == CARD_TYPE.LOCATION || card.myType == CARD_TYPE.WILD_LOCATION)
    {
      camera.MoveToMainBoard();

      currentState = ACTION_STATE.CHOOSING_SPACE;

      List<IndustrySpace> viableSpaces = ObjectManager.GetAllViableBuildSpaces(card);

      if (viableSpaces.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.SPACE_BUILD;

      ObjectManager.HighlightIndustrySpaces(viableSpaces);
      ObjectManager.MakeCorrectIndustrySpacesClickable(card);
      ObjectManager.canChooseSpace = true;
    }
    else
    {
      camera.MoveToPersonalBoard();

      currentState = ACTION_STATE.CHOOSING_TILE;

      List<TileScript> viableTiles = ObjectManager.GetAllViableTiles(card);

      if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_BUILD;

      ObjectManager.HighlightTiles(viableTiles);
      ObjectManager.canPickUpTile = true;
    }

  }

  static public void BuildPickedUpItem()
  {

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    if (ObjectManager.chosenBuildSpace is not null)
    {
      currentState = ACTION_STATE.NONE;

      BuildSpaceAndTileChosen();
      return;
    }

    currentState = ACTION_STATE.CHOOSING_SPACE;
    ObjectManager.DestroyAllSpaceBorders();
    ObjectManager.canChooseSpace = true;
    ObjectManager.MakeCorrectIndustrySpacesClickable();
    List<IndustrySpace> viableSpaces = ObjectManager.GetMyNetworkFreeSpacesForItemInHand(GameManager.activePlayerIndex);

    if (viableSpaces.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.SPACE_BUILD;

    ObjectManager.HighlightIndustrySpaces(viableSpaces);
  }

  static public void BuildChoseIndustrySpace()
  {

    if (ObjectManager.itemInHand is not null)
    {
      currentState = ACTION_STATE.NONE;

      BuildSpaceAndTileChosen();
      return;
    }

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToPersonalBoard();

    currentState = ACTION_STATE.CHOOSING_TILE;
    ObjectManager.DestroyAllTileBorders();
    ObjectManager.canPickUpTile = true;
    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_BUILD;

    ObjectManager.HighlightTiles(viableTiles);
  }
  static void BuildRequireCoal()
  {
    currentState = ACTION_STATE.CHOOSING_COAL;

    if (ObjectManager.GetNearestCoalMinesWithFreeCoal(ObjectManager.chosenBuildSpace.myLocation).Count <= 0)
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(ObjectManager.chosenBuildSpace.myLocation).Count > 0) //Check whether buildSpace is connected to any merchant
      {
        if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetCoalStoragePrice()))
        {
          ObjectManager.MakeCoalStorageClickable();
          ObjectManager.HighlightCoalStorage();
        }
        else curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL;
      } 
      else // Else there is not any coal to get
        curMisRes = ACTION_MISSING_RESOURCE.COAL;
    }
    else
    {
      ObjectManager.UpdateNearestFreeCoalTiles(ObjectManager.chosenBuildSpace.myLocation);
      ObjectManager.HighlightNearestFreeCoalSpaces();
    }
  }

  static void BuildRequireIron()
  {
    currentState = ACTION_STATE.CHOOSING_IRON;

    if (ObjectManager.GetAllIronWorksWithFreeIron().Count <= 0)
    {
      if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetIronStoragePrice()))
      {
        ObjectManager.MakeIronStorageClickable();
        ObjectManager.HighlightIronStorage();
      }
      else
        curMisRes = ACTION_MISSING_RESOURCE.MONEY_IRON;
    }
    else
    {
      ObjectManager.UpdateFreeIronTiles();
      ObjectManager.HighlightFreeIronSpaces();
    }
  }


  static public void BuildSpaceAndTileChosen()
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.canChooseSpace = false;
    ObjectManager.canPickUpTile = false;

    ObjectManager.DestroyAllBorders();

    int requiredCoal = ObjectManager.itemInHand.buildCoalReq;
    int requiredIron = ObjectManager.itemInHand.buildIronReq;

    if (requiredCoal > 0)
    {
      ObjectManager.buildCoalReq = ObjectManager.itemInHand.buildCoalReq;

      BuildRequireCoal();
    }

    else if (requiredIron > 0)
    {
      ObjectManager.buildIronReq = ObjectManager.itemInHand.buildIronReq;

      BuildRequireIron();
    }

    else EndBuildAction();

  }
  static List<CoalMineTileScript> buildCoalSources = new();
  static public void BuildChoseCoal(CoalMineTileScript coalSource)
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.buildCoalReq--;

    buildCoalSources.Add(coalSource);

    ObjectManager.DestroyAllBorders();

    if (ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }

    else if (ObjectManager.buildIronReq > 0)
    {
      ObjectManager.buildIronReq = ObjectManager.itemInHand.buildIronReq;


      BuildRequireIron();
    }

    else EndBuildAction();

  }

  static int buildCoalTakenFromStorage = 0;
  static public void BuildChoseCoalStorage()
  {
    currentState = ACTION_STATE.NONE;

    buildCoalTakenFromStorage++;
    var coalFromStorage = ObjectManager.GetCoalFromStorage(out int coalPrice);

    coalFromStorage.GetComponent<SpriteRenderer>().enabled = false;
    coalFromStorage.SetActive(false);
    //Debug.Log($"Got coal from storage for {coalPrice}");
    GameManager.ActivePlayerSpendMoney(coalPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL;
      Debug.Log("Not enough money to buy coal from storage");
      CancelAction();
    }

    ObjectManager.buildCoalReq--;

    ObjectManager.DestroyAllBorders();

    if (ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }

    else if (ObjectManager.buildIronReq > 0)
    {
      ObjectManager.buildIronReq = ObjectManager.itemInHand.buildIronReq;

      BuildRequireIron();
    }

    else EndBuildAction();
  }

  static List<IronWorksTileScript> buildIronSources = new();
  static public void BuildChoseIron(IronWorksTileScript ironSource)
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.buildIronReq--;

    buildIronSources.Add(ironSource);

    ObjectManager.DestroyAllBorders();

    if (ObjectManager.buildIronReq > 0)
    {
      BuildRequireIron();
    }

    else
    {
      EndBuildAction();
    }
  }

  static int buildIronTakenFromStorage = 0;
  static public void BuildChoseIronStorage()
  {
    currentState = ACTION_STATE.NONE;

    buildIronTakenFromStorage++;
    var ironFromStorage = ObjectManager.GetIronFromStorage(out int ironPrice);
    ironFromStorage.SetActive(false);
    GameManager.ActivePlayerSpendMoney(ironPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_IRON;
      Debug.Log("Not enough money to buy iron from storage");
      CancelAction();
    }

    ObjectManager.buildIronReq--;

    ObjectManager.DestroyAllBorders();

    if (ObjectManager.buildIronReq > 0)
    {
      BuildRequireIron();
    }

    else
    {
      EndBuildAction();
    }
  }


  static void EndBuildAction()
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.BuildHeldIndustry();

    if (ObjectManager.overBuiltTile is not null)
      ObjectManager.RemoveTile(ObjectManager.overBuiltTile);

    CancelAction(true);
  }

  static List<TileScript> recentlySoldTiles = new();
  static TileScript tileToBeSold = null;
  static void SellAction()
  {
    ChooseCardForAction();
  }

  static void SellAfterCard()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    currentState = ACTION_STATE.CHOOSING_TILE;

    ObjectManager.canSellTile = true;
    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL;

    ObjectManager.HighlightTiles(viableTiles);
  }

  static public void SellChoseTile(TileScript tile)
  {
    ObjectManager.canSellTile = false;
    ObjectManager.sellReqBarrel = true;
    tileToBeSold = tile;
    ObjectManager.DestroyAllBorders();

    currentState = ACTION_STATE.CHOOSING_BARREL;

    ObjectManager.FillBarrelsForSell(tile.builtOnSpace.myLocation, tile);
    
    if (ObjectManager.GetAllAvailableMerchantBarrels(tile.builtOnSpace.myLocation, tile).Count <= 0 &&
      ObjectManager.GetAllSpacesWithAvailableBarrels(tile.builtOnSpace.myLocation).Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.BARREL;

    ObjectManager.HighlightBarrelsForSell();

  }

  static List<BarrelSpace> merchBarrelsSpaceUsedToSell = new();
  static public void SellChoseBarrel(BarrelSpace barrelMerchSpace)
  {
    merchBarrelsSpaceUsedToSell.Add(barrelMerchSpace);

    ObjectManager.SellTile(tileToBeSold);
    recentlySoldTiles.Add(tileToBeSold);
    tileToBeSold = null;
    ObjectManager.canSellTile = true;
    ObjectManager.sellReqBarrel = false;

    ObjectManager.ClearBarrelsForSell();
    ObjectManager.DestroyAllBorders();

    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL;

    currentState = ACTION_STATE.CHOOSING_TILE;
    ObjectManager.HighlightTiles(viableTiles);
    //EndSellAction(); //Uncomment for a single time sell action
  }

  static List<BreweryTileScript> tileBarrelsUsedToSell = new();

  static public void SellChoseBarrel(BreweryTileScript barrelTile)
  {

    tileBarrelsUsedToSell.Add(barrelTile);

    ObjectManager.SellTile(tileToBeSold);
    recentlySoldTiles.Add(tileToBeSold);
    tileToBeSold = null;
    ObjectManager.canSellTile = true;
    ObjectManager.sellReqBarrel = false;

    ObjectManager.ClearBarrelsForSell();
    ObjectManager.DestroyAllBorders();
    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL;
    currentState = ACTION_STATE.CHOOSING_TILE;

    ObjectManager.HighlightTiles(viableTiles);
    //EndSellAction(); //Uncomment for a single time sell action
  }

  static void EndSellAction()
  {
    ObjectManager.canSellTile = false;
    CancelAction(true);
  }

  static void LoanAction()
  {

    if (GameManager.GetActivePlayer().income < 3)

    {
      curMisRes = ACTION_MISSING_RESOURCE.INCOME_LOAN;
      Debug.Log("Not enough income for loan action!");
      CancelAction();
      return;
    }

    ChooseCardForAction();

  }

  static public void LoanAfterCard()
  {
    currentState = ACTION_STATE.NONE;

    GameManager.ActivePlayerGainMoney(Constants.loanMoney);
    GameManager.PlayerLoseIncomeLevel(GameManager.activePlayerIndex, Constants.loanIncomeCost);

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();
    CancelAction(true);
  }

  static void ScoutAction()
  {

    if (CardManager.PlayerHasWildCard(GameManager.activePlayerIndex))
    {
      curMisRes = ACTION_MISSING_RESOURCE.WILD_CARD_ALREADY_IN_HAND;
      Debug.Log("Can't scout when wild card is already in hand");
      CancelAction();
      return;
    }

    ChooseCardForAction();
  }

  static void ScoutAfterCards()
  {
    CardManager.canDrawWildCards = true;
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    currentState = ACTION_STATE.CHOOSING_DECK;

    CardManager.HighlightWildDecks();
  }

  static public void ScoutChoseWildCard()
  {
    currentState = ACTION_STATE.NONE;

    CardManager.canDrawWildCards = false;
    CancelAction(true);
  }

  static bool developFromMerchantReward = false;
  static void DevelopAction()
  {
    if (developFromMerchantReward)
      DevelopAfterCard();
    else
      ChooseCardForAction();
  }


  static void DevelopAfterCard()
  {
    //Debug.Log("DevelopAfterCard actionState " + currentAction);
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToPersonalBoard();

    currentState = ACTION_STATE.CHOOSING_TILE;

    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP;

    ObjectManager.HighlightTiles(viableTiles);
    ObjectManager.canPickUpTile = true;

  }

  static TileScript developFirstChosenItem = null;
  static TileScript developSecondChosenItem = null;



  static public void DevelopChoseTile(TileScript tile)
  {
    ObjectManager.DevelopTile(tile);
    ObjectManager.DestroyAllBorders();

    if (developFromMerchantReward)
    {
      CancelAction(true);
      return;
    }

    currentState = ACTION_STATE.CHOOSING_IRON;

    if (ObjectManager.GetAllIronWorksWithFreeIron().Count <= 0)
    {
      if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetIronStoragePrice()))
      {
        ObjectManager.MakeIronStorageClickable();
        ObjectManager.HighlightIronStorage();
      }
    }
    else ObjectManager.HighlightFreeIronSpaces();

    if (developFirstChosenItem is null && !developFromMerchantReward)
      developFirstChosenItem = tile;
    else
      developSecondChosenItem = tile;

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    ObjectManager.canPickUpTile = false;
    ObjectManager.developReqIron = true;

  }

  static IronWorksTileScript developIronSource1 = null;
  static IronWorksTileScript developIronSource2 = null;
  static public void DevelopChoseIron(IronWorksTileScript ironSource)
  {
    //Debug.Log($"DevelopChoseIron: {ironSource}");
    if (developSecondChosenItem is null) //If this is the first developed item
    {
      ObjectManager.canPickUpTile = true;
      ObjectManager.developReqIron = false;

      developIronSource1 = ironSource;
      currentState = ACTION_STATE.CHOOSING_TILE;

      ObjectManager.DestroyAllTileBorders();
      List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

      if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP;

      ObjectManager.HighlightTiles(viableTiles);

      CameraScript camera = Camera.main.GetComponent<CameraScript>();
      camera.MoveToPersonalBoard();

      return;
    }
    currentState = ACTION_STATE.NONE;

    ObjectManager.canPickUpTile = false;
    ObjectManager.developReqIron = false;

    developIronSource2 = ironSource;

    developSecondChosenItem = null;
    developFirstChosenItem = null;
    CancelAction(true);
  }

  static int developIronTakenFromStorage = 0;

  static public void DevelopChoseIronStorage()
  {
    developIronTakenFromStorage++;
    var ironFromStorage = ObjectManager.GetIronFromStorage(out int ironPrice);
    ironFromStorage.SetActive(false);
    GameManager.ActivePlayerSpendMoney(ironPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_IRON;
      Debug.Log("Not enough money to buy iron from storage");
      CancelAction();
    }

    if (developSecondChosenItem is null && !developFromMerchantReward) //If this is the first developed item
    {
      ObjectManager.canPickUpTile = true;
      ObjectManager.developReqIron = false;
      ObjectManager.MakeIronStorageUnClickable();

      currentState = ACTION_STATE.CHOOSING_TILE;

      ObjectManager.DestroyAllTileBorders();
      List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

      if (viableTiles.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP;

      ObjectManager.HighlightTiles(viableTiles);

      CameraScript camera = Camera.main.GetComponent<CameraScript>();
      camera.MoveToPersonalBoard();

      return;
    }
    currentState = ACTION_STATE.NONE;

    ObjectManager.canPickUpTile = false;
    ObjectManager.developReqIron = false;


    developSecondChosenItem = null;
    developFirstChosenItem = null;
    CancelAction(true);
  }

  static void NetworkAction()
  {

    if ((GameManager.currentEra == ERA.TRAIN && GameManager.GetActivePlayer().money < Constants.train1Cost)
      || (GameManager.currentEra == ERA.BOAT && GameManager.GetActivePlayer().money < Constants.boatCost)) // Catch not enough money right away
    {
      Debug.Log("Not enough money to perform network action");
      CancelAction();
      return;
    }

    ChooseCardForAction();

  }
  static void NetworkAfterCard()
  {

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    currentState = ACTION_STATE.CHOOSING_NETWORK_SPACE;

    ObjectManager.MakeCorrectNetworkSpacesClickable();

    List<NetworkSpace> viableSpaces = ObjectManager.GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);

    if (viableSpaces.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE;

    ObjectManager.HighlightNetworkSpaces(viableSpaces);

  }

  static void NetworkRequireCoal(NetworkSpace space)
  {
    currentState = ACTION_STATE.CHOOSING_COAL;

    ObjectManager.MakeAllNetworkSpacesUnclickable();
    ObjectManager.DestroyAllBorders();
    ObjectManager.networkReqCoal = true;
    if (ObjectManager.GetNearestCoalMinesWithFreeCoal(space).Count <= 0)
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(space).Count > 0) //Check whether networkSpace is connected to any merchant
      {
        if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetCoalStoragePrice()))
        {
          ObjectManager.MakeCoalStorageClickable();
          ObjectManager.HighlightCoalStorage();
        }
        else
          curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL;
      }
      else // Else there is not any coal to get
        curMisRes = ACTION_MISSING_RESOURCE.COAL;
    }
    else
    {
      ObjectManager.UpdateNearestFreeCoalTiles(space);
      ObjectManager.HighlightNearestFreeCoalSpaces();
    }
  }

  static public void NetworkAdded(NetworkSpace space)
  {
    if (placedNetworkSpace is null) Network1Added(space);
    else Network2Added(space);
  }

  static NetworkSpace placedNetworkSpace = null;
  static public void Network1Added(NetworkSpace space)
  {
    currentState = ACTION_STATE.NONE;

    int vehicleCost;
    if (GameManager.currentEra == ERA.BOAT) vehicleCost = Constants.boatCost;
    else vehicleCost = Constants.train1Cost;

    GameManager.ActivePlayerSpendMoney(vehicleCost, out bool enoughMoney);

    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_NETWORK;
      Debug.Log("Not enough money for network!");
      CancelAction();
      return;
    }
    if (GameManager.currentEra == ERA.BOAT) //Catch not enough money for second train right away
    {
      EndNetworkAction();
      return;
    }


    placedNetworkSpace = space;

    NetworkRequireCoal(space);
  }

  static List<CoalMineTileScript> networkCoalSources = new();

  static public void NetworkChoseCoal(CoalMineTileScript coalSource)
  {
    currentState = ACTION_STATE.NONE;

    networkCoalSources.Add(coalSource);

    ObjectManager.networkReqCoal = false;

    if (placedNetworkSpace2 is null)
    {
      //Debug.Log("Second placed network is null");

      currentState = ACTION_STATE.CHOOSING_NETWORK_SPACE;

      ObjectManager.DestroyAllBorders();
      ObjectManager.MakeCorrectNetworkSpacesClickable();

      List<NetworkSpace> viableSpaces = ObjectManager.GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);

      if (viableSpaces.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE;

      ObjectManager.HighlightNetworkSpaces(viableSpaces);
    }

    else
      NetworkRequireBarrel(placedNetworkSpace2);

  }
  static int networkCoalTakenFromStorage = 0;
  static public void NetworkChoseCoalStorage()
  {
    currentState = ACTION_STATE.NONE;

    networkCoalTakenFromStorage++;

    var coalFromStorage = ObjectManager.GetCoalFromStorage(out int coalPrice);
    coalFromStorage.SetActive(false);
    GameManager.ActivePlayerSpendMoney(coalPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL;
      Debug.Log("Not enough money to buy coal from storage");
      CancelAction();
    }

    ObjectManager.networkReqCoal = false;

    if (placedNetworkSpace2 is null)
    {
      //Debug.Log("Second placed network is null");

      currentState = ACTION_STATE.CHOOSING_NETWORK_SPACE;

      ObjectManager.DestroyAllBorders();
      ObjectManager.MakeCorrectNetworkSpacesClickable();
      List<NetworkSpace> viableSpaces = ObjectManager.GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);

      if (viableSpaces.Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE;

      ObjectManager.HighlightNetworkSpaces(viableSpaces);
    }

    else
      NetworkRequireBarrel(placedNetworkSpace2);
  }

  static NetworkSpace placedNetworkSpace2 = null;
  static public void Network2Added(NetworkSpace space)
  {
    currentState = ACTION_STATE.NONE;
    placedNetworkSpace2 = space;

    GameManager.ActivePlayerSpendMoney(Constants.train2Cost, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_NETWORK;
      Debug.Log("Not enough money for network!");
      CancelAction();
      return;
    }

    NetworkRequireCoal(space);
  }

  static void NetworkRequireBarrel(NetworkSpace space)
  {
    currentState = ACTION_STATE.CHOOSING_BARREL;

    ObjectManager.networkReqBarrel = true;
    ObjectManager.DestroyAllBorders();
    ObjectManager.FillCurrentSpacesWithBarrelsForNetwork(space);

    if (ObjectManager.GetAllSpacesWithAvailableBarrels(space).Count <= 0) curMisRes = ACTION_MISSING_RESOURCE.BARREL;

    ObjectManager.HighlightCurrentSpacesWithBarrelsForNetwork();
    ObjectManager.MakeAllNetworkSpacesUnclickable();
  }

  static public void NetworkChoseBarrel(TileScript tile)
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.DestroyAllBorders();
    ObjectManager.ClearCurrentSpacesWithBarrelsForNetwork();
    EndNetworkAction();
  }

  static void EndNetworkAction()
  {
    CancelAction(true);
  }


  static public bool IsActionFinishable() =>
    (developFirstChosenItem is not null && (developIronSource1 is not null || developIronTakenFromStorage == 1)
        && developSecondChosenItem is null && currentAction == ACTION.DEVELOP) //Develop finishable requirements
    || (recentlySoldTiles.Count > 0 && currentAction == ACTION.SELL) //Sell finishable requirements
    || (placedNetworkSpace is not null && placedNetworkSpace2 is null && //Network finishabel requirements
        (networkCoalSources.Count == 1 ^ networkCoalTakenFromStorage == 1) && currentAction == ACTION.NETWORK);
  static public bool PlayerDoneAllActions() => actionsPlayed >= Constants.maxActionsPerRound || (GameManager.firstEverRound && actionsPlayed >= 1);

  static public void CancelAction(bool endedSuccesfully = false)
  {

    if (currentAction == ACTION.BUILD && !endedSuccesfully)
    {
      foreach (CoalMineTileScript coalSource in buildCoalSources)
        coalSource.AddCoal();
      foreach (IronWorksTileScript ironSource in buildIronSources)
        ironSource.AddIron();
      for (int i = 0; i < buildCoalTakenFromStorage; i++)
        ObjectManager.AddCoalToStorage();
      for (int i = 0; i < buildIronTakenFromStorage; i++)
        ObjectManager.AddIronToStorage();
    }

    else if (currentAction == ACTION.DEVELOP && !endedSuccesfully)
    {
      if (developFirstChosenItem is not null)
        ObjectManager.UndevelopTile(developFirstChosenItem);
      if (developSecondChosenItem is not null)
        ObjectManager.UndevelopTile(developSecondChosenItem);
      if (developIronSource1 is not null && developIronSource1 != null) //Double check since is not null kept failing for some reason
      {
        //Debug.Log($"Adding Iron to first develop iron source: {developIronSource1}");
        developIronSource1.AddIron();

      }
      if (developIronSource2 is not null && developIronSource1 != null)
      {
        //Debug.Log($"Adding Iron to second develop iron source: {developIronSource2}");
        developIronSource2.AddIron();
      }

      for (int i = 0; i < developIronTakenFromStorage; i++)
        ObjectManager.AddIronToStorage();

    }

    else if (currentAction == ACTION.SELL && !endedSuccesfully)
    {
      foreach (TileScript tile in recentlySoldTiles)
        tile.Downgrade();
      foreach (BarrelSpace barrelSpace in merchBarrelsSpaceUsedToSell)
        barrelSpace.AddBarrel();
      foreach (BreweryTileScript barrelSpace in tileBarrelsUsedToSell)
        barrelSpace.AddBarrel();
    }

    else if (currentAction == ACTION.NETWORK && !endedSuccesfully)
    {
      if (placedNetworkSpace is not null)
        ObjectManager.RemoveNetwork(placedNetworkSpace);
      if (placedNetworkSpace2 is not null)
        ObjectManager.RemoveNetwork(placedNetworkSpace2);
      foreach (CoalMineTileScript coalSource in networkCoalSources)
        coalSource.AddCoal();
      for (int i = 0; i < networkCoalTakenFromStorage; i++)
        ObjectManager.AddCoalToStorage();
    }

    if (endedSuccesfully)
      if (!developFromMerchantReward)
        actionsPlayed++;

    buildIronTakenFromStorage = 0;
    buildCoalTakenFromStorage = 0;
    buildCoalSources.Clear();
    buildIronSources.Clear();
    developFirstChosenItem = null;
    developSecondChosenItem = null;
    developFromMerchantReward = false;
    developIronSource1 = null;
    developIronSource2 = null;
    tileToBeSold = null;
    recentlySoldTiles.Clear();
    placedNetworkSpace = null;
    placedNetworkSpace2 = null;
    networkCoalTakenFromStorage = 0;
    networkCoalSources.Clear();

    if (!endedSuccesfully && CardManager.chosenCards.Count > 0)
      foreach (CardScript card in CardManager.chosenCards)
        CardManager.ReturnCardFromDiscard(card, GameManager.activePlayerIndex);
    cardsChosen = 0;
    CardManager.canChooseCard = false;
    CardManager.ClearChosenCards();
    CardManager.DestroyAllBorders();

    ObjectManager.networkReqBarrel = false;
    ObjectManager.networkReqCoal = false;
    ObjectManager.canChooseSpace = false;
    ObjectManager.canPickUpTile = false;

    ObjectManager.developReqIron = false;
    ObjectManager.buildCoalReq = 0;
    ObjectManager.buildCoalReq = 0;
    ObjectManager.DestroyAllBorders();
    ObjectManager.MakeAllIndustrySpacesUnclickable();
    ObjectManager.MakeAllNetworkSpacesUnclickable();
    ObjectManager.MakeCoalStorageUnClickable();
    ObjectManager.MakeIronStorageUnClickable();
    ObjectManager.DropItem();
    ObjectManager.ForgetSpace();
    ObjectManager.RestoreColliders();

    if (currentAction == ACTION.SELL)
    {
      foreach (BarrelSpace barrel in merchBarrelsSpaceUsedToSell)
        GameManager.GainMerchantBarrelReward(barrel.myReward);
      merchBarrelsSpaceUsedToSell.Clear();
      tileBarrelsUsedToSell.Clear();
    }

    if (!endedSuccesfully) GameManager.ResetActivePlayerToStateBeforeAction();

    GameManager.UpdateActivePlayerPropertiesBeforeAction();

    currentAction = ACTION.NONE;
    //Debug.Log("Setting action state to none");
    currentState = ACTION_STATE.NONE;

    DoRemainingDevelopActionsAfterSell();

    //AI support
    if (endedSuccesfully && developActionsToDo <= 0) AIManager.ActionWasDone();

  }

  public static void DoRemainingDevelopActionsAfterSell()
  {
    if (developActionsToDo <= 0) return;
    developActionsToDo--;
    developFromMerchantReward = true;
    currentAction = ACTION.DEVELOP;
    DevelopAction();
  }


}
public enum ACTION { BUILD, SELL, LOAN, SCOUT, DEVELOP, NETWORK, NONE };
public enum ACTION_STATE { CHOOSING_CARD, CHOOSING_DECK, CHOOSING_TILE, CHOOSING_SPACE, CHOOSING_IRON, CHOOSING_COAL, CHOOSING_BARREL, CHOOSING_NETWORK_SPACE, NONE };
public enum ACTION_MISSING_RESOURCE { CARD, WILD_CARD_ALREADY_IN_HAND,
        TILE_BUILD, TILE_SELL, TILE_DEVELOP, 
        SPACE_BUILD, NETWORK_SPACE,
        MONEY_COAL, MONEY_IRON, INCOME_LOAN, MONEY_NETWORK,
        IRON, COAL, BARREL, NONE};