using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
  public static ActionManager instance;

  /// <summary>
  /// Action that is currently being done
  /// </summary>
  public static ACTION currentAction = ACTION.NONE;
  /// <summary>
  /// State of action -> for example choosing card or choosing tile
  /// </summary>
  public static ACTION_STATE currentState = ACTION_STATE.NONE;
  /// <summary>
  /// Missing resource - in case current action is unfinishable this tells the reason why
  /// </summary>
  public static ACTION_MISSING_RESOURCE curMisRes = ACTION_MISSING_RESOURCE.NONE;



  //TODO: Actions and their logical system


  static int vicPtsBeforeAction = 0;
  static int incomeBeforeAction = 0;
  static int moneyBeforeAction = 0;
  static int moneySpentThisTurnBeforeAction = 0;

  /// <summary>
  /// Number of successfully finished actions this turn
  /// </summary>
  static public int actionsPlayed = 0;

  /// <summary>
  /// Number of develop actions to do after current action -> from merchant reward
  /// </summary>
  static public int developActionsToDo = 0;


  static int cardsChosen = 0;

  void Start()
  {

  }

  private void Awake()
  {
    CreateSingleton();
  }

  /// <summary>
  /// Make this object a singleton
  /// </summary>
  void CreateSingleton()
  {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);


    DontDestroyOnLoad(gameObject);
  }


  /// <summary>
  /// Set player attributes to values before current action -> in case of unsuccessful finish
  /// </summary>
  static public void ResetActivePlayerToStateBeforeAction()
  {
    GameManager.GetActivePlayer().victoryPoints = vicPtsBeforeAction;
    GameManager.GetActivePlayer().income = incomeBeforeAction;
    GameManager.GetActivePlayer().money = moneyBeforeAction;
    GameManager.GetActivePlayer().moneySpentThisTurn = moneySpentThisTurnBeforeAction;
  }


  /// <summary>
  /// Set new values of player attributes before action (save changes) -> in case of successful finish
  /// </summary>
  static public void UpdateActivePlayerStateBeforeAction()
  {
    vicPtsBeforeAction = GameManager.GetActivePlayer().victoryPoints;
    incomeBeforeAction = GameManager.GetActivePlayer().income;
    moneyBeforeAction = GameManager.GetActivePlayer().money;
    moneySpentThisTurnBeforeAction = GameManager.GetActivePlayer().moneySpentThisTurn;
  }

  /// <summary>
  /// Make active player start given action
  /// </summary>
  /// <param name="action">Action to be done</param>
  static public void DoAction(ACTION action)
  {
    //Don't do any action if player already played all actions
    if (PlayerDoneAllActions()) return;

    //If any other action is being done -> can't start another
    if (currentAction != ACTION.NONE) return;
    //CancelAction();
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

    if (possibleCards.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.CARD; HelpFunctions.HUDProblemUpdate();}

      CardManager.HighlightCards(possibleCards);
    //Debug.Log("Setting action state to CHOOSING_CARD");
    currentState = ACTION_STATE.CHOOSING_CARD;

    //Move camera to hand -> tells player to choose card
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToCardPreviewHand();

  }
  
  /// <summary>
  /// Function called after card was chosen
  /// </summary>
  /// <param name="card">Chosen card</param>
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
        //Card chosen for discard -> wanted to end turn

        //Debug.Log("Card discarded for no action");
        actionsPlayed++;
        currentState = ACTION_STATE.NONE;
        if (GameManager.GameRunning())
        {
          ClearCardManagerMemoryAfterCancel(true); //This card choosing was not a part of action
          //Debug.Log("Calling EndTurn from empty action -> after discard");
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

  /// <summary>
  /// Build action after a card was selected
  /// </summary>
  /// <param name="card">Selected card</param>
  static void BuildWithCard(CardScript card)
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();

    //If card is a location - first choose space
    if (card.myType == CARD_TYPE.LOCATION || card.myType == CARD_TYPE.WILD_LOCATION)
    {
      camera.MoveToMainBoard();

      currentState = ACTION_STATE.CHOOSING_SPACE;

      List<IndustrySpace> viableSpaces = ObjectManager.GetAllViableBuildSpaces(card);

      if (viableSpaces.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.SPACE_BUILD; HelpFunctions.HUDProblemUpdate(); }


        ObjectManager.HighlightIndustrySpaces(viableSpaces);
      ObjectManager.MakeCorrectIndustrySpacesClickable(card);
      ObjectManager.canChooseSpace = true;
    }

    //If card is industry - first choose tile
    else
    {
      camera.MoveToPersonalBoard();

      currentState = ACTION_STATE.CHOOSING_TILE;

      List<TileScript> viableTiles = ObjectManager.GetAllViableTiles(card);

      if (viableTiles.Count <= 0) {curMisRes = ACTION_MISSING_RESOURCE.TILE_BUILD; HelpFunctions.HUDProblemUpdate();}


      ObjectManager.HighlightTiles(viableTiles);
      ObjectManager.canPickUpTile = true;
    }

  }

  /// <summary>
  /// Build action after a building tile was chosen
  /// </summary>
  static public void BuildPickedUpItem()
  {

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    //If space already chosen - we have everything
    if (ObjectManager.chosenBuildSpace is not null)
    {
      currentState = ACTION_STATE.NONE;

      BuildSpaceAndTileChosen();
      return;
    }

    //else choose space
    currentState = ACTION_STATE.CHOOSING_SPACE;
    ObjectManager.DestroyAllSpaceBorders();
    ObjectManager.canChooseSpace = true;
    ObjectManager.MakeCorrectIndustrySpacesClickable();
    List<IndustrySpace> viableSpaces = ObjectManager.GetMyNetworkFreeSpacesForItemInHand(GameManager.activePlayerIndex);

    if (viableSpaces.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.SPACE_BUILD; HelpFunctions.HUDProblemUpdate(); }
      ObjectManager.HighlightIndustrySpaces(viableSpaces);
  }
  /// <summary>
  /// Build action after industrySpace was chosen
  /// </summary>
  static public void BuildChoseIndustrySpace()
  {
    //If tile already chosen - we have everything

    if (ObjectManager.itemInHand is not null)
    {
      currentState = ACTION_STATE.NONE;

      BuildSpaceAndTileChosen();
      return;
    }

    //else choose tile

    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToPersonalBoard();

    currentState = ACTION_STATE.CHOOSING_TILE;
    ObjectManager.DestroyAllTileBorders();
    ObjectManager.canPickUpTile = true;
    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_BUILD; HelpFunctions.HUDProblemUpdate(); }
    ObjectManager.HighlightTiles(viableTiles);
  }
  /// <summary>
  /// Building requires coal
  /// </summary>
  static void BuildRequireCoal()
  {
    currentState = ACTION_STATE.CHOOSING_COAL;

    if (ObjectManager.GetNearestCoalMinesWithFreeCoal(ObjectManager.chosenBuildSpace.myLocation).Count <= 0) //Check whether there is connected coalMine with coal
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(ObjectManager.chosenBuildSpace.myLocation).Count > 0) //Check whether buildSpace is connected to any merchant
      {
        if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetCoalStoragePrice())) //Check whether player has money for coal
        {
          ObjectManager.MakeCoalStorageClickable();
          ObjectManager.HighlightCoalStorage();
        }
        else { curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL; HelpFunctions.HUDProblemUpdate(); }
        }
      else
      // Else there is not any coal to get
      { curMisRes = ACTION_MISSING_RESOURCE.COAL; HelpFunctions.HUDProblemUpdate();}
    }
    else //There is connected coalMine
    {
      ObjectManager.UpdateNearestFreeCoalTiles(ObjectManager.chosenBuildSpace.myLocation);
      ObjectManager.HighlightNearestFreeCoalSpaces();
    }
  }
  /// <summary>
  /// Building requires iron
  /// </summary>

  static void BuildRequireIron()
  {
    currentState = ACTION_STATE.CHOOSING_IRON;


    if (ObjectManager.GetAllIronWorksWithFreeIron().Count <= 0) //Check whether there is ironWorks with iron
    {
      if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetIronStoragePrice())) //If not - check whether player has money for iron
      {
        ObjectManager.MakeIronStorageClickable();
        ObjectManager.HighlightIronStorage();
      }
      else
      { curMisRes = ACTION_MISSING_RESOURCE.MONEY_IRON; HelpFunctions.HUDProblemUpdate(); }

    }
    else //There is ironWorks with iron
    {
      ObjectManager.UpdateFreeIronTiles();
      ObjectManager.HighlightFreeIronSpaces();
    }
  }

  /// <summary>
  /// Both building and space were chosen for build action
  /// </summary>
  static public void BuildSpaceAndTileChosen()
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.canChooseSpace = false;
    ObjectManager.canPickUpTile = false;

    ObjectManager.DestroyAllBorders();

    //Pay required resources for building

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

    //If no resources are needed - build successful
    else EndBuildAction();

  }

  /// <summary>
  /// coalMines used for building
  /// </summary>
  static List<CoalMineTileScript> buildCoalSources = new();
  /// <summary>
  /// coalMine chosen as coal source for Build action
  /// </summary>
  /// <param name="coalSource">Chosen coalMine</param>
  static public void BuildChoseCoal(CoalMineTileScript coalSource)
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.buildCoalReq--;

    buildCoalSources.Add(coalSource);

    ObjectManager.DestroyAllBorders();

    //Require remaining resources

    if (ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }

    else if (ObjectManager.buildIronReq > 0)
    {
      ObjectManager.buildIronReq = ObjectManager.itemInHand.buildIronReq;


      BuildRequireIron();
    }

    //If no more resources are needed - build successful
    else EndBuildAction();

  }

  /// <summary>
  /// Amount of coal purchased from coalStorage
  /// </summary>
  static int buildCoalTakenFromStorage = 0;

  /// <summary>
  /// Chose coalStorage as source for build action
  /// </summary>
  static public void BuildChoseCoalStorage()
  {
    currentState = ACTION_STATE.NONE;

    buildCoalTakenFromStorage++;
    var coalFromStorage = ObjectManager.GetCoalFromStorage(out int coalPrice);

    coalFromStorage.GetComponent<SpriteRenderer>().enabled = false;
    coalFromStorage.SetActive(false);

    ObjectManager.MakeCoalStorageUnClickable();
    ObjectManager.DestroyAllBorders();

    //Debug.Log($"Got coal from storage for {coalPrice}");
    GameManager.ActivePlayerSpendMoney(coalPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL;
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money to buy coal from storage");
      return;
      //CancelAction();
    }

    ObjectManager.buildCoalReq--;

    ObjectManager.DestroyAllBorders();

    //Require remaining resources


    if (ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }

    else if (ObjectManager.buildIronReq > 0)
    {
      ObjectManager.buildIronReq = ObjectManager.itemInHand.buildIronReq;

      BuildRequireIron();
    }
    //If no more resources are needed - build successful

    else EndBuildAction();
  }

  /// <summary>
  /// ironWorks used as iron sources for build action
  /// </summary>
  static List<IronWorksTileScript> buildIronSources = new();

  /// <summary>
  /// Chose ironWorks as iron source for build action
  /// </summary>
  /// <param name="ironSource">Chosen ironWorks</param>
  static public void BuildChoseIron(IronWorksTileScript ironSource)
  {
    currentState = ACTION_STATE.NONE;

    ObjectManager.buildIronReq--;

    buildIronSources.Add(ironSource);

    ObjectManager.DestroyAllBorders();
    //Require remaining resources


    if (ObjectManager.buildIronReq > 0)
    {
      BuildRequireIron();
    }

    else if(ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }
    //If no more resources are needed - build successful

    else
    {
      EndBuildAction();
    }
  }
  /// <summary>
  /// Amount of iron purchased from ironStorage
  /// </summary>
  static int buildIronTakenFromStorage = 0;

  /// <summary>
  /// Chose coalStorage as source for build action
  /// </summary>
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
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money to buy iron from storage");
      return;
      //CancelAction();
    }


    ObjectManager.MakeIronStorageUnClickable();
    ObjectManager.DestroyAllBorders();


    ObjectManager.buildIronReq--;

    ObjectManager.DestroyAllBorders();

    //Require remaining resources


    if (ObjectManager.buildIronReq > 0)
    {
      BuildRequireIron();
    }

    else if (ObjectManager.buildCoalReq > 0)
    {
      BuildRequireCoal();
    }
    //If no more resources are needed - build successful

    else
    {
      EndBuildAction();
    }
  }

  /// <summary>
  /// Call if Build action was done successfuly
  /// </summary>
  static void EndBuildAction()
  {
    //Debug.Log($"Ending build action succesfully - building {ObjectManager.itemInHand} on {ObjectManager.chosenBuildSpace}");
    //currentState = ACTION_STATE.NONE;

    ObjectManager.BuildHeldIndustry();

    CancelAction(true);
  }


  /// <summary>
  /// Tiles sold in the current Sell action
  /// </summary>
  static List<TileScript> recentlySoldTiles = new();
  /// <summary>
  /// Tile chosen in the current Sell action, but not yet sold
  /// </summary>
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

    if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL; HelpFunctions.HUDProblemUpdate(); }

      ObjectManager.HighlightTiles(viableTiles);
  }

  /// <summary>
  /// Chosen tile for sell action
  /// </summary>
  /// <param name="tile">Chosen buildin tile</param>
  static public void SellChoseTile(TileScript tile)
  {
    ObjectManager.canSellTile = false;
    ObjectManager.sellReqBarrel = true;
    tileToBeSold = tile;
    ObjectManager.DestroyAllBorders();

    currentState = ACTION_STATE.CHOOSING_BARREL;

    //Update inner memory of barrels available for sell action
    ObjectManager.FillBarrelsForSell(tile.builtOnSpace.myLocation, tile);

    //If there is no barrel available, this is unfinishable
    if (ObjectManager.GetAllAvailableMerchantBarrels(tile.builtOnSpace.myLocation, tile).Count <= 0 &&
      ObjectManager.GetAllSpacesWithAvailableBarrels(tile.builtOnSpace.myLocation).Count <= 0)
    { curMisRes = ACTION_MISSING_RESOURCE.BARREL; HelpFunctions.HUDProblemUpdate(); }

      ObjectManager.HighlightBarrelsForSell();

  }

  /// <summary>
  /// Merchant barrel spaces where barrels were used for the current Sell action
  /// </summary>
  static List<BarrelSpace> merchBarrelsSpaceUsedToSell = new();

  /// <summary>
  /// Chose merchant barrel for Sell action
  /// </summary>
  /// <param name="barrelMerchSpace">Chosen merchant barrel space</param>
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

    //Choose more tile to sell

    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    //If there are no more tiles to be sold -> tell player -> sell is finishable
    if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL; HelpFunctions.HUDProblemUpdate(); }

      currentState = ACTION_STATE.CHOOSING_TILE;
    ObjectManager.HighlightTiles(viableTiles);
    //EndSellAction(); //Uncomment for a single time sell action
  }

  /// <summary>
  /// Breweries where barrels were used for current Sell action
  /// </summary>
  static List<BreweryTileScript> tileBarrelsUsedToSell = new();

  /// <summary>
  /// Brewery chosen as barrel source for Sell action
  /// </summary>
  /// <param name="barrelTile">Chosen brewery as barrel source</param>
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

    //Choose more tile to sell

    List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

    //If there are no more tiles to be sold -> tell player -> sell is finishable
    if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_SELL; HelpFunctions.HUDProblemUpdate(); }
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
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough income for loan action!");
      //CancelAction();
      return;
    }

    ChooseCardForAction();

  }

  static public void LoanAfterCard()
  {
    currentState = ACTION_STATE.NONE;

    GameManager.ActivePlayerGainMoney(Constants.loanMoney);
    GameManager.PlayerLoseIncomeLevel(GameManager.activePlayerIndex, Constants.loanIncomeCost);

    EndLoanAction();
  }

  static void EndLoanAction()
  {
    CancelAction(true);
  }

  static void ScoutAction()
  {

    if (CardManager.PlayerHasWildCard(GameManager.activePlayerIndex))
    {
      curMisRes = ACTION_MISSING_RESOURCE.WILD_CARD_ALREADY_IN_HAND;
      HelpFunctions.HUDProblemUpdate();
      Debug.Log("Can't scout when wild card is already in hand");
      //CancelAction();
      return;
    }

    ChooseCardForAction();
  }

  /// <summary>
  /// Chosen all 3 card for Scout action
  /// </summary>
  static void ScoutAfterCards()
  {
    CardManager.canDrawWildCards = true;
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    currentState = ACTION_STATE.CHOOSING_DECK;

    CardManager.HighlightWildDecks();
  }

  /// <summary>
  /// Chosen wild card to be drawn from Scout action
  /// </summary>
  static CardScript scoutChosenWildCard = null;

  /// <summary>
  /// Chosen which wild card will be drawn in Scout action
  /// </summary>
  /// <param name="chosenWildCard">Chosen wild card</param>
  static public void ScoutChoseWildCard(CardScript chosenWildCard)
  {
    scoutChosenWildCard = chosenWildCard;
    //Debug.Log($"Player {GameManager.activePlayerIndex} selected wild card in scout");
    EndScoutAction();
  }

  static void EndScoutAction()
  {
    CardManager.canDrawWildCards = false;
    CancelAction(true);
  }

  /// <summary>
  /// Whether current develop action is a reward from merchant barrel usage (No resources are required and only 1 tile can be developed)
  /// </summary>
  static bool developFromMerchantReward = false;
  static void DevelopAction()
  {
    //Develop from merchant reward doesn't require a card 
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

    if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP; HelpFunctions.HUDProblemUpdate(); }

    ObjectManager.HighlightTiles(viableTiles);
    ObjectManager.canPickUpTile = true;

  }

  /// <summary>
  /// First of the two tiles chosen to be developed in Develop action
  /// </summary>
  static TileScript developFirstChosenItem = null;
  /// <summary>
  /// Second of the two tiles chosen to be developed in Develop action
  /// </summary>
  static TileScript developSecondChosenItem = null;



  static public void DevelopChoseTile(TileScript tile)
  {
    ObjectManager.DevelopTile(tile);
    ObjectManager.DestroyAllBorders();

    //develop from merchant develops always only one tile
    if (developFromMerchantReward)
    {
      EndDevelopAction();
      return;
    }

    currentState = ACTION_STATE.CHOOSING_IRON;

    //require iron for develop

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

  /// <summary>
  /// ironWorks chosen as iron source for the first developed item
  /// </summary>
  static IronWorksTileScript developIronSource1 = null;
  /// <summary>
  /// ironWorks chosen as iron source for the second developed item
  /// </summary>
  static IronWorksTileScript developIronSource2 = null;
  /// <summary>
  /// Chosen ironWorks as iron source for Develop action
  /// </summary>
  /// <param name="ironSource">chosen ironWorks</param>
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

      //Choose another tile to be developed - now the action is finishable
      List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

      if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP; HelpFunctions.HUDProblemUpdate(); }

        ObjectManager.HighlightTiles(viableTiles);

      CameraScript camera = Camera.main.GetComponent<CameraScript>();
      camera.MoveToPersonalBoard();

      return;
    }
    currentState = ACTION_STATE.NONE;

    ObjectManager.canPickUpTile = false;
    ObjectManager.developReqIron = false;

    developIronSource2 = ironSource;

    EndDevelopAction();
  }

  /// <summary>
  /// amount of iron taken from storage for Develop action
  /// </summary>
  static int developIronTakenFromStorage = 0;

  /// <summary>
  /// Chosen iron storage as iron source for DevelopAction
  /// </summary>
  static public void DevelopChoseIronStorage()
  {
    developIronTakenFromStorage++;
    var ironFromStorage = ObjectManager.GetIronFromStorage(out int ironPrice);
    ironFromStorage.SetActive(false);
    GameManager.ActivePlayerSpendMoney(ironPrice, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_IRON;
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money to buy iron from storage");
      return;
      //CancelAction();
    }

    if (developSecondChosenItem is null && !developFromMerchantReward) //If this is the first developed item
    {
      ObjectManager.canPickUpTile = true;
      ObjectManager.developReqIron = false;
      ObjectManager.MakeIronStorageUnClickable();

      currentState = ACTION_STATE.CHOOSING_TILE;

      ObjectManager.DestroyAllTileBorders();

      //Choose another tile to be developed - now the action is finishable

      List<TileScript> viableTiles = ObjectManager.GetViableTilesForCurrentAction();

      if (viableTiles.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.TILE_DEVELOP; HelpFunctions.HUDProblemUpdate(); }

        ObjectManager.HighlightTiles(viableTiles);

      CameraScript camera = Camera.main.GetComponent<CameraScript>();
      camera.MoveToPersonalBoard();

      return;
    }
    currentState = ACTION_STATE.NONE;

    ObjectManager.canPickUpTile = false;
    ObjectManager.developReqIron = false;

    EndDevelopAction();

  }

  static void EndDevelopAction()
  {
    CancelAction(true);
  }

  static void NetworkAction()
  {

    if ((GameManager.currentEra == ERA.TRAIN && GameManager.GetActivePlayer().money < Constants.train1Cost)
      || (GameManager.currentEra == ERA.BOAT && GameManager.GetActivePlayer().money < Constants.boatCost)) // Catch not enough money right away
    {
      Debug.Log("Not enough money to perform network action");
      //CancelAction();
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

    if (viableSpaces.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE; HelpFunctions.HUDProblemUpdate(); }

      ObjectManager.HighlightNetworkSpaces(viableSpaces);

  }

  static void NetworkRequireCoal(NetworkSpace space)
  {
    currentState = ACTION_STATE.CHOOSING_COAL;

    ObjectManager.MakeAllNetworkSpacesUnclickable();
    ObjectManager.DestroyAllBorders();
    ObjectManager.networkReqCoal = true;
    if (ObjectManager.GetNearestCoalMinesWithFreeCoal(space).Count <= 0) //Check whether there is a connected coalMine with free coal
    {
      if (ObjectManager.GetAllConnectedMerchantTiles(space).Count > 0) //Check whether networkSpace is connected to any merchant
      {
        if (GameManager.ActivePlayerHasEnoughMoney(ObjectManager.GetCoalStoragePrice())) //Check whether player has enough money for coal
        {
          ObjectManager.MakeCoalStorageClickable();
          ObjectManager.HighlightCoalStorage();
        }
        else
        { curMisRes = ACTION_MISSING_RESOURCE.MONEY_COAL; HelpFunctions.HUDProblemUpdate(); }
      }
      else // Else there is not any coal to get
      { curMisRes = ACTION_MISSING_RESOURCE.COAL; HelpFunctions.HUDProblemUpdate(); }
    }
    else //There is a connected coalMine with free iron
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

  /// <summary>
  /// first of the two possible networkSpaces where a vehicle was placed
  /// </summary>
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
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money for network!");
      //CancelAction();
      return;
    }

    placedNetworkSpace = space;

    if (GameManager.currentEra == ERA.BOAT) 
    {
      EndNetworkAction();
      return;
    }

    //Trains require coal
    NetworkRequireCoal(space);
  }

  /// <summary>
  /// coalMines chosen as coal sources for Network action
  /// </summary>
  static List<CoalMineTileScript> networkCoalSources = new();


  /// <summary>
  /// Chose coalMine as coal source for Network action
  /// </summary>
  /// <param name="coalSource">chosen coalMine</param>
  static public void NetworkChoseCoal(CoalMineTileScript coalSource)
  {
    currentState = ACTION_STATE.NONE;

    networkCoalSources.Add(coalSource);

    ObjectManager.networkReqCoal = false;

    if (placedNetworkSpace2 is null) //If this is the coal for first train
    {
      //Debug.Log("Second placed network is null");


      //Choose another networkSpace - action is now finishable
      currentState = ACTION_STATE.CHOOSING_NETWORK_SPACE;

      ObjectManager.DestroyAllBorders();
      ObjectManager.MakeCorrectNetworkSpacesClickable();

      List<NetworkSpace> viableSpaces = ObjectManager.GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);

      if (viableSpaces.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE; HelpFunctions.HUDProblemUpdate(); }

        ObjectManager.HighlightNetworkSpaces(viableSpaces);
    }

    //Second train also requires a barrel
    else
      NetworkRequireBarrel(placedNetworkSpace2);

  }

  /// <summary>
  /// amount of coal taken from storage for Network action
  /// </summary>
  static int networkCoalTakenFromStorage = 0;

  /// <summary>
  /// Chose coal storage as coal source for Network action
  /// </summary>
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
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money to buy coal from storage");
      return;
      //CancelAction();
    }

    ObjectManager.networkReqCoal = false;

    if (placedNetworkSpace2 is null) //This is a coal for the first train
    {
      //Debug.Log("Second placed network is null");

      currentState = ACTION_STATE.CHOOSING_NETWORK_SPACE;


      //Choose another train - action is now finishable
      ObjectManager.DestroyAllBorders();
      ObjectManager.MakeCorrectNetworkSpacesClickable();
      List<NetworkSpace> viableSpaces = ObjectManager.GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);

      if (viableSpaces.Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.NETWORK_SPACE; HelpFunctions.HUDProblemUpdate(); }

        ObjectManager.HighlightNetworkSpaces(viableSpaces);
    }

    //Second train also requires barrel
    else
      NetworkRequireBarrel(placedNetworkSpace2);
  }

  /// <summary>
  /// second of the two possible networkSpaces where a vehicle was placed
  /// </summary>
  static NetworkSpace placedNetworkSpace2 = null;

  /// <summary>
  /// Second network chosen and vehicle placed there - Only in train era
  /// </summary>
  /// <param name="space">Chosen network space</param>
  static public void Network2Added(NetworkSpace space)
  {
    currentState = ACTION_STATE.NONE;
    placedNetworkSpace2 = space;

    GameManager.ActivePlayerSpendMoney(Constants.train2Cost, out bool enoughMoney);
    if (!enoughMoney)
    {
      curMisRes = ACTION_MISSING_RESOURCE.MONEY_NETWORK;
      HelpFunctions.HUDProblemUpdate();

      Debug.Log("Not enough money for network!");
      //CancelAction();
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

    if (ObjectManager.GetAllSpacesWithAvailableBarrels(space).Count <= 0) { curMisRes = ACTION_MISSING_RESOURCE.BARREL; HelpFunctions.HUDProblemUpdate(); }

      ObjectManager.HighlightCurrentSpacesWithBarrelsForNetwork();
    ObjectManager.MakeAllNetworkSpacesUnclickable();
  }

  /// <summary>
  /// Brewery chosen as barrel source for second train in Network action
  /// </summary>
  static BreweryTileScript networkBarrelSource = null;

  static public void NetworkChoseBarrel(BreweryTileScript brewery)
  {
    //currentState = ACTION_STATE.NONE;
    networkBarrelSource = brewery;

    ObjectManager.DestroyAllBorders();
    ObjectManager.ClearCurrentSpacesWithBarrelsForNetwork();
    EndNetworkAction();
  }

  static void EndNetworkAction()
  {
    CancelAction(true);
  }

  /// <summary>
  /// Gives bool whether current action in the current state is finishable
  /// </summary>
  static public bool IsActionFinishable() =>
    (developFirstChosenItem is not null && (developIronSource1 is not null || developIronTakenFromStorage == 1)//Develop finishable requirements
        && developSecondChosenItem is null && currentAction == ACTION.DEVELOP) 
    || (recentlySoldTiles.Count > 0 && currentAction == ACTION.SELL) //Sell finishable requirements
    || (placedNetworkSpace is not null && placedNetworkSpace2 is null && //Network finishable requirements
        (networkCoalSources.Count == 1 ^ networkCoalTakenFromStorage == 1) && currentAction == ACTION.NETWORK);

  /// <summary>
  /// Gives bool whehter current player has already played all available actions -> only remaining action is to end turn
  /// </summary>
  /// <returns></returns>
  static public bool PlayerDoneAllActions() => actionsPlayed >= Constants.maxActionsPerRound || (GameManager.firstEverRound && actionsPlayed >= 1);

  /// <summary>
  /// Stop the current action
  /// </summary>
  /// <param name="endedSuccessfully">Whether action was finished to the end or it was interrupted in the middle</param>
  static public void CancelAction(bool endedSuccessfully = false)
  {

    //Tell AI that action was succesful - if it is not happy with this action -> give it a chance to cancel in the last moment
    if (endedSuccessfully)
    {
      AIManager.ActionAboutToEndSuccessfuly(out bool cancelAction);
      if (cancelAction) { CancelAction(); return; }
    }

    //Debug.Log($"Cancel action called with {endedSuccesfully} ending");
    HUDMessageShowAfterCancelAction(endedSuccessfully);

    //Move camera to main board after action
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();

    //Action ended -> nothing is missing now
    curMisRes = ACTION_MISSING_RESOURCE.NONE;

    //Revert or place correct action effects after cancel
    switch (currentAction)
    {
      case ACTION.BUILD:
        BuildEffectsAfterCancel(endedSuccessfully);
        break;
      case ACTION.SELL:
        SellEffectsAfterCancel(endedSuccessfully);
        break;
      case ACTION.LOAN:
        break;
      case ACTION.SCOUT:
        ScoutEffectsAfterCancel(endedSuccessfully);
        break;
      case ACTION.DEVELOP:
        DevelopEffectsAfterCancel(endedSuccessfully);
        break;
      case ACTION.NETWORK:
        NetworkEffectsAfterCancel(endedSuccessfully);
        break;
      case ACTION.NONE:
        break;
      default:
        break;
    }

    //Add played actions
    if (endedSuccessfully)
      if (!developFromMerchantReward)
        actionsPlayed++;


    //Clear memory - we don't want any effect from last action to hold in the next one
    ClearLocalMemoryAfterCancel();

    ClearCardManagerMemoryAfterCancel(endedSuccessfully);

    ClearObjectManagerMemoryAfterCancel(endedSuccessfully);
    

    //Set player attributes to correct values
    if (!endedSuccessfully) GameManager.ResetActivePlayerToStateBeforeAction();

    GameManager.UpdateActivePlayerPropertiesBeforeAction();

    //Action ended - state and action is now none
    currentAction = ACTION.NONE;
    currentState = ACTION_STATE.NONE;

    //Begin new develop action if we gained it from merchant reward
    DoRemainingDevelopActionsAfterSell();

    //AI support
    if (endedSuccessfully && developActionsToDo <= 0) AIManager.ActionWasDone();

  }

  /// <summary>
  /// Show correct message after action
  /// </summary>
  static void HUDMessageShowAfterCancelAction(bool endedSuccessfully)
  {
    if (!endedSuccessfully)
      HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.ACTION_CANCELED);
    else
    {
      //Debug.Log("HUD info showing succesfully done action");
      switch (currentAction)
      {
        case ACTION.BUILD:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.BUILD_SUCCESS);
          break;
        case ACTION.SELL:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.SELL_SUCCESS);
          break;
        case ACTION.LOAN:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.LOAN_SUCCESS);
          break;
        case ACTION.SCOUT:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.SCOUT_SUCCESS);
          break;
        case ACTION.DEVELOP:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.DEVELOP_SUCCESS);
          break;
        case ACTION.NETWORK:
          HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.NETWORK_SUCCESS);
          break;
        case ACTION.NONE:
          break;
        default:
          break;
      }
    }
  }
  /// <summary>
  /// Place correct effect after Build action
  /// </summary>
  static void BuildEffectsAfterCancel(bool endedSuccessfully)
  {
    if (!endedSuccessfully)
    {
      foreach (CoalMineTileScript coalSource in buildCoalSources)
      {
        if (coalSource.GetResourceCount() <= 0 && coalSource.isUpgraded)
          coalSource.Downgrade();
        coalSource.AddCoal();
      }
      foreach (IronWorksTileScript ironSource in buildIronSources)
      {
        if (ironSource.GetResourceCount() <= 0 && ironSource.isUpgraded)
          ironSource.Downgrade();
        ironSource.AddIron();
      }
      for (int i = 0; i < buildCoalTakenFromStorage; i++)
        ObjectManager.AddCoalToStorage();
      for (int i = 0; i < buildIronTakenFromStorage; i++)
        ObjectManager.AddIronToStorage();

      if (ObjectManager.itemInHand is not null)
        ObjectManager.UnBuildHeldIndustry();
    }

    else
    {
      //Debug.Log("Build successful");
      if (ObjectManager.overBuiltTile is not null)
      {
        //Debug.Log($"Overbuilt tile - removing the previous {ObjectManager.overBuiltTile}");
        ObjectManager.RemoveTile(ObjectManager.overBuiltTile);
      }
    }
  }
  /// <summary>
  /// Place correct effect after Sell action
  /// </summary>
  static void SellEffectsAfterCancel(bool endedSuccessfully) 
  {
    if (!endedSuccessfully)
    {
      foreach (TileScript tile in recentlySoldTiles)
        tile.Downgrade();
      foreach (BarrelSpace barrelSpace in merchBarrelsSpaceUsedToSell)
        barrelSpace.AddBarrel();
      foreach (BreweryTileScript barrelSpace in tileBarrelsUsedToSell)
      {
        if (barrelSpace.GetResourceCount() <= 0 && barrelSpace.isUpgraded) barrelSpace.Downgrade();
        barrelSpace.AddBarrel();
      }
    }
    else
      foreach (BarrelSpace barrel in merchBarrelsSpaceUsedToSell)
        GameManager.GainMerchantBarrelReward(barrel.myReward);
  }
  /// <summary>
  /// Place correct effect after Scout action
  /// </summary>
  static void ScoutEffectsAfterCancel(bool endedSuccessfully)
  {
    if (!endedSuccessfully && scoutChosenWildCard is not null)
    {
      CardManager.DiscardCard(scoutChosenWildCard, GameManager.activePlayerIndex);
    }
  }
  /// <summary>
  /// Place correct effect after Develop action
  /// </summary>
  static void DevelopEffectsAfterCancel(bool endedSuccessfully)
  {
    if (!endedSuccessfully)
    {
      if (developFirstChosenItem is not null)
        ObjectManager.UndevelopTile(developFirstChosenItem);
      if (developSecondChosenItem is not null)
        ObjectManager.UndevelopTile(developSecondChosenItem);
      if (developIronSource1 is not null && developIronSource1 != null) //Double check since is not null kept failing for some reason
      {
        //Debug.Log($"Adding Iron to first develop iron source: {developIronSource1}");
        if (developIronSource1.GetResourceCount() <= 0 && developIronSource1.isUpgraded)
          developIronSource1.Downgrade();
        developIronSource1.AddIron();
      }
      if (developIronSource2 is not null && developIronSource1 != null)
      {
        //Debug.Log($"Adding Iron to second develop iron source: {developIronSource2}");
        if (developIronSource2.GetResourceCount() <= 0 && developIronSource2.isUpgraded)
          developIronSource2.Downgrade();
        developIronSource2.AddIron();
      }

      for (int i = 0; i < developIronTakenFromStorage; i++)
        ObjectManager.AddIronToStorage();
    }
  }
  /// <summary>
  /// Place correct effect after Network action
  /// </summary>
  static void NetworkEffectsAfterCancel(bool endedSuccessfully)
  {
    if (!endedSuccessfully)
    {
      if (placedNetworkSpace is not null)
        ObjectManager.RemoveNetwork(placedNetworkSpace);
      if (placedNetworkSpace2 is not null)
        ObjectManager.RemoveNetwork(placedNetworkSpace2);
      foreach (CoalMineTileScript coalSource in networkCoalSources)
      {
        if (coalSource.GetResourceCount() <= 0 && coalSource.isUpgraded) coalSource.Downgrade();
        coalSource.AddCoal();

      }
      for (int i = 0; i < networkCoalTakenFromStorage; i++)
        ObjectManager.AddCoalToStorage();

      if (networkBarrelSource is not null)
      {
        if (networkBarrelSource.GetResourceCount() <= 0 && networkBarrelSource.isUpgraded) networkBarrelSource.Downgrade();
        networkBarrelSource.AddBarrel();

      }
    }
  }

  static void ClearLocalMemoryAfterCancel()
  {
    buildIronTakenFromStorage = 0;
    buildCoalTakenFromStorage = 0;
    buildCoalSources.Clear();
    buildIronSources.Clear();
    scoutChosenWildCard = null;
    developFirstChosenItem = null;
    developSecondChosenItem = null;
    developFromMerchantReward = false;
    developIronSource1 = null;
    developIronSource2 = null;
    developIronTakenFromStorage = 0;
    tileToBeSold = null;
    recentlySoldTiles.Clear();
    placedNetworkSpace = null;
    placedNetworkSpace2 = null;
    networkCoalTakenFromStorage = 0;
    networkCoalSources.Clear();
    networkBarrelSource = null;
    cardsChosen = 0;
    merchBarrelsSpaceUsedToSell.Clear();
    tileBarrelsUsedToSell.Clear();
  }

  static void ClearCardManagerMemoryAfterCancel(bool endedSuccessfully)
  {
    if (!endedSuccessfully && CardManager.chosenCards.Count > 0)
      foreach (CardScript card in CardManager.chosenCards)
        CardManager.ReturnCardFromDiscard(card, GameManager.activePlayerIndex);
    CardManager.canChooseCard = false;
    CardManager.ClearChosenCards();
    CardManager.DestroyAllBorders();
  }

  static void ClearObjectManagerMemoryAfterCancel(bool endedSuccessfully)
  {

    ObjectManager.networkReqBarrel = false;
    ObjectManager.networkReqCoal = false;
    ObjectManager.canChooseSpace = false;
    ObjectManager.canPickUpTile = false;

    ObjectManager.developReqIron = false;
    ObjectManager.buildCoalReq = 0;
    ObjectManager.buildIronReq = 0;
    ObjectManager.overBuiltTile = null;
    ObjectManager.DestroyAllBorders();
    ObjectManager.MakeAllIndustrySpacesUnclickable();
    ObjectManager.MakeAllNetworkSpacesUnclickable();
    ObjectManager.MakeCoalStorageUnClickable();
    ObjectManager.MakeIronStorageUnClickable();
    ObjectManager.DropItem();
    ObjectManager.ForgetSpace();
    ObjectManager.RestoreColliders();

  }

  /// <summary>
  /// Do all remaining develop actions gained from last Sell action
  /// </summary>
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