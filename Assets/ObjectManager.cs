using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour, ISaveable
{
  public static ObjectManager instance;


  /// <summary>
  /// Chosen tile for current action
  /// </summary>
  public static TileScript itemInHand;

  /// <summary>
  /// Chosen industry space for current action
  /// </summary>
  public static IndustrySpace chosenBuildSpace;

  static bool alreadyCreatedObjects = false;

  public static bool canPickUpTile = false;
  public static bool canChooseSpace = false;
  public static bool canSellTile = false;
  public static bool networkReqBarrel = false;
  public static bool networkReqCoal = false;
  public static bool sellReqBarrel = false;
  public static bool developReqIron = false;

  public static int buildIronReq = 0;
  public static int buildCoalReq = 0;

  static List<TileScript> redBreweries = new List<TileScript>();
  static List<TileScript> yellowBreweries = new List<TileScript>();
  static List<TileScript> whiteBreweries = new List<TileScript>();
  static List<TileScript> purpleBreweries = new List<TileScript>();
  static List<TileScript> redIronWorks = new List<TileScript>();
  static List<TileScript> yellowIronWorks = new List<TileScript>();
  static List<TileScript> whiteIronWorks = new List<TileScript>();
  static List<TileScript> purpleIronWorks = new List<TileScript>();
  static List<TileScript> redCoalMines = new List<TileScript>();
  static List<TileScript> yellowCoalMines = new List<TileScript>();
  static List<TileScript> whiteCoalMines = new List<TileScript>();
  static List<TileScript> purpleCoalMines = new List<TileScript>();
  static List<TileScript> redCottonMills = new List<TileScript>();
  static List<TileScript> yellowCottonMills = new List<TileScript>();
  static List<TileScript> whiteCottonMills = new List<TileScript>();
  static List<TileScript> purpleCottonMills = new List<TileScript>();
  static List<TileScript> redPotteries = new List<TileScript>();
  static List<TileScript> yellowPotteries = new List<TileScript>();
  static List<TileScript> whitePotteries = new List<TileScript>();
  static List<TileScript> purplePotteries = new List<TileScript>();
  static List<TileScript> redManufacturers = new List<TileScript>();
  static List<TileScript> yellowManufacturers = new List<TileScript>();
  static List<TileScript> whiteManufacturers = new List<TileScript>();
  static List<TileScript> purpleManufacturers = new List<TileScript>();

  /// <summary>
  /// All breweries seperated per player
  /// </summary>
  static List<TileScript>[] allBreweries = { redBreweries, yellowBreweries, whiteBreweries, purpleBreweries };
  /// <summary>
  /// All coalMines seperated per player
  /// </summary>
  static List<TileScript>[] allCoalMines = { redCoalMines, yellowCoalMines, whiteCoalMines, purpleCoalMines };
  /// <summary>
  /// All cottonMills seperated per player
  /// </summary>
  static List<TileScript>[] allCottonMills = { redCottonMills, yellowCottonMills, whiteCottonMills, purpleCottonMills };
  /// <summary>
  /// All ironWorks seperated per player
  /// </summary>
  static List<TileScript>[] allIronWorks = { redIronWorks, yellowIronWorks, whiteIronWorks, purpleIronWorks };
  /// <summary>
  /// All potteries seperated per player
  /// </summary>
  static List<TileScript>[] allPotteries = { redPotteries, yellowPotteries, whitePotteries, purplePotteries };
  /// <summary>
  /// All manufacturers seperated per player
  /// </summary>
  static List<TileScript>[] allManufacturers = { redManufacturers, yellowManufacturers, whiteManufacturers, purpleManufacturers };

  /// <summary>
  /// All tile of red player seperated by type
  /// </summary>
  static List<TileScript>[] allRedTiles = { redBreweries, redCoalMines, redCottonMills, redIronWorks, redManufacturers, redPotteries };
  /// <summary>
  /// All tile of yellow player seperated by type
  /// </summary>
  static List<TileScript>[] allYellowTiles = { yellowBreweries, yellowCoalMines, yellowCottonMills, yellowIronWorks, yellowManufacturers, yellowPotteries };
  /// <summary>
  /// All tile of white player seperated by type
  /// </summary>
  static List<TileScript>[] allWhiteTiles = { whiteBreweries, whiteCoalMines, whiteCottonMills, whiteIronWorks, whiteManufacturers, whitePotteries };
  /// <summary>
  /// All tile of purple player seperated by type
  /// </summary>
  static List<TileScript>[] allPurpleTiles = { purpleBreweries, purpleCoalMines, purpleCottonMills, purpleIronWorks, purpleManufacturers, purplePotteries };

  /// <summary>
  /// All tiles seperated per player seperated per industry
  /// </summary>
  static List<TileScript>[][] allTilesPerPlayer = { allRedTiles, allYellowTiles, allWhiteTiles, allPurpleTiles };

  /// <summary>
  /// All tiles seperated per industry seperated per player
  /// </summary>
  static List<TileScript>[][] allTilesPerIndustry = { allBreweries, allCoalMines, allCottonMills, allIronWorks, allManufacturers, allPotteries };

  public static List<GameObject> allSpaceBorders = new List<GameObject>();
  public static List<GameObject> allNetworkBorders = new List<GameObject>();
  public static List<GameObject> allTileBorders = new List<GameObject>();
  public static List<GameObject>[] allBorders = { allSpaceBorders, allNetworkBorders, allTileBorders };

  /// <summary>
  /// All players' victory points tokens
  /// </summary>
  static List<GameObject> VPTokens = new();
  /// <summary>
  /// All players' income tokens
  /// </summary>
  static List<GameObject> incomeTokens = new();


  static List<LocationScript> allLocations = new List<LocationScript>();
  static List<LocationScript> allIndustryLocations = new List<LocationScript>();
  static Dictionary<LocationScript, List<IndustrySpace>> locationSpacesDict = new Dictionary<LocationScript, List<IndustrySpace>>();

  static List<LocationScript> allMerchants = new List<LocationScript>();
  static List<MerchantTileScript> allMerchantTiles = new();
  //static Dictionary<LocationScript, List<BarrelSpace>> merchantBarrelsDict = new();
  //static Dictionary<LocationScript, List<MerchantTile>> merchantTileDict = new();
  //static Dictionary<LocationScript, MerchantReq> merchantReqDict = new();


  private static List<int> firstUnbuiltBreweryIndeces = new List<int>();
  private static List<int> firstUnbuiltIronWorksIndeces = new List<int>();
  private static List<int> firstUnbuiltCoalMineIndeces = new List<int>();
  private static List<int> firstUnbuiltCottonMillIndeces = new List<int>();
  private static List<int> firstUnbuiltPotteryIndeces = new List<int>();
  private static List<int> firstUnbuiltManufacturerIndeces = new List<int>();

  private static List<NetworkSpace> allNetworkSpacesBoat = new();
  private static List<NetworkSpace> allNetworkSpacesTrain = new();

  static Dictionary<LocationScript, List<NetworkSpace>> locationNetworkSpacesDictBoat = new();
  static Dictionary<LocationScript, List<NetworkSpace>> locationNetworkSpacesDictTrain = new();

  static List<IndustrySpace> currentSpacesWithBarrelsForNetwork = new();
  static List<BarrelSpace> currentMerchantBarrelsForSell = new();
  static List<IndustrySpace> currentSpacesWithBarrelsForSell = new();
  static List<IronWorksTileScript> currentTilesWithFreeIron = new();
  static List<CoalMineTileScript> currentNearestTilesWithFreeCoal = new();

  static List<TileScript> tilesWithDisabledColliders = new();
  public static TileScript overBuiltTile = null;

  private static ResourceStorage coalStorage;
  private static ResourceStorage ironStorage;

  private static GameObject mainBoard;


  static bool firstCall = true;
  // Update is called once per frame
  void Update()
  {
    //Debug.Log("ObjectManager Update");
    if (firstCall)
    {
      InitializeObjects();
      firstCall = false;
    }
    UpdateTokenPositions();
  }
  private void OnDestroy()
  {
    firstCall = true;
    ClearAllLocalMemory();
  }

  private void Start()
  {
    //Debug.Log("ObjectManager Start called!");
    mainBoard = GameObject.Find(Constants.mainBoardName);

    LoadAllStaticObjects();

  }

  static void LoadAllStaticObjects()
  {
    LoadAllLocations();
    LoadAllNetworkSpaces();
    coalStorage = mainBoard.transform.Find(Constants.coalStorageName).gameObject.GetComponent<ResourceStorage>();
    ironStorage = mainBoard.transform.Find(Constants.ironStorageName).gameObject.GetComponent<ResourceStorage>();
  }

  public static void InitializeObjects()
  {
    //Debug.Log("Initializing objects");
    DestroyAllObjects();

    CreateAllObjects();
    AddIdsToMerchantTileSpaces();
  }

  public static void LoadAllLocations()
  {
    GameObject locations = mainBoard.transform.Find(Constants.locationParentName).gameObject;
    int industryId = 0;
    foreach (Transform location in locations.transform)
    {
      LocationScript locationObject = location.gameObject.GetComponent<LocationScript>();
      allLocations.Add(locationObject);
      if (locationObject.myType == LocationType.CITY || locationObject.myType == LocationType.NAMELESS_BREWERY)
      {
        allIndustryLocations.Add(locationObject);
        locationSpacesDict[locationObject] = new List<IndustrySpace>();
        foreach (Transform space in location)
        {
          IndustrySpace industrySpace = space.gameObject.GetComponent<IndustrySpace>();
          locationSpacesDict[locationObject].Add(industrySpace);
          industrySpace.id = industryId++;

        }
      }
      else if (locationObject.myType == LocationType.MERCHANT)
      {
        allMerchants.Add(locationObject);

      }
    }
  }

  private static void AddIdsToMerchantTileSpaces()
  {
    int merchantSpaceID = 0;

    foreach (LocationScript merchantLoc in allMerchants)
      foreach (MerchantTileSpace space in merchantLoc.myMerchants)
        {
          //Debug.Log($"Setting merchant space ID to {merchantSpaceID}");
          space.id = merchantSpaceID++;
        }
  }
  private static void LoadAllNetworkSpaces()
  {
    LoadAllNetworkSpaces(Constants.boatNetworkParentName, allNetworkSpacesBoat, locationNetworkSpacesDictBoat);
    LoadAllNetworkSpaces(Constants.trainNetworkParentName, allNetworkSpacesTrain, locationNetworkSpacesDictTrain);
  }

  private static void LoadAllNetworkSpaces(string parentName, List<NetworkSpace> addToList, Dictionary<LocationScript, List<NetworkSpace>> addToDict)
  {
    int id = 0;
    Transform spaces = mainBoard.transform.Find(parentName);
    foreach (Transform child in spaces)
    {
      if (child.gameObject.TryGetComponent(out NetworkSpace space))
      {
        addToList.Add(space);
        space.id = id++;

        foreach (LocationScript location in space.connectsLocations)
        {
          if (!addToDict.ContainsKey(location))
            addToDict[location] = new();
          addToDict[location].Add(space);
        }
      }
    }
  }

  private static void CreateTokens()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      var vicTokenResource = HelpFunctions.LoadPrefabFromFile(Constants.allVictoryTokes[i]);
      GameObject newVictoryToken = Instantiate(vicTokenResource) as GameObject;
      VPTokens.Add(newVictoryToken);

      var incomeTokenResource = HelpFunctions.LoadPrefabFromFile(Constants.allIncomeTokens[i]);
      GameObject newIncomeToken = Instantiate(incomeTokenResource) as GameObject;
      incomeTokens.Add(newIncomeToken);

      Debug.Log($"Created token {i}");
    }
  }

  static void SetVictoryTokenPosition(int playerIndex, int victoryPoints)
  {
    //Debug.Log($"Victory token {VPTokens[playerIndex]}");
    if (VPTokens.Count < playerIndex + 1) return;
    VPTokens[playerIndex].transform.position = mainBoard.transform.Find(Constants.VPSpacesName).GetChild(victoryPoints%99).position
      + Constants.victoryTokenOffset + Constants.playerTokenOffsets[playerIndex];
  }

  static void SetIncomeTokenPosition(int playerIndex, int income)
  {
    if (incomeTokens.Count < playerIndex + 1) return;

    Vector3 destination = mainBoard.transform.Find(Constants.VPSpacesName).transform.GetChild(income%99).position
      + Constants.incomeTokenOffset + Constants.playerTokenOffsets[playerIndex]; 
    //Debug.Log($"Setting position of {incomeTokens[playerIndex]} to {destination}");
    incomeTokens[playerIndex].transform.position = destination;
  }

  public static void UpdateTokenPositions()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      SetVictoryTokenPosition(i, GameManager.GetPlayer(i).victoryPoints);
      SetIncomeTokenPosition(i, GameManager.GetPlayer(i).income);
    }
  }

  /// <summary>
  /// Choose a tile for build action
  /// </summary>
  public static void BuildPickUpTile(TileScript item)
  {
    DropItem();
    itemInHand = item;
    item.GetComponent<SpriteRenderer>().enabled = false;
    itemInHand.GetComponent<BoxCollider2D>().enabled = false;

    GameManager.ActivePlayerSpendMoney(itemInHand.buildCost, out bool enoughMoney);
    if (!enoughMoney)
    {
      Debug.LogError("Chose tile without money to pay for it");
      return;
    }

    ActionManager.BuildPickedUpItem();
  }

  public static void SellTile(TileScript tile)
  {
    tile.Upgrade();
  }

  static public void DevelopTile(TileScript tile)
  {
    AddFirstUnbuiltIndex(tile, out bool success);
    if (!success)
    {
      Debug.LogError("Can't develop tile which is over max level");
      return;
    }

    tile.Develop();
  }

  static public void UndevelopTile(TileScript tile)
  {
    SubtractFirstUnbuiltIndex(tile, out bool success);
    if (!success)
    {
      Debug.LogError("Can't unDevelop tile which is under level 1");
      return;
    }

    tile.Undevelop();

  }

  /// <summary>
  /// Disable given tile and forget it
  /// </summary>
  static public void RemoveTile(TileScript tile)
  {
    List<TileScript>[] allPlayersLists = null; 
    switch (tile.industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.COALMINE:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.POTTERY:
        allPlayersLists = allBreweries;
        break;
      case INDUSTRY_TYPE.NONE:
        return;
      default:
        break;
    }

    foreach (List<TileScript> tilesPerPlayer in allPlayersLists)
      tilesPerPlayer.Remove(tile);

    if (tile != overBuiltTile)
      tile.builtOnSpace.RemoveBuiltIndustry();
    tile.Remove();

    //Debug.Log("Removed " + tile);
  }

  public static void BarrelSpaceClicked(BarrelSpace barrelSpace)
  {
    if (!sellReqBarrel || !barrelSpace.hasBarrel) return;

    if (currentMerchantBarrelsForSell.Contains(barrelSpace))
    {
      barrelSpace.RemoveBarrel();
      ActionManager.SellChoseBarrel(barrelMerchSpace: barrelSpace);
      return;
    }
  }

  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>
  static bool NetworkBarrelTileChoosing(TileScript tile)
  {
    //Network barrel choosing
    //Debug.Log("Choose space and reqBarrel=" + networkReqBarrel.ToString());
    if (networkReqBarrel)
    {
      //Debug.Log("Barrel space choosing");
      //Debug.Log($"Tile is {tile}");
      if (currentSpacesWithBarrelsForNetwork.Contains(tile.builtOnSpace) && tile is BreweryTileScript brewery)
      {
        brewery.RemoveBarrel();
        ActionManager.NetworkChoseBarrel(brewery);
        return true;
      }
    }
    return false;
  }
  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>

  static bool NetworkCoalTileChoosing(TileScript tile)
  {
    //Network barrel choosing
    //Debug.Log("Choose space and reqBarrel=" + networkReqBarrel.ToString());
    if (tile is not CoalMineTileScript) return false;
    CoalMineTileScript coalMine = (CoalMineTileScript)tile;
    if (networkReqCoal && currentNearestTilesWithFreeCoal.Contains(coalMine))
    {
      coalMine.RemoveCoal();
      ActionManager.NetworkChoseCoal(coalMine);
      return true;
    }
    return false;
  }

  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>
  static bool SellBarrelTileChoosing(TileScript tile) {
    //Sell barrel choosing
    if (sellReqBarrel)
    {
      //Debug.Log("Barrel space choosing");
      if (currentSpacesWithBarrelsForSell.Contains(tile.builtOnSpace) && tile is BreweryTileScript)
      {
        BreweryTileScript brewery = (BreweryTileScript)tile;
        brewery.RemoveBarrel();
        ActionManager.SellChoseBarrel(barrelTile: brewery);
        return true;
      }
    }
    return false;
  }

  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>
  static bool DevelopIronTileChoosing(TileScript tile)
  {
    UpdateFreeIronTiles();
    if (tile is not IronWorksTileScript) return false;
    IronWorksTileScript ironWorks = (IronWorksTileScript)tile;
    if (developReqIron && currentTilesWithFreeIron.Contains(ironWorks))
    {
      ironWorks.RemoveIron();
      ActionManager.DevelopChoseIron(ironWorks);
      return true;
    }
    return false;
  }
  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>

  static bool BuildCoalTileChoosing(TileScript tile)
  {
    if (tile is not CoalMineTileScript) return false;
    CoalMineTileScript coalMine = (CoalMineTileScript)tile;
    if (buildCoalReq > 0 && currentNearestTilesWithFreeCoal.Contains(coalMine))
    {
      coalMine.RemoveCoal();
      ActionManager.BuildChoseCoal(coalMine);
      return true;
    }
    return false;
  }
  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>

  static bool BuildIronTileChoosing(TileScript tile)
  {
    UpdateFreeIronTiles();
    if (tile is not IronWorksTileScript) return false;
    IronWorksTileScript ironWorks = (IronWorksTileScript)tile;
    if (buildIronReq>0 && currentTilesWithFreeIron.Contains(ironWorks))
    {
      ironWorks.RemoveIron();
      ActionManager.BuildChoseIron(ironWorks);
      return true;
    }
    return false;
  }
  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>

  static bool PickTileFromPersonalBoard(TileScript tile)
  {

    // Build/Develop tile choosing
    // If space is chosen, tile of different type can't be picked up
    if ((chosenBuildSpace is not null && !chosenBuildSpace.myTypes.Contains(tile.industryType)) || !canPickUpTile) return false;

    // If picking up tile - trying to build - check whether it is the lowest level unbuilt tile possible
    if (CanTileBeBuilt(tile) && ActionManager.currentAction == ACTION.BUILD && GameManager.ActivePlayerHasEnoughMoney(tile.buildCost) && 
      (chosenBuildSpace is null ||
      chosenBuildSpace.myTile is null || chosenBuildSpace.myTile.industryType == tile.industryType)) //Can overbuild but not different type
    {
      BuildPickUpTile(tile);
      return true;
    }
    else if (ActionManager.currentAction == ACTION.DEVELOP && CanTileBeDeveloped(tile))
    {
      ActionManager.DevelopChoseTile(tile);
      return true;
    }
    
    return false;
  }
  /// <param name="tile"></param>
  /// <returns>Bool returned is whether resource was succesfully chosen</returns>

  static bool SellTileChoosing(TileScript tile)
  {
    // Sell tile choosing
    if (ActionManager.currentAction == ACTION.SELL && canSellTile)
    {
      List<MerchantTileSpace> connectedMerchants = GetAllConnectedMerchantTiles(tile.builtOnSpace.myLocation, tile.industryType);
      bool barrelAvailable = false;
      bool merchantOfMyTypeAvailable = connectedMerchants.Count > 0;

      foreach (MerchantTileSpace merchant in connectedMerchants)
        if (merchant.myBarrelSpace.hasBarrel) barrelAvailable = true;

      if (!barrelAvailable)
        if (GetAllSpacesWithAvailableBarrels(tile.builtOnSpace.myLocation).Count > 0) barrelAvailable = true;


      if (canSellTile && //Check whether activePlayer can sell tile
        tile.alreadyBuilt && //tile is built on main board
        tile.ownerPlayerIndex == GameManager.activePlayerIndex && //tile is owned by current player
        !tile.isUpgraded && //tile is not already sold
        merchantOfMyTypeAvailable &&//Correct merchant is connected to the tile
        barrelAvailable)//Barrel is available to pay for trade
      { ActionManager.SellChoseTile(tile); return true; }
    }
    return false;
  }

  /// <summary>
  /// Tile was chosen -> this function sorts the choosing based on current action and state
  /// </summary>
  public static void ChooseTile(TileScript tile)
  {
    if (NetworkCoalTileChoosing(tile)) return;
    if (NetworkBarrelTileChoosing(tile)) return;
    //Debug.Log("Didn't choose network barrel");
    if (SellBarrelTileChoosing(tile)) return;
    //Debug.Log("Didn't choose sell barrel");
    if (BuildCoalTileChoosing(tile)) return;
    if (BuildIronTileChoosing(tile)) return;
    if (DevelopIronTileChoosing(tile)) return;
    //Debug.Log("Didn't choose develop iron");
    if (PickTileFromPersonalBoard(tile)) return;
    //Debug.Log("Didn't choose tile to pick up/develop");
    if (SellTileChoosing(tile)) return;
    //Debug.Log("Didn't choose tile to sell");


  }

  /// <summary>
  /// Forgets chosen item and returns it to its original position
  /// </summary>
  public static void DropItem()
  {
    if (itemInHand is null) return;
    itemInHand.GetComponent<SpriteRenderer>().enabled = true;
    itemInHand.GetComponent<BoxCollider2D>().enabled = true;
    itemInHand = null;
  }

  public static void ChoseIronStorage()
  {
    if (developReqIron)
      ActionManager.DevelopChoseIronStorage();
    else if (buildIronReq > 0)
    {
      ActionManager.BuildChoseIronStorage();
    }
  }

  public static void ChoseCoalStorage()
  {
    if (networkReqCoal)
      ActionManager.NetworkChoseCoalStorage();
    if (buildCoalReq > 0)
      ActionManager.BuildChoseCoalStorage();
  }
  public static void ClearAllLocalMemory()
  {
    overBuiltTile = null;

    allLocations.Clear();
    allIndustryLocations.Clear();
    allMerchants.Clear();
    allMerchantTiles.Clear();
    locationSpacesDict.Clear();

    redBreweries.Clear();
    yellowBreweries.Clear();
    whiteBreweries.Clear();
    purpleBreweries.Clear();
    redIronWorks.Clear();
    yellowIronWorks.Clear();
    whiteIronWorks.Clear();
    purpleIronWorks.Clear();
    redCoalMines.Clear();
    yellowCoalMines.Clear();
    whiteCoalMines.Clear();
    purpleCoalMines.Clear();
    redCottonMills.Clear();
    yellowCottonMills.Clear();
    whiteCottonMills.Clear();
    purpleCottonMills.Clear();
    redPotteries.Clear();
    yellowPotteries.Clear();
    whitePotteries.Clear();
    purplePotteries.Clear();
    redManufacturers.Clear();
    yellowManufacturers.Clear();
    whiteManufacturers.Clear();
    purpleManufacturers.Clear();

    VPTokens.Clear();
    incomeTokens.Clear();

    allNetworkSpacesBoat.Clear();
    allNetworkSpacesTrain.Clear();

    locationNetworkSpacesDictBoat.Clear();
    locationNetworkSpacesDictTrain.Clear();
  }
  public static void DestroyAllObjects()
  {
    if (!alreadyCreatedObjects) return;
    
    DestroyAllNetwork();
    DestroyAllBorders();

    DestroyAllBuildingTiles();
    DestroyAllMerchantTiles();
    DestroyAllResourcesInStorage();
    DestroyTokens();

    alreadyCreatedObjects = false;

  }
  public static void DestroyTokens()
  {
    foreach (GameObject token in incomeTokens)
      token.SetActive(false);

    incomeTokens.Clear();

    foreach (GameObject token in VPTokens)
      token.SetActive(false);

    VPTokens.Clear();
  }
  public static void DestroyAllBuildingTiles()
  {
    foreach (List<TileScript>[] industry in allTilesPerIndustry) {
      foreach (List<TileScript> playerTiles in industry)
      {
        foreach (TileScript tile in playerTiles)
          tile.Remove();
        playerTiles.Clear();
      }
    }
    foreach (LocationScript location in allIndustryLocations)
    {
      foreach (IndustrySpace space in locationSpacesDict[location])
      {
        space.RemoveBuiltIndustry();
      }
    }

    firstUnbuiltBreweryIndeces.Clear();
    firstUnbuiltIronWorksIndeces.Clear();
    firstUnbuiltCoalMineIndeces.Clear();
    firstUnbuiltCottonMillIndeces.Clear();
    firstUnbuiltPotteryIndeces.Clear();
    firstUnbuiltManufacturerIndeces.Clear();
  }

  public static void DestroyAllMerchantTiles()
  {
    foreach (LocationScript merchant in allMerchants)
    {
      foreach (MerchantTileSpace space in merchant.myMerchants)
      {
        if (space.myTile is not null)
        {
          space.myTile.gameObject.SetActive(false);
          space.myTile = null;
        }
        if (space.myBarrelSpace.hasBarrel)
        {
          space.myBarrelSpace.hasBarrel = false;
          space.myBarrelSpace.barrelObject.SetActive(false);
          space.myBarrelSpace.barrelObject = null;
        }
      }
    }
    //allMerchants.Clear();
  }


  public static void DestroyAllResourcesInStorage()
  {
    DestroyAllCoalInStorage();
    DestroyAllIronInStorage();
  }
  public static void DestroyAllCoalInStorage()
  {
    foreach (Transform spaceTransform in coalStorage.transform)
    {
      ResourceStorageSpace space = spaceTransform.gameObject.GetComponent<ResourceStorageSpace>();
      if (space.HasResource())
      {
        space.DestroyResource();
      }
    }
  }
  public static void DestroyAllIronInStorage()
  {
    foreach (Transform spaceTransform in ironStorage.transform)
    {
      ResourceStorageSpace space = spaceTransform.gameObject.GetComponent<ResourceStorageSpace>();
      if (space.HasResource())
      {
        space.DestroyResource();
      }
    }
  }

  public static void DestroyAllNetwork()
  {
    foreach (NetworkSpace child in allNetworkSpacesBoat)
      child.DestroyMyVehicle();
    foreach (NetworkSpace child in allNetworkSpacesTrain)
      child.DestroyMyVehicle();
  }
  static void UpdateHelpingArrays()
  {
    allBreweries[0] = redBreweries;
    allBreweries[1] = yellowBreweries;
    allBreweries[2] = whiteBreweries;
    allBreweries[3] = purpleBreweries;

    allCoalMines[0] = redCoalMines;
    allCoalMines[1] = yellowCoalMines;
    allCoalMines[2] = whiteCoalMines;
    allCoalMines[3] = purpleCoalMines;

    allCottonMills[0] = redCottonMills;
    allCottonMills[1] = yellowCottonMills;
    allCottonMills[2] = whiteCottonMills;
    allCottonMills[3] = purpleCottonMills;

    allIronWorks[0] = redIronWorks;
    allIronWorks[1] = yellowIronWorks;
    allIronWorks[2] = whiteIronWorks;
    allIronWorks[3] = purpleIronWorks;

    allPotteries[0] = redPotteries;
    allPotteries[1] = yellowPotteries;
    allPotteries[2] = whitePotteries;
    allPotteries[3] = purplePotteries;

    allManufacturers[0] = redManufacturers;
    allManufacturers[1] = yellowManufacturers;
    allManufacturers[2] = whiteManufacturers;
    allManufacturers[3] = purpleManufacturers;

    allRedTiles[0] = redBreweries;
    allRedTiles[1] = redCoalMines;
    allRedTiles[2] = redCottonMills;
    allRedTiles[3] = redIronWorks;
    allRedTiles[4] = redManufacturers;
    allRedTiles[5] = redPotteries;

    allYellowTiles[0] = yellowBreweries;
    allYellowTiles[1] = yellowCoalMines;
    allYellowTiles[2] = yellowCottonMills;
    allYellowTiles[3] = yellowIronWorks;
    allYellowTiles[4] = yellowManufacturers;
    allYellowTiles[5] = yellowPotteries;

    allWhiteTiles[0] = whiteBreweries;
    allWhiteTiles[1] = whiteCoalMines;
    allWhiteTiles[2] = whiteCottonMills;
    allWhiteTiles[3] = whiteIronWorks;
    allWhiteTiles[4] = whiteManufacturers;
    allWhiteTiles[5] = whitePotteries;

    allPurpleTiles[0] = purpleBreweries;
    allPurpleTiles[1] = purpleCoalMines;
    allPurpleTiles[2] = purpleCottonMills;
    allPurpleTiles[3] = purpleIronWorks;
    allPurpleTiles[4] = purpleManufacturers;
    allPurpleTiles[5] = purplePotteries;

    allTilesPerPlayer[0] = allRedTiles;
    allTilesPerPlayer[1] = allYellowTiles;
    allTilesPerPlayer[2] = allWhiteTiles;
    allTilesPerPlayer[3] = allPurpleTiles;

    allTilesPerIndustry[0] = allBreweries; 
    allTilesPerIndustry[1] = allCoalMines; 
    allTilesPerIndustry[2] = allCottonMills; 
    allTilesPerIndustry[3] = allIronWorks; 
    allTilesPerIndustry[4] = allManufacturers;
    allTilesPerIndustry[5] = allPotteries;
  }
  public static void CreateAllObjects()
  {
    if (alreadyCreatedObjects) return;
    CreateAllBuildingTiles();
    CreateAllMerchantTiles();
    FillMerchantBarrels();
    FillCoalStorage();
    FillIronStorage();
    CreateTokens();
    UpdateHelpingArrays();
  }
  static void InitFirstUnbuiltindeces()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      firstUnbuiltBreweryIndeces.Add(0);
      firstUnbuiltIronWorksIndeces.Add(0);
      firstUnbuiltCoalMineIndeces.Add(0);
      firstUnbuiltCottonMillIndeces.Add(0);
      firstUnbuiltPotteryIndeces.Add(0);
      firstUnbuiltManufacturerIndeces.Add(0);
    }
  }
  public static void CreateAllBuildingTiles()
  {

    InitFirstUnbuiltindeces();

    GameObject player1Board = GameObject.Find(Constants.player1PersonalBoardName);
    GameObject player2Board = GameObject.Find(Constants.player2PersonalBoardName);
    GameObject player3Board = GameObject.Find(Constants.player3PersonalBoardName);
    GameObject player4Board = GameObject.Find(Constants.player4PersonalBoardName);
    GameObject[] playerBoards = { player1Board, player2Board, player3Board, player4Board };

    string[] playerFolders = { Constants.redTilesPath, Constants.yellowTilesPath, Constants.whiteTilesPath, Constants.purpleTilesPath };


    int playerIndex = 0;
    foreach (GameObject playerBoard in playerBoards)
    {
      //Debug.Log($"Player board is {playerBoard}");
      GameObject industrySpaces = playerBoard.transform.Find(Constants.IndustrySpacesOnPersonalBoardParentName).gameObject;

      GameObject breweries = industrySpaces.transform.Find(Constants.breweriesParentName).gameObject;
      GameObject manufacturers = industrySpaces.transform.Find(Constants.manufacturersParentName).gameObject;
      GameObject cottonMills = industrySpaces.transform.Find(Constants.cottonMillsParentName).gameObject;
      GameObject potteries = industrySpaces.transform.Find(Constants.potteriesParentName).gameObject;
      GameObject ironWorks = industrySpaces.transform.Find(Constants.ironWorksParentName).gameObject;
      GameObject coalMines = industrySpaces.transform.Find(Constants.coalMinesParentName).gameObject;


      //Create each industry seperately -> add id based on type, index, level and ownerPlayerIndex

      int levelCounter = 0;
      foreach (Transform brewerySource in breweries.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.breweryNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.breweryAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = brewerySource.transform.position;
          allBreweries[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 1*10 + i;

        }
        levelCounter++;
      }

      levelCounter = 0;
      foreach (Transform manufacturerSource in manufacturers.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.manufacturerNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.manufacturerAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = manufacturerSource.transform.position;
          allManufacturers[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 2 * 10 + i;

        }
        levelCounter++;
      }

      levelCounter = 0;
      foreach (Transform cottonMillsSource in cottonMills.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.cottonMillNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.cottonMillAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = cottonMillsSource.transform.position;
          allCottonMills[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 3 * 10 + i;

        }
        levelCounter++;
      }


      levelCounter = 0;
      foreach (Transform potteriesSource in potteries.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.potteryNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.potteryAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = potteriesSource.transform.position;
          allPotteries[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 4 * 10 + i;

        }
        levelCounter++;
      }



      levelCounter = 0;
      foreach (Transform ironWorksSource in ironWorks.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.ironWorksNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.ironWorksAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = ironWorksSource.transform.position;
          allIronWorks[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 5 * 10 + i;

        }
        levelCounter++;
      }



      levelCounter = 0;
      foreach (Transform coalMineSource in coalMines.transform)
      {
        var newObjectResource = HelpFunctions.LoadPrefabFromFile(playerFolders[playerIndex] + Constants.coalMineNames[levelCounter]);
        //var newObjectResource = LoadPrefabFromFile("Tiles/Purple/Brewery1");

        for (int i = 0; i < Constants.coalMineAmounts[levelCounter]; i++)
        {
          TileScript newObject = (Instantiate(newObjectResource) as GameObject).GetComponent<TileScript>();
          newObject.transform.position = coalMineSource.transform.position;
          allCoalMines[playerIndex].Add(newObject);
          newObject.id = playerIndex * 1000 + levelCounter * 100 + 6 * 10 + i;

        }
        levelCounter++;
      }


      allBreweries[playerIndex].Sort(SortByLevel);
      allCoalMines[playerIndex].Sort(SortByLevel);
      allCottonMills[playerIndex].Sort(SortByLevel);
      allIronWorks[playerIndex].Sort(SortByLevel);
      allManufacturers[playerIndex].Sort(SortByLevel);
      allPotteries[playerIndex].Sort(SortByLevel);

      playerIndex++;
    }



    alreadyCreatedObjects = true;

  }

  public static void CreateAllMerchantTiles()
  {
    List<MerchantTileScript> loadedTiles = new();
    int id = 0;
    foreach (string path in Constants.allMerchantTilesPaths)
    {
      var tileResource = HelpFunctions.LoadPrefabFromFile(path);
      MerchantTileScript newTile = (Instantiate(tileResource) as GameObject).GetComponent<MerchantTileScript>();
      newTile.id = id++;
      if (newTile.minPlayers <= GameManager.numOfPlayers)
      {
        loadedTiles.Add(newTile);
      }
      //else Debug.Log("Not enough players for this tile " + newTile.ToString());
    }
    loadedTiles = HelpFunctions.GetShuffledList(loadedTiles);
    allMerchantTiles = loadedTiles;
    //Debug.Log("Loaded total of " + loadedTiles.Count.ToString() + " merchant tiles");
    int tileIndex = 0;
    foreach (LocationScript merchant in allMerchants)
    {
      foreach (MerchantTileSpace space in merchant.myMerchants)
      {
        if (tileIndex >= loadedTiles.Count) continue;
        if (space.minPlayersToOpen <= GameManager.numOfPlayers)
        {
          loadedTiles[tileIndex].transform.position = space.transform.position;
          space.myTile = loadedTiles[tileIndex];
          tileIndex++;
        }
      }
    }
    if (tileIndex < loadedTiles.Count - 1)
    {
      Debug.Log("Not all merchant tiles found their home");
    }

  }

  public static void FillMerchantBarrels()
  {
    foreach (LocationScript merchant in allMerchants)
    {
      foreach (MerchantTileSpace space in merchant.myMerchants)
      {
        if (space.myTile is null) continue;
        MerchantTileScript tile = space.myTile;
        if (!tile.hasCot && !tile.hasMan && !tile.hasPottery) continue;
        BarrelSpace barrelSpace = space.myBarrelSpace;
        barrelSpace.AddBarrel();

      }
    }
  }

  public static void FillIronStorage()
  {
    foreach (Transform spaceTransform in ironStorage.transform)
    {
      AddIronToStorage();
    }

    //Based on rules -> initially iron storage is missing 2 irons
    GetIronFromStorage(out _).SetActive(false);
    GetIronFromStorage(out _).SetActive(false);

  }
  public static void FillCoalStorage()
  {
    foreach (Transform spaceTransform in coalStorage.transform)
    {
      AddCoalToStorage();
    }
    //Based on rules -> initially coal storage is missing 1 coal
    GetCoalFromStorage(out _).SetActive(false);
  }
  public static int GetCoalStoragePrice() => coalStorage.CurrentPrice();
  public static int GetIronStoragePrice() => ironStorage.CurrentPrice();
  /// <summary>
  /// Get cheapest coal from storage -> doesn't destroy the coal object
  /// </summary>
  public static GameObject GetCoalFromStorage(out int price)
  {
    return coalStorage.GetCheapestResource(out price);
  }
  /// <summary>
  /// Get cheapest iron from storage -> doesn't destroy the iron object
  /// </summary>
  public static GameObject GetIronFromStorage(out int price)
  {
    return ironStorage.GetCheapestResource(out price);
  }
  /// <summary>
  /// Adds coal to storage
  /// </summary>
  /// <param name="coal">If given coal is not given or is null -> storage creates a new coal object</param>
  /// <returns>Price for which the coal was added</returns>
  public static int AddCoalToStorage(GameObject coal = null)
  {
    return coalStorage.AddMostExpensiveResource(coal);
  }
  /// <summary>
  /// Adds iron to storage
  /// </summary>
  /// <param name="iron">If given iron is not given or is null -> storage creates a new iron object</param>
  /// <returns>Price for which the iron was added</returns>
  public static int AddIronToStorage(GameObject iron = null)
  {
    return ironStorage.AddMostExpensiveResource(iron);
  }
  public static bool HasCoalStorageFreeSpace() => coalStorage.HasFreeSpace();
  public static bool HasIronStorageFreeSpace() => ironStorage.HasFreeSpace();
  public static void ChoseNetwork(NetworkSpace space, int playerIndex = -1)
  {
    if (space.IsOccupied())
    {
      Debug.Log("Network already occupied!");
      return;
    }
    if (playerIndex == -1)
      playerIndex = GameManager.activePlayerIndex;


    space.AddVehicle(playerIndex);

    ActionManager.NetworkAdded(space);

  }

  public static void RemoveNetwork(NetworkSpace space)
  {
    space.DestroyMyVehicle();
  }

  static private int SortByLevel(TileScript p1, TileScript p2)
  {
    return p1.level.CompareTo(p2.level);
  }

  static bool CanTileBeBuilt(TileScript tile)
  {
    return tile.level == GetCurrentIndustryLevel(tile.industryType, out bool outOfLimits) && !outOfLimits && !tile.alreadyBuilt &&
      !((GameManager.currentEra == ERA.BOAT && !tile.canBeBuiltInBoatEra) || (GameManager.currentEra == ERA.TRAIN && !tile.canBeBuiltInTrainEra)); //Tile isn't limited to different era
      
  }
  static bool CanTileBeDeveloped(TileScript tile)
  {
    int curIndustryLvl = GetCurrentIndustryLevel(tile.industryType, out _);
    //Debug.Log($"Cur industry level of {tile.industryType} is {curIndustryLvl}");

    return tile.level == GetCurrentIndustryLevel(tile.industryType, out bool outOfLimits) && !outOfLimits && !tile.alreadyBuilt && tile.canBeDeveloped && !tile.isDeveloped; //Tile isn't limited to different era

  }

  static private void PrintLevels(List<TileScript> list)
  {
    Debug.Log("Printing list of all " + list[0].industryType);
    foreach (TileScript item in list)
    {
      Debug.Log(item.level);
    }
  }

  static public GameObject CreateIron()
  {
    var barrelResource = HelpFunctions.LoadPrefabFromFile(Constants.ironPath);
    return Instantiate(barrelResource) as GameObject;
  }
  static public GameObject CreateCoal()
  {
    var barrelResource = HelpFunctions.LoadPrefabFromFile(Constants.coalPath);
    return Instantiate(barrelResource) as GameObject;
  }

  /// <summary>
  /// Adds unbuilt index to active player based on tile industry type -> next in line to be built could be a level higher
  /// </summary>
  /// <param name="succesful">Whether the addition was succesful -> could be out of range</param>
  static void AddFirstUnbuiltIndex(TileScript tile, out bool succesful)
  {
    int activePlayerIndex = GameManager.activePlayerIndex;
    succesful = true;
    switch (tile.industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        if (firstUnbuiltBreweryIndeces[activePlayerIndex] >= allBreweries[activePlayerIndex].Count) succesful = false;
        firstUnbuiltBreweryIndeces[activePlayerIndex]++;
        //Debug.Log("Increased index of brewery to " + firstUnbuiltBreweryIndeces[activePlayerIndex].ToString());
        //PrintLevels(allBreweries);
        break;
      case INDUSTRY_TYPE.COALMINE:
        if (firstUnbuiltCoalMineIndeces[activePlayerIndex] >= allCoalMines[activePlayerIndex].Count) succesful = false;
        firstUnbuiltCoalMineIndeces[activePlayerIndex]++;
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        if (firstUnbuiltCottonMillIndeces[activePlayerIndex] >= allCottonMills[activePlayerIndex].Count) succesful = false;
        firstUnbuiltCottonMillIndeces[activePlayerIndex]++;
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        if (firstUnbuiltIronWorksIndeces[activePlayerIndex] >= allIronWorks[activePlayerIndex].Count) succesful = false;
        firstUnbuiltIronWorksIndeces[activePlayerIndex]++;
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        if (firstUnbuiltManufacturerIndeces[activePlayerIndex] >= allManufacturers[activePlayerIndex].Count) succesful = false;
        firstUnbuiltManufacturerIndeces[activePlayerIndex]++;
        break;
      case INDUSTRY_TYPE.POTTERY:
        if (firstUnbuiltPotteryIndeces[activePlayerIndex] >= allPotteries[activePlayerIndex].Count) succesful = false;
        firstUnbuiltPotteryIndeces[activePlayerIndex]++;
        break;
      default:
        break;
    }
  }
  /// <summary>
  /// Subtracts unbuilt index to active player based on tile industry type -> next in line to be built could be a level lower
  /// </summary>
  /// <param name="succesful">Whether the subtraction was succesful -> could be out of range</param>
  static void SubtractFirstUnbuiltIndex(TileScript tile, out bool succesful)
  {
    int activePlayerIndex = GameManager.activePlayerIndex;
    succesful = true;
    switch (tile.industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        if (firstUnbuiltBreweryIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltBreweryIndeces[activePlayerIndex]--;
        //Debug.Log("Increased index of brewery to " + firstUnbuiltBreweryIndeces[activePlayerIndex].ToString());
        //PrintLevels(allBreweries);
        break;
      case INDUSTRY_TYPE.COALMINE:
        if (firstUnbuiltCoalMineIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltCoalMineIndeces[activePlayerIndex]--;
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        if (firstUnbuiltCottonMillIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltCottonMillIndeces[activePlayerIndex]--;
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        if (firstUnbuiltIronWorksIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltIronWorksIndeces[activePlayerIndex]--;
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        if (firstUnbuiltManufacturerIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltManufacturerIndeces[activePlayerIndex]--;
        break;
      case INDUSTRY_TYPE.POTTERY:
        if (firstUnbuiltPotteryIndeces[activePlayerIndex] <= 0) succesful = false;
        firstUnbuiltPotteryIndeces[activePlayerIndex]--;
        break;
      default:
        break;
    }
  }

  static public void BuildHeldIndustry()
  {
    AddFirstUnbuiltIndex(itemInHand, out bool success);
    if (!success)
    {
      Debug.LogError("Can't build tile with level under 1");
      return;
    }

    if (chosenBuildSpace.myTile is not null)
      overBuiltTile = chosenBuildSpace.myTile;

    chosenBuildSpace.industryTypeOfBuiltTile = itemInHand.industryType;
    itemInHand.transform.position = chosenBuildSpace.transform.position;
    //Debug.Log($"Moved {itemInHand} to position of {chosenBuildSpace}");
    itemInHand.builtOnSpace = chosenBuildSpace;


    chosenBuildSpace.myTile = itemInHand;

    itemInHand.BecomeBuilt();
    //DropItem();
  }
  static public void UnBuildHeldIndustry()
  {
    if (itemInHand is null || !itemInHand.alreadyBuilt) return;

    SubtractFirstUnbuiltIndex(itemInHand, out bool success);
    if (!success)
    {
      Debug.LogError("Can't unBuild tile with level under 1");
      return;
    }

    if(chosenBuildSpace is not null)
      chosenBuildSpace.RemoveBuiltIndustry();
    if(overBuiltTile is not null)
    {
      chosenBuildSpace.industryTypeOfBuiltTile = overBuiltTile.industryType;
      chosenBuildSpace.myTile = overBuiltTile;
    }

    itemInHand.transform.position = GetPositionOfTileSource(itemInHand, GameManager.activePlayerIndex);
    itemInHand.BecomeUnbuilt();

  }
  /// <summary>
  /// Get the position of player personal board, where the given tile comes from
  /// </summary>
  static Vector3 GetPositionOfTileSource(TileScript tile, int playerIndex)
  {
    GameObject playerBoard;
    if (playerIndex == 0) playerBoard = GameObject.Find(Constants.player1PersonalBoardName);
    else if (playerIndex == 1) playerBoard = GameObject.Find(Constants.player2PersonalBoardName);
    else if (playerIndex == 2) playerBoard = GameObject.Find(Constants.player3PersonalBoardName);
    else playerBoard = GameObject.Find(Constants.player4PersonalBoardName);

    GameObject industrySpaces = playerBoard.transform.Find(Constants.IndustrySpacesOnPersonalBoardParentName).gameObject;


    GameObject sources;


    switch (tile.industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        sources = industrySpaces.transform.Find(Constants.breweriesParentName).gameObject;
        break;
      case INDUSTRY_TYPE.COALMINE:
        sources = industrySpaces.transform.Find(Constants.coalMinesParentName).gameObject;
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        sources = industrySpaces.transform.Find(Constants.cottonMillsParentName).gameObject;
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        sources = industrySpaces.transform.Find(Constants.ironWorksParentName).gameObject;
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        sources = industrySpaces.transform.Find(Constants.manufacturersParentName).gameObject;
        break;
      case INDUSTRY_TYPE.POTTERY:
        sources = industrySpaces.transform.Find(Constants.potteriesParentName).gameObject;
        break;
      case INDUSTRY_TYPE.NONE:
      default:
        Debug.LogError("Can't get position of source of tile with type NONE");
        return Vector3.zero;
    }

    Transform source = sources.transform.GetChild(tile.level-1);


    return source.position;

  }

  public static void ChooseSpace(IndustrySpace space)
  {
    if (!canChooseSpace) return;

    chosenBuildSpace = space;

    ActionManager.BuildChoseIndustrySpace();
  }

  public static void ForgetSpace()
  {
    chosenBuildSpace = null;
  }

  static public int GetCurrentIndustryLevel(INDUSTRY_TYPE type, out bool outOfLimits)
  {
    int activePlayerIndex = GameManager.activePlayerIndex;
    outOfLimits = false;
    switch (type)
    {
      case INDUSTRY_TYPE.BREWERY:
        if (firstUnbuiltBreweryIndeces[activePlayerIndex] < 0 || firstUnbuiltBreweryIndeces[activePlayerIndex] >= allBreweries[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allBreweries[activePlayerIndex][firstUnbuiltBreweryIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.COALMINE:
        if (firstUnbuiltCoalMineIndeces[activePlayerIndex] < 0 || firstUnbuiltCoalMineIndeces[activePlayerIndex] >= allCoalMines[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allCoalMines[activePlayerIndex][firstUnbuiltCoalMineIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.COTTONMILL:
        if (firstUnbuiltCottonMillIndeces[activePlayerIndex] < 0 || firstUnbuiltCottonMillIndeces[activePlayerIndex] >= allCottonMills[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allCottonMills[activePlayerIndex][firstUnbuiltCottonMillIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.IRONWORKS:
        if (firstUnbuiltIronWorksIndeces[activePlayerIndex] < 0 || firstUnbuiltIronWorksIndeces[activePlayerIndex] >= allIronWorks[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allIronWorks[activePlayerIndex][firstUnbuiltIronWorksIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.MANUFACTURER:
        if (firstUnbuiltManufacturerIndeces[activePlayerIndex] < 0 || firstUnbuiltManufacturerIndeces[activePlayerIndex] >= allManufacturers[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allManufacturers[activePlayerIndex][firstUnbuiltManufacturerIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.POTTERY:
        if (firstUnbuiltPotteryIndeces[activePlayerIndex] < 0 || firstUnbuiltPotteryIndeces[activePlayerIndex] >= allPotteries[activePlayerIndex].Count)
        {
          outOfLimits = true;
          return -1;
        }
        return allPotteries[activePlayerIndex][firstUnbuiltPotteryIndeces[activePlayerIndex]].level;

      case INDUSTRY_TYPE.NONE:

      default:
        Debug.LogError("Invalid type");
        return -1;
    }
  }

  static public TileScript GetLowestLevelTileOfType(INDUSTRY_TYPE type, int playerIndex)
  {
    List<TileScript> correctTiles;
    int index = int.MaxValue;
    switch (type)
    {
      case INDUSTRY_TYPE.BREWERY:

        if (firstUnbuiltBreweryIndeces.Count <= 0)
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltBreweryIndeces[playerIndex];

        correctTiles = allBreweries[playerIndex];
        break;
      case INDUSTRY_TYPE.COALMINE:
        if (firstUnbuiltCoalMineIndeces.Count <= 0)
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltCoalMineIndeces[playerIndex];
        correctTiles = allCoalMines[playerIndex];
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        if (firstUnbuiltCottonMillIndeces.Count <= 0)
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltCottonMillIndeces[playerIndex];
        correctTiles = allCottonMills[playerIndex];
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        if (firstUnbuiltIronWorksIndeces.Count <= 0)        
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltIronWorksIndeces[playerIndex];
        correctTiles = allIronWorks[playerIndex];
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        if (firstUnbuiltManufacturerIndeces.Count <= 0)        
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltManufacturerIndeces[playerIndex];
        correctTiles = allManufacturers[playerIndex];
        break;
      case INDUSTRY_TYPE.POTTERY:
        if (firstUnbuiltPotteryIndeces.Count <= 0)        
        {
          //Debug.LogError("FirstUnbuiltIndeces not yet initialized!");
          return null;
        }

        index = firstUnbuiltPotteryIndeces[playerIndex];
        correctTiles = allPotteries[playerIndex];
        break;
      case INDUSTRY_TYPE.NONE:
      default:
        return null;
    }

    if (index >= correctTiles.Count)//Edge case where there are no more unbuilt
    {
      //Debug.Log("No more tiles to be built!");
      return null;
    }
    return correctTiles[index];


  }
  static public TileScript GetTileOfNextLevel(TileScript tile)
  {
    List<TileScript> tilesOfCorrectType;
    switch (tile.industryType)
    {
      case INDUSTRY_TYPE.BREWERY:
        tilesOfCorrectType = allBreweries[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.COALMINE:
        tilesOfCorrectType = allCoalMines[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        tilesOfCorrectType = allCottonMills[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        tilesOfCorrectType = allIronWorks[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        tilesOfCorrectType = allManufacturers[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.POTTERY:
        tilesOfCorrectType = allPotteries[tile.ownerPlayerIndex];
        break;
      case INDUSTRY_TYPE.NONE:
      default:
        return null;
    }

    foreach (TileScript curTile in tilesOfCorrectType)
      if (curTile.level == tile.level + 1)
        return curTile;

    return null;
  }
  static public List<TileScript> GetViableTilesForCurrentAction()
  {
    ACTION action = ActionManager.currentAction;
    int activePlayer = GameManager.activePlayerIndex;


    List<TileScript> viableTiles = new List<TileScript>();
    switch (action)
    {
      case ACTION.BUILD:
        foreach (List<TileScript> tiles in allTilesPerPlayer[activePlayer])
          foreach (TileScript tile in tiles)
          {
            if (CanTileBeBuilt(tile) && (chosenBuildSpace is null || 
                (chosenBuildSpace.myTypes.Contains(tile.industryType) && 
                (chosenBuildSpace.myTile is null || chosenBuildSpace.myTile.industryType == tile.industryType))) &&
                GameManager.ActivePlayerHasEnoughMoney(tile.buildCost))
              viableTiles.Add(tile);
          }
        break;
      case ACTION.SELL:
        foreach (List<TileScript> tiles in allTilesPerPlayer[activePlayer])
          foreach (TileScript tile in tiles)
          {
            if (!tile.alreadyBuilt) continue;

            List<MerchantTileSpace> connectedMerchants = GetAllConnectedMerchantTiles(tile.builtOnSpace.myLocation, tile.industryType);
            bool barrelAvailable = false;
            bool merchantOfMyTypeAvailable = connectedMerchants.Count > 0;

            foreach (MerchantTileSpace merchant in connectedMerchants)
              if (merchant.myBarrelSpace.hasBarrel) barrelAvailable = true;

            if (!barrelAvailable)
              if (GetAllSpacesWithAvailableBarrels(tile.builtOnSpace.myLocation).Count > 0) barrelAvailable = true;

            //Debug.Log(tile + " has location " + tile.builtOnSpace.myLocation );
            //Debug.Log(tile + " has found " + GetAllConnectedMerchantTiles(tile.builtOnSpace.myLocation, tile.industryType).Count + " correct merchants");
            if (tile.alreadyBuilt && !tile.isUpgraded &&
              merchantOfMyTypeAvailable && //Check whether there is correct merchant connected
              barrelAvailable) //Check if there is a barrel to pay for trade
              viableTiles.Add(tile);
          }
        break;
      case ACTION.LOAN:
        break;
      case ACTION.SCOUT:
        break;
      case ACTION.DEVELOP:
        foreach (List<TileScript> tiles in allTilesPerPlayer[activePlayer])
          foreach (TileScript tile in tiles)
          {
            if (CanTileBeDeveloped(tile))
              viableTiles.Add(tile);
          }
        break;
      case ACTION.NETWORK:
        break;
      case ACTION.NONE:
        break;
      default:
        break;
    }
    return viableTiles;
  }

  static public void HighlightIndustrySpaces(List<IndustrySpace> spaces)
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);


    foreach (IndustrySpace space in spaces)
    {
      var newBorder = Instantiate(borderResource) as GameObject;
      newBorder.transform.position = space.transform.position;
      allSpaceBorders.Add(newBorder);
    }
  }

  static public void MakeCorrectNetworkSpacesClickable()
  {
    List<NetworkSpace> correctSpaces = GetMyNetworkNeighborConnections(GameManager.activePlayerIndex);
    foreach (NetworkSpace space in correctSpaces)
      space.BecomeClickable();
  }

  static public void MakeAllNetworkSpacesUnclickable()
  {
    foreach (NetworkSpace space in allNetworkSpacesBoat)
      space.BecomeUnclickable();
    foreach (NetworkSpace space in allNetworkSpacesTrain)
      space.BecomeUnclickable();
  }

  static public void MakeAllIndustrySpacesUnclickable()
  {
    foreach (LocationScript location in allIndustryLocations)
      foreach (IndustrySpace space in locationSpacesDict[location])
        space.BecomeUnclickable();
  }
  static public void MakeCorrectIndustrySpacesClickable()
  {
    List<IndustrySpace> myNetworkFreeSpaces = GetMyNetworkFreeSpacesForItemInHand(GameManager.activePlayerIndex);
    foreach (IndustrySpace space in myNetworkFreeSpaces)
      space.BecomeClickable();
  }
  static public void MakeCorrectIndustrySpacesClickable(CardScript card)
  {
    List<IndustrySpace> correctSpaces = GetAllViableBuildSpaces(card);
    foreach (IndustrySpace space in correctSpaces)
      space.BecomeClickable();
  }

  static public void HighlightNetworkSpaces(List<NetworkSpace> spaces)
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);

    foreach (NetworkSpace space in spaces)
    {

      var newBorder = Instantiate(borderResource) as GameObject;
      newBorder.transform.SetPositionAndRotation(space.transform.position, space.transform.rotation);
      allNetworkBorders.Add(newBorder);
    }
  }


  static public List<IndustrySpace> GetAllViableBuildSpaces(CardScript card)
  {
    List<IndustrySpace> viableSpaces;
    switch (card.myType)
    {
      case CARD_TYPE.LOCATION:
        viableSpaces = GetViableBuildSpacesLocationCard(card);
        break;
      case CARD_TYPE.INDUSTRY:
        viableSpaces = GetViableBuildSpacesIndustryCard(card);

        break;
      case CARD_TYPE.WILD_LOCATION:
        viableSpaces = GetViableBuildSpacesWildLocationCard();

        break;
      case CARD_TYPE.WILD_INDUSTRY:
        viableSpaces = GetViableBuildSpacesWildIndustryCard();

        break;
      default:
        viableSpaces = new();
        break;
    }

    return viableSpaces;
  }

  static List<IndustrySpace> GetViableBuildSpacesLocationCard(CardScript card)
  {
    if (card.myType != CARD_TYPE.LOCATION)
      Debug.LogError("Wrong card type!");

    foreach (LocationScript location in allIndustryLocations)
    {
      if (location.name == card.myLocation)
      {
        bool occupiedLocation = false;
        List<IndustrySpace> curFreeSpaces = new();
        List<IndustrySpace> returnSpaces = new();
        foreach (IndustrySpace space in locationSpacesDict[location])
        {
          if (space.myTile is null)
            curFreeSpaces.Add(space);
          else 
          {
            if (CanTileBeOverBuilt(space.myTile, GameManager.activePlayerIndex))
            {
              DisableCollider(space.myTile);
              returnSpaces.Add(space);
            }
            //Check if in Boat ERA there isn't already an industry from this player 
            if (GameManager.currentEra == ERA.BOAT && space.myTile.ownerPlayerIndex == GameManager.activePlayerIndex) occupiedLocation = true; 
          }
        }
        if (!occupiedLocation)
          foreach (IndustrySpace space in curFreeSpaces)
            returnSpaces.Add(space);
        return returnSpaces;
      }
    }
    return new List<IndustrySpace>();
  }
  static public List<IndustrySpace> GetViableBuildSpacesWildLocationCard()
  {
    List<IndustrySpace> allSpaces = new();
    foreach (LocationScript location in allIndustryLocations)
      if (location.myType == LocationType.CITY) //Nameless breweries and Merchants are not possible from wild location
      {
        bool occupiedLocation = false;
        List<IndustrySpace> curFreeSpaces = new();
        foreach (IndustrySpace space in locationSpacesDict[location])
        {
          if (space.myTile is null)
            curFreeSpaces.Add(space);
          else
          {
            if (CanTileBeOverBuilt(space.myTile, GameManager.activePlayerIndex))
            {
              DisableCollider(space.myTile);
              allSpaces.Add(space);
            }
            //Check if in Boat ERA there isn't already an industry from this player 
            if (GameManager.currentEra == ERA.BOAT && space.myTile.ownerPlayerIndex == GameManager.activePlayerIndex) occupiedLocation = true;
          }
        }
        if (!occupiedLocation)
          foreach (IndustrySpace space in curFreeSpaces)
            allSpaces.Add(space);
      }
    return allSpaces;
  }
  static List<IndustrySpace> GetViableBuildSpacesIndustryCard(CardScript card)
  {
    if (card.myType != CARD_TYPE.INDUSTRY)
      Debug.LogError("Wrong card type!");

    List<LocationScript> myNetworkLocations = GetMyNetworkLocations(GameManager.activePlayerIndex);

    if (myNetworkLocations.Count <= 0)
      myNetworkLocations = allIndustryLocations;
    List<IndustrySpace> viableSpaces = new();


    foreach (LocationScript location in myNetworkLocations)
    {
      if (location.myType == LocationType.MERCHANT) continue;
      int playerIndex = GameManager.activePlayerIndex;

      bool occupiedLocation = false;
      List<IndustrySpace> curFreeSpaces = new();
      foreach (IndustrySpace space in locationSpacesDict[location])
      {
        if (space.myTile is not null)
        {
          TileScript tile = space.myTile;
          if (GameManager.currentEra == ERA.BOAT)
          {
            if (tile.ownerPlayerIndex == playerIndex)
              occupiedLocation = true;
          }
          if (CanTileBeOverBuilt(tile, playerIndex))
          {
            foreach (INDUSTRY_TYPE type in card.myIndustries)
              if (space.CanBuildIndustryType(type))
              {
                DisableCollider(tile);
                viableSpaces.Add(space);
                break;
              }
          }
        }

        else
        {
          foreach (INDUSTRY_TYPE type in card.myIndustries)
            if (space.CanBuildIndustryType(type))
            {
              curFreeSpaces.Add(space);
              break;
            }
        }

      }
      if (!occupiedLocation)
        foreach (IndustrySpace space in curFreeSpaces)
          viableSpaces.Add(space);
    }

    return viableSpaces;
  }
  static public List<IndustrySpace> GetViableBuildSpacesWildIndustryCard()
  {
    if (itemInHand is not null)
      return GetMyNetworkFreeSpacesForItemInHand(GameManager.activePlayerIndex);
    else
      return GetMyNetworkFreeSpaces(GameManager.activePlayerIndex);
  }

  static public void HighlightTiles<T>(List<T> tiles) where T : TileScript
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);

    foreach (T tile in tiles)
    {
      var newBorder = Instantiate(borderResource) as GameObject;
      newBorder.transform.position = tile.transform.position;
      allTileBorders.Add(newBorder);
    }
  }

  static public List<TileScript> GetAllViableTiles(CardScript card)
  {
    List<TileScript> viableTiles;
    switch (card.myType)
    {
      case CARD_TYPE.LOCATION:
        viableTiles = GetViableTilesLocationCard(card);
        break;
      case CARD_TYPE.INDUSTRY:
        viableTiles = GetViableTilesIndustryCard(card);

        break;
      case CARD_TYPE.WILD_LOCATION:
        viableTiles = GetViableTilesWildLocationCard();

        break;
      case CARD_TYPE.WILD_INDUSTRY:
        viableTiles = GetViableTilesWildIndustryCard();

        break;
      default:
        viableTiles = new();
        break;
    }

    List<TileScript> viableAffordableTiles = new();
    foreach (TileScript tile in viableTiles)
    {
      if (GameManager.ActivePlayerHasEnoughMoney(tile.buildCost))
      {
        //Debug.Log(tile + " with cost " + tile.buildCost + " is affordable ");
        viableAffordableTiles.Add(tile);
      }
    }

    return viableAffordableTiles;
  }

  static List<TileScript> GetViableTilesLocationCard(CardScript card)
  {
    if (card.myType != CARD_TYPE.LOCATION)
      Debug.LogError("Wrong card type!");

    if (chosenBuildSpace is null) return new List<TileScript>();

    List<TileScript> viableTiles = new();
    foreach (List<TileScript> tiles in allTilesPerPlayer[GameManager.activePlayerIndex])
      foreach (TileScript tile in tiles)
      {

        if (CanTileBeBuilt(tile) && chosenBuildSpace.myTypes.Contains(tile.industryType))
        {
          viableTiles.Add(tile);
        }
      }
    return viableTiles;
  }
  static public List<TileScript> GetViableTilesWildLocationCard()
  {
    if (chosenBuildSpace is null) return new List<TileScript>();

    List<TileScript> viableTiles = new();
    foreach (List<TileScript> tiles in allTilesPerPlayer[GameManager.activePlayerIndex])
      foreach (TileScript tile in tiles)
      {
        if (CanTileBeBuilt(tile) && chosenBuildSpace.myTypes.Contains(tile.industryType))
        {
          viableTiles.Add(tile);
        }
      }
    return viableTiles;
  }
  static List<TileScript> GetViableTilesIndustryCard(CardScript card)
  {
    if (card.myType != CARD_TYPE.INDUSTRY)
      Debug.LogError("Wrong card type!");

    List<TileScript> viableTiles = new();
    foreach (List<TileScript> tiles in allTilesPerPlayer[GameManager.activePlayerIndex])
      foreach (TileScript tile in tiles)
      {
        if (CanTileBeBuilt(tile) && card.myIndustries.Contains(tile.industryType))
          viableTiles.Add(tile);
      }

    return viableTiles;
  }
  static public List<TileScript> GetViableTilesWildIndustryCard()
  {
    List<TileScript> viableTiles = new();
    foreach (List<TileScript> tiles in allTilesPerPlayer[GameManager.activePlayerIndex])
      foreach (TileScript tile in tiles)
      {
        if (CanTileBeBuilt(tile))
          viableTiles.Add(tile);
      }

    return viableTiles;
  }


  static public List<IndustrySpace> GetAllIndustrySpacesInLocation(LocationScript location)
  {
    if (location.myType == LocationType.MERCHANT)
    {
      Debug.LogError("Can't get industry spaces of Merchant");
      return new List<IndustrySpace>();
    }
    return locationSpacesDict[location];
  }
  static public IndustrySpace GetIndustrySpaceByID(int id)
  {
    foreach(LocationScript location in allIndustryLocations)
      foreach (IndustrySpace space in locationSpacesDict[location])
        if (space.id == id) return space;

    Debug.LogError("Failed to find industry space id");
    return null;
    
  }
  static public MerchantTileScript GetMerchantTileScriptByID(int id)
  {
    foreach (MerchantTileScript tile in allMerchantTiles)
      if (tile.id == id) return tile;

    Debug.LogError($"Failed to find merchantTile with id {id}");
    return null;
  }
  static public List<TileScript> GetAllMyBuiltTiles(int playerIndex)
  {
    List<TileScript> myTiles = new();

    foreach (List<TileScript> industryTiles in allTilesPerPlayer[playerIndex])
      foreach (TileScript tile in industryTiles)
        if (tile.alreadyBuilt && tile.ownerPlayerIndex == playerIndex) myTiles.Add(tile);

    return myTiles;
  }
  static public List<TileScript> GetAllMyUnbuiltTiles(int playerIndex, int level, INDUSTRY_TYPE type)
  {
    List<TileScript> returnList = new();
    List<TileScript> allTiles = new();
    switch (type)
    {
      case INDUSTRY_TYPE.BREWERY:
        allTiles = allBreweries[playerIndex];
        break;
      case INDUSTRY_TYPE.COALMINE:
        allTiles = allCoalMines[playerIndex];
        break;
      case INDUSTRY_TYPE.COTTONMILL:
        allTiles = allCottonMills[playerIndex];
        break;
      case INDUSTRY_TYPE.IRONWORKS:
        allTiles = allIronWorks[playerIndex];
        break;
      case INDUSTRY_TYPE.MANUFACTURER:
        allTiles = allManufacturers[playerIndex];
        break;
      case INDUSTRY_TYPE.POTTERY:
        allTiles = allPotteries[playerIndex];
        break;
      default:
        break;
    }

    foreach (TileScript tile in allTiles)
      if (tile.level == level && !tile.alreadyBuilt && !tile.isDeveloped) returnList.Add(tile);

    return returnList;
  }
  /// <summary>
  /// Get all network space where the given player has a vehicle
  /// </summary>
  static public List<NetworkSpace> GetAllMyNetworkSpaces(int playerIndex)
  {
    List<NetworkSpace> myNetworkSpaces = new();

    List<NetworkSpace> allCurrentNetworkSpaces;
    if (GameManager.currentEra == ERA.BOAT) allCurrentNetworkSpaces = allNetworkSpacesBoat;
    else allCurrentNetworkSpaces = allNetworkSpacesTrain;

    foreach (NetworkSpace networkSpace in allCurrentNetworkSpaces)
    {
      if (networkSpace.IsOccupied() && networkSpace.GetVehicle().ownerPlayerIndex == playerIndex)
      {
        myNetworkSpaces.Add(networkSpace);
      }
    }

    return myNetworkSpaces;
  }
  static public List<LocationScript> GetAllLocationsWithMyTile(int playerIndex)
  {
    List<LocationScript> myLocations = new();

    //Get all locations where there is an industry owned by the player
    foreach (LocationScript location in allIndustryLocations)
    {
      foreach (IndustrySpace space in locationSpacesDict[location])
        if (space.myTile is not null && space.myTile.ownerPlayerIndex == playerIndex)
        {
          if (myLocations.Contains(location)) break;
          myLocations.Add(location);
          break;
        }
    }

    return myLocations;
  }

  static public List<LocationScript> GetAllLocationNeighboringWithMyVehicle(int playerIndex)
  {
    //Get all locations which are connected by the players vehicles
    List<LocationScript> networkLocations = new();

    List<NetworkSpace> allCurrentNetworkSpaces;
    if (GameManager.currentEra == ERA.BOAT) allCurrentNetworkSpaces = allNetworkSpacesBoat;
    else allCurrentNetworkSpaces = allNetworkSpacesTrain;

    foreach (NetworkSpace networkSpace in allCurrentNetworkSpaces)
    {
      if (networkSpace.IsOccupied() && networkSpace.GetVehicle().ownerPlayerIndex == playerIndex)
      {
        foreach (LocationScript location in networkSpace.connectsLocations)
        {
          if (!networkLocations.Contains(location)) networkLocations.Add(location);
        }
      }
    }
    return networkLocations;
  }
  static public List<LocationScript> GetMyNetworkLocations(int playerIndex)
  {

    List<LocationScript> locations = GetAllLocationsWithMyTile(playerIndex);

    List<LocationScript> networkLocations = GetAllLocationNeighboringWithMyVehicle(playerIndex);

    foreach (LocationScript location in networkLocations)
      if (!locations.Contains(location)) locations.Add(location);

    return locations;
  }
  /// <summary>
  /// Tells whether the given space has only one industry that can be built there 
  /// </summary>
  static bool IsIndustrySpacePure(IndustrySpace industrySpace)
  {
    int industriesAmount = 0;
    if (industrySpace.hasBrew) industriesAmount++;
    if (industrySpace.hasCoal) industriesAmount++;
    if (industrySpace.hasCot) industriesAmount++;
    if (industrySpace.hasIron) industriesAmount++;
    if (industrySpace.hasMan) industriesAmount++;
    if (industrySpace.hasPot) industriesAmount++;

    return industriesAmount == 1;

  }
  static bool CanTileBeOverBuilt(TileScript tile, int playerIndex) {
    if (itemInHand is not null)
    {
      if (tile.level >= itemInHand.level) return false;
      if (tile.industryType != itemInHand.industryType) return false;
    }

    else
      if (tile.level >= GetCurrentIndustryLevel(tile.industryType, out bool outOfLimits) || outOfLimits) return false;

    if (!tile.alreadyBuilt) return false;
    if (tile.ownerPlayerIndex == playerIndex) return true; //Overbuild players own building
    if (tile is IronWorksTileScript && GetAllIronWorksWithFreeIron().Count <= 0 && ironStorage.IsEmpty()) return true;//Overbuild other players ironWorks
    if (tile is CoalMineTileScript && GetAllCoalMinesWithFreeCoal().Count <= 0 && coalStorage.IsEmpty()) return true;//Overbuild other players coalMine

    return false;
  }
  /// <summary>
  /// Gets all industry space which are part of given players network but don't have any tile built on them yet
  /// </summary>
  static public List<IndustrySpace> GetMyNetworkFreeSpaces(int playerIndex)
  {
    List<LocationScript> myNetworkLocations = GetMyNetworkLocations(playerIndex);
    if (myNetworkLocations.Count <= 0)
      myNetworkLocations = allIndustryLocations;

    List<IndustrySpace> myNetworkFreeSpaces = new();

    foreach (LocationScript location in myNetworkLocations)
    {
      if (location.myType == LocationType.MERCHANT) continue;

      bool occupiedLocation = false; //For detecting location with already built industry by this player out of boat era
      List<IndustrySpace> curFreeSpaces = new();
      foreach (IndustrySpace space in locationSpacesDict[location])
      {
        if (space.myTile is not null)
        {
          TileScript tile = space.myTile;
          if (GameManager.currentEra == ERA.BOAT)
          {
            if (tile.ownerPlayerIndex == playerIndex)
              occupiedLocation = true;
          }
        }

        else
          curFreeSpaces.Add(space);
      }

      if (occupiedLocation) continue;

      else
        foreach (IndustrySpace curSpace in curFreeSpaces)
          myNetworkFreeSpaces.Add(curSpace);
    }
    return myNetworkFreeSpaces;
  }
  /// <summary>
  /// Gets all industry space which are part of given players network but don't
  /// have any tile built on them yet and where can be built the currently chosen tile
  /// </summary>
  static public List<IndustrySpace> GetMyNetworkFreeSpacesForItemInHand(int playerIndex)
  {
    List<LocationScript> myNetworkLocations = GetMyNetworkLocations(playerIndex);
    if (myNetworkLocations.Count <= 0)
      myNetworkLocations = allIndustryLocations;

    List<IndustrySpace> myNetworkFreeSpaces = new();

    foreach (LocationScript location in myNetworkLocations)
    {
      if (location.myType == LocationType.MERCHANT) continue;

      bool occupiedLocation = false; //For detecting location with already built industry by this player out of boat era
      List<IndustrySpace> curFreeSpaces = new();
      foreach (IndustrySpace space in locationSpacesDict[location])
      {
        if (space.myTile is not null)
        {
          TileScript tile = space.myTile;
          if (GameManager.currentEra == ERA.BOAT)
          {
            if (tile.ownerPlayerIndex == playerIndex)
              occupiedLocation = true;
          }
          if (CanTileBeOverBuilt(tile, playerIndex))
          {
            DisableCollider(tile);
            myNetworkFreeSpaces.Add(space);
          }
        }

        else if (space.CanBuildIndustryType(itemInHand.industryType))
          curFreeSpaces.Add(space);
      }

      if (occupiedLocation) continue;

      if (curFreeSpaces.Count > 1) //Check if there is pure industry space -> add only pure ones
      {
        List<IndustrySpace> pureSpaces = new();
        foreach (IndustrySpace space in curFreeSpaces)
          if (IsIndustrySpacePure(space)) pureSpaces.Add(space);

        if (pureSpaces.Count > 0)
          foreach (IndustrySpace space in pureSpaces)
            myNetworkFreeSpaces.Add(space);
        else
          foreach (IndustrySpace space in curFreeSpaces)
            myNetworkFreeSpaces.Add(space);
      }
      else if (curFreeSpaces.Count == 1) myNetworkFreeSpaces.Add(curFreeSpaces[0]);
    }
    return myNetworkFreeSpaces;
  }
  /// <summary>
  /// Gets all networkSpaces which are neighboring with given players network
  /// </summary>
  static public List<NetworkSpace> GetMyNetworkNeighborConnections(int playerIndex)
  {
    List<LocationScript> myNetworkLocations = GetMyNetworkLocations(playerIndex);

    List<NetworkSpace> myNetworkSpaces = new();

    List<NetworkSpace>[] allNetworkSpacesBothEras = { allNetworkSpacesBoat, allNetworkSpacesTrain };
    int eraIndex = 0;
    if (GameManager.currentEra == ERA.TRAIN) eraIndex = 1;


    //If there aren't any built networks for this player
    if (myNetworkLocations.Count <= 0)
    {
      foreach (NetworkSpace space in allNetworkSpacesBothEras[eraIndex])
        if (!space.IsOccupied())
        {
          if (networkReqBarrel)
          {
            bool barrelAvailable = false;
            foreach (LocationScript loc in space.connectsLocations)
            {
              if (GetAllSpacesWithAvailableBarrels(loc).Count > 0)//If it is the second train - check whether there is a connected barrel available
              {
                barrelAvailable = true;
                break;
              }
            }
            if (barrelAvailable)
              myNetworkSpaces.Add(space);
          }

          else myNetworkSpaces.Add(space);
        }
      return myNetworkSpaces;
    }


    //Already has network
    foreach (NetworkSpace space in allNetworkSpacesBothEras[eraIndex])
    {
      if (space.IsOccupied()) continue;
      foreach (LocationScript location in space.connectsLocations)
      {
        if (myNetworkLocations.Contains(location))
        {
          if (networkReqBarrel)
          {
            bool barrelAvailable = false;
            foreach (LocationScript loc in space.connectsLocations)
            {
              if (GetAllSpacesWithAvailableBarrels(loc).Count > 0)//If it is the second train - check whether there is a connected barrel available
              {
                barrelAvailable = true;
                break;
              }
            }
            if (barrelAvailable)
            {
              myNetworkSpaces.Add(space);
              break;
            }
          }

          else
          {
            myNetworkSpaces.Add(space);
            break;
          }
        }
      }
    }


    return myNetworkSpaces;
  }

  static public bool IsLocationPartOfMyNetwork(LocationScript location, int playerIndex)
  {
    if (location.myType == LocationType.CITY)
      foreach (IndustrySpace space in locationSpacesDict[location])
        if (space.myTile is not null && space.myTile.ownerPlayerIndex == playerIndex) return true;

    List<NetworkSpace> neighborNetwork;
    if (GameManager.currentEra == ERA.BOAT) neighborNetwork = locationNetworkSpacesDictBoat[location];
    else neighborNetwork = locationNetworkSpacesDictTrain[location];

    foreach (NetworkSpace network in neighborNetwork)
      if (network.IsOccupied() && network.GetVehicle().ownerPlayerIndex == playerIndex) return true;

    return false;
  }

  static public bool IsLocationConnectedToMyNetwork(LocationScript location, int playerIndex)
  {
    Queue<LocationScript> locationsQueue = new();
    locationsQueue.Enqueue(location);
    List<LocationScript> locationsVisited = new();

    Dictionary<LocationScript, List<NetworkSpace>> correctLocNetDict;
    if (GameManager.currentEra == ERA.BOAT) correctLocNetDict = locationNetworkSpacesDictBoat;
    else correctLocNetDict = locationNetworkSpacesDictTrain;

    while (locationsQueue.Count > 0)
    {
      List<LocationScript> neighbors;

      LocationScript curLocation = locationsQueue.Dequeue();

      if (GameManager.currentEra == ERA.BOAT) neighbors = curLocation.neighborsBoats;
      else neighbors = curLocation.neighborsTrains;
      foreach (LocationScript neighbor in neighbors)
      {
        if (!AreLocationsConnectedDirectly(curLocation, neighbor)) continue;

        if (locationsQueue.Contains(neighbor) || locationsVisited.Contains(neighbor)) continue;
        //Debug.Log("Neighbor is " + neighbor);

        if (neighbor.myType == LocationType.MERCHANT) continue;

        foreach(IndustrySpace space in locationSpacesDict[neighbor])
        {
          if (space.myTile is not null && space.myTile.ownerPlayerIndex == playerIndex)
            return true;
        }
        foreach(NetworkSpace netSpace in correctLocNetDict[neighbor])
        {
          if (netSpace.IsOccupied() && netSpace.GetVehicle().ownerPlayerIndex == playerIndex)
            return true;
        }


        locationsQueue.Enqueue(neighbor);
      }

      locationsVisited.Add(curLocation);
    }

    return false;
  }
  /// <summary>
  /// Tells whether the given locations are neighbors and networkSpace between them is occupied
  /// </summary>
  static bool AreLocationsConnectedDirectly(LocationScript loc1, LocationScript loc2)
  {
    Dictionary<LocationScript, List<NetworkSpace>> correctDict;
    if (GameManager.currentEra == ERA.BOAT) correctDict = locationNetworkSpacesDictBoat;
    else correctDict = locationNetworkSpacesDictTrain;
    foreach (NetworkSpace networkSpace in correctDict[loc1])
      if (networkSpace.IsOccupied() && networkSpace.connectsLocations.Contains(loc2)) return true;

    return false;
  }

  static public LocationScript GetNearestUnconnectedMerchant(NetworkSpace networkSpace, out int distance)
  {
    //DFS
    List<MerchantTileSpace> connectedMerchants = GetAllConnectedMerchantTiles(networkSpace);

    Queue<(LocationScript, int)> locationsQueue = new();
    foreach(LocationScript location in networkSpace.connectsLocations)
      locationsQueue.Enqueue((location,0));
    List<LocationScript> locationsVisited = new();

    while (locationsQueue.Count > 0)
    {
      List<LocationScript> neighbors;

      (LocationScript,int) curTuple = locationsQueue.Dequeue();

      LocationScript curLocation = curTuple.Item1;
      int curDistance = curTuple.Item2;

      if (locationsVisited.Contains(curLocation)) continue; //Skip if already visited
      
      if (GameManager.currentEra == ERA.BOAT) neighbors = curLocation.neighborsBoats;
      else neighbors = curLocation.neighborsTrains;
      foreach (LocationScript neighbor in neighbors)
      {

        if (locationsVisited.Contains(neighbor)) continue;
        //Debug.Log("Neighbor is " + neighbor);
        if (neighbor.myType == LocationType.MERCHANT)
        {
          bool connected = false;
          foreach (MerchantTileSpace tile in neighbor.myMerchants)
          {
            if (connectedMerchants.Contains(tile)) connected = true;
          }
          if (!connected)
          {
            distance = curDistance;
            return neighbor;
          }
        }
        else locationsQueue.Enqueue((neighbor, curDistance+1));
      }

      locationsVisited.Add(curLocation);
    }

    distance = int.MaxValue;
    return null;
  }
  static public List<MerchantTileSpace> GetAllConnectedMerchantTiles(LocationScript location)
  {
    //DFS
    Queue<LocationScript> locationsQueue = new();
    locationsQueue.Enqueue(location);
    List<LocationScript> locationsVisited = new();
    List<MerchantTileSpace> connectedMerchants = new();

    while (locationsQueue.Count > 0)
    {
      List<LocationScript> neighbors;

      LocationScript curLocation = locationsQueue.Dequeue();

      if (GameManager.currentEra == ERA.BOAT) neighbors = curLocation.neighborsBoats;
      else neighbors = curLocation.neighborsTrains;
      foreach (LocationScript neighbor in neighbors)
      {
        if (!AreLocationsConnectedDirectly(curLocation, neighbor)) continue;

        if (locationsQueue.Contains(neighbor) || locationsVisited.Contains(neighbor)) continue;
        //Debug.Log("Neighbor is " + neighbor);
        if (neighbor.myType == LocationType.MERCHANT)
        {
          foreach (MerchantTileSpace tile in neighbor.myMerchants)
          {
            connectedMerchants.Add(tile);
          }
        }
        else locationsQueue.Enqueue(neighbor);
      }

      locationsVisited.Add(curLocation);
    }

    return connectedMerchants;
  }
  static public List<MerchantTileSpace> GetAllConnectedMerchantTiles(NetworkSpace space)
  {
    List<MerchantTileSpace> returnList = new();
    foreach (LocationScript loc in space.connectsLocations)
      foreach (MerchantTileSpace merch in GetAllConnectedMerchantTiles(loc))
        returnList.Add(merch);

    return returnList;
  }
  static public List<MerchantTileSpace> GetAllConnectedMerchantTiles(LocationScript location, INDUSTRY_TYPE wantedType)
  {
    List<MerchantTileSpace> allMerchants = GetAllConnectedMerchantTiles(location);
    List<MerchantTileSpace> correctMerchants = new();
    foreach (MerchantTileSpace merchant in allMerchants)
    {
      switch (wantedType)
      {
        case INDUSTRY_TYPE.COTTONMILL:
          if (merchant.myTile.hasCot) correctMerchants.Add(merchant);
          break;
        case INDUSTRY_TYPE.MANUFACTURER:
          if (merchant.myTile.hasMan) correctMerchants.Add(merchant);
          break;
        case INDUSTRY_TYPE.POTTERY:
          if (merchant.myTile.hasPottery) correctMerchants.Add(merchant);
          break;
        default:
          break;
      }
    }
    return correctMerchants;
  }
  static public List<(LocationScript, int)> GetAllConnectedLocationsDistances(LocationScript location)
  {
    //DFS
    List<(LocationScript, int)> finalList = new();

    Queue<LocationScript> locationsQueue = new();
    Queue<int> distancesQueue = new();
    locationsQueue.Enqueue(location);
    distancesQueue.Enqueue(0);
    List<LocationScript> locationsVisited = new();
    List<int> distancesVisited = new();
    while (locationsQueue.Count > 0)
    {
      List<LocationScript> neighbors;

      LocationScript curLocation = locationsQueue.Dequeue();
      int curDistance = distancesQueue.Dequeue();

      if (GameManager.currentEra == ERA.BOAT) neighbors = curLocation.neighborsBoats;
      else neighbors = curLocation.neighborsTrains;

      foreach (LocationScript neighbor in neighbors)
      {
        if (!AreLocationsConnectedDirectly(curLocation, neighbor)) continue;

        if (locationsQueue.Contains(neighbor) || locationsVisited.Contains(neighbor)) continue;
        //Debug.Log("Neighbor is " + neighbor);
        locationsQueue.Enqueue(neighbor);
        distancesQueue.Enqueue(curDistance + 1);
      }

      locationsVisited.Add(curLocation);
      distancesVisited.Add(curDistance);
      finalList.Add((curLocation, curDistance));
    }

    return finalList;

  }

  static public List<IronWorksTileScript> GetAllIronWorksWithFreeIron()
  {
    UpdateFreeIronTiles();

    return currentTilesWithFreeIron;
  }
  static public void UpdateFreeIronTiles()
  {
    List<IronWorksTileScript> finalTiles = new();
    foreach (List<TileScript> ironWorksPlayer in allIronWorks)
      foreach (TileScript tile in ironWorksPlayer)
      {
        if (tile is not IronWorksTileScript)
        {
          Debug.LogError("Detected non-IronWorks tile in ironWorks list");
          continue;
        }
        IronWorksTileScript ironWorks = (IronWorksTileScript)tile;
        if (ironWorks.HasIron()) finalTiles.Add(ironWorks);

      }

    currentTilesWithFreeIron = finalTiles;
  }
  static public void HighlightFreeIronSpaces()
  {
    HighlightTiles(currentTilesWithFreeIron);
  }
  static public List<CoalMineTileScript> GetNearestCoalMinesWithFreeCoal(LocationScript location)
  {
    UpdateNearestFreeCoalTiles(location);

    return currentNearestTilesWithFreeCoal;
  }
  static public List<CoalMineTileScript> GetNearestCoalMinesWithFreeCoal(NetworkSpace networkSpace)
  {
    UpdateNearestFreeCoalTiles(networkSpace);

    return currentNearestTilesWithFreeCoal;
  }
  static public List<CoalMineTileScript> GetAllCoalMinesWithFreeCoal()
  {
    List<CoalMineTileScript> finalTiles = new();
    foreach (List<TileScript> coalMinesPlayer in allCoalMines)
      foreach (TileScript tile in coalMinesPlayer)
      {
        if (tile is not CoalMineTileScript)
        {
          Debug.LogError("Detected non-CoalMine tile in coalMines list");
          continue;
        }
        CoalMineTileScript coalMine = (CoalMineTileScript)tile;
        if (coalMine.HasCoal()) finalTiles.Add(coalMine);

      }
    return finalTiles;
  }
  static public List<(CoalMineTileScript, int)> GetAllConnectedUnturnedCoalMinesDistances(LocationScript location)
  {
    List<(CoalMineTileScript, int)> allConnectedCoalMinesDistances = new();
    foreach ((LocationScript, int) locationDistance in GetAllConnectedLocationsDistances(location))
    {
      LocationScript curLocation = locationDistance.Item1;
      int curDistance = locationDistance.Item2;

      if (curLocation.myType == LocationType.MERCHANT) continue;
      foreach (IndustrySpace space in GetAllIndustrySpacesInLocation(curLocation))
      {
        if (space.myTile is CoalMineTileScript && ((CoalMineTileScript)space.myTile).HasCoal())
          allConnectedCoalMinesDistances.Add(((CoalMineTileScript)space.myTile, curDistance));
      }
    }

    return allConnectedCoalMinesDistances;
  }
  static public void UpdateNearestFreeCoalTiles(LocationScript location)
  {
    List<CoalMineTileScript> finalTiles = new();
    List<(CoalMineTileScript, int)> allConnectedCoalMinesDistances = GetAllConnectedUnturnedCoalMinesDistances(location);
    int minDistance = int.MaxValue;

    foreach((CoalMineTileScript,int) coalMineDistance in allConnectedCoalMinesDistances)
      if (coalMineDistance.Item2 < minDistance) minDistance = coalMineDistance.Item2;

    foreach ((CoalMineTileScript, int) coalMineDistance in allConnectedCoalMinesDistances)
      if (coalMineDistance.Item2 == minDistance) finalTiles.Add(coalMineDistance.Item1);

    currentNearestTilesWithFreeCoal = finalTiles;
  }

  static public void UpdateNearestFreeCoalTiles(NetworkSpace networkSpace)
  {
    List<CoalMineTileScript> finalTiles = new();
    List<List<(CoalMineTileScript, int)>> allConnectedCoalMinesDistances = new();

    foreach (LocationScript location in networkSpace.connectsLocations)
    {
      allConnectedCoalMinesDistances.Add(GetAllConnectedUnturnedCoalMinesDistances(location));
    }

    int minDistance = int.MaxValue;
    foreach (List<(CoalMineTileScript, int)> mineDistances in allConnectedCoalMinesDistances)
      foreach ((CoalMineTileScript, int) coalMineDistance in mineDistances)
        if (coalMineDistance.Item2 < minDistance) minDistance = coalMineDistance.Item2;

    foreach (List<(CoalMineTileScript, int)> mineDistances in allConnectedCoalMinesDistances)
      foreach ((CoalMineTileScript, int) coalMineDistance in mineDistances)
        if (coalMineDistance.Item2 == minDistance && !finalTiles.Contains(coalMineDistance.Item1)) finalTiles.Add(coalMineDistance.Item1);

    currentNearestTilesWithFreeCoal = finalTiles;
  }

  static public void HighlightNearestFreeCoalSpaces()
  {
    HighlightTiles(currentNearestTilesWithFreeCoal);
  }
  static public void HighlightIronStorage()
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.ironStorageBorderPath);

    var newBorder = Instantiate(borderResource) as GameObject;
    newBorder.transform.position = ironStorage.transform.position;
    allTileBorders.Add(newBorder);
  }

  static public void HighlightCoalStorage()
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.coalStorageBorderPath);

    var newBorder = Instantiate(borderResource) as GameObject;
    newBorder.transform.position = coalStorage.transform.position;
    allTileBorders.Add(newBorder);
  }
  static public void MakeIronStorageClickable() => ironStorage.BecomeClickable();
  static public void MakeIronStorageUnClickable() => ironStorage.BecomeUnclickable();
  static public void MakeCoalStorageClickable() => coalStorage.BecomeClickable();
  static public void MakeCoalStorageUnClickable() => coalStorage.BecomeUnclickable();

  static public List<BreweryTileScript> GetAllBreweriesWithFreeBarrels()
  {
    List<BreweryTileScript> finalTiles = new();
    foreach (List<TileScript> breweriesPerPlayer in allBreweries)
      foreach (TileScript tile in breweriesPerPlayer)
      {
        if (tile is not BreweryTileScript)
        {
          Debug.LogError("Detected non-Brewery tile in coalMines list");
          continue;
        }
        BreweryTileScript brewery = (BreweryTileScript)tile;
        if (brewery.HasBarrel()) finalTiles.Add(brewery);

      }
    return finalTiles;
  }
  static public List<IndustrySpace> GetAllSpacesWithAvailableBarrels(LocationScript curLocation)
  {
    List<LocationScript> connectedLocations = GetAllConnectedLocations(curLocation);
    List<IndustrySpace> availableSpaceWithBarrels = new();
    foreach(LocationScript loc in connectedLocations)
      foreach(Transform spaceTrans in loc.transform)
      {
        if(!spaceTrans.TryGetComponent(out IndustrySpace space)) continue;
        if (space.myTile is BreweryTileScript && ((BreweryTileScript)space.myTile).HasBarrel())
          availableSpaceWithBarrels.Add(space);

      }

    foreach(BreweryTileScript brewery in allBreweries[GameManager.activePlayerIndex])
    {
      if (brewery.alreadyBuilt && brewery.HasBarrel())
        if(!availableSpaceWithBarrels.Contains(brewery.builtOnSpace))
          availableSpaceWithBarrels.Add(brewery.builtOnSpace);
    }


    return availableSpaceWithBarrels;
  }

  static public void FillBarrelsForSell(LocationScript location, TileScript tile)
  {
    List<BarrelSpace> merchantBarrels = new();
    foreach (MerchantTileSpace merchant in GetAllConnectedMerchantTiles(location, tile.industryType))
      if (merchant.myBarrelSpace.hasBarrel)
        merchantBarrels.Add(merchant.myBarrelSpace);

    currentMerchantBarrelsForSell = merchantBarrels;

    List<IndustrySpace> industrySpacesWithBarrels = GetAllSpacesWithAvailableBarrels(location);

    currentSpacesWithBarrelsForSell = industrySpacesWithBarrels;
  }
  static public List<BarrelSpace> GetAllAvailableMerchantBarrels(LocationScript location, TileScript tile)
  {
    FillBarrelsForSell(location, tile);
    return currentMerchantBarrelsForSell;
  }

  static public void ClearBarrelsForSell()
  {
    currentSpacesWithBarrelsForSell.Clear();
  }

  static public void HighlightBarrelsForSell()
  {
    //Debug.Log("Highlight barrels called");
    Object borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);
    foreach (IndustrySpace barrelSpace in currentSpacesWithBarrelsForSell)
    {
      GameObject border = Instantiate(borderResource) as GameObject;
      border.transform.SetLocalPositionAndRotation(barrelSpace.transform.position, barrelSpace.transform.rotation);
      allSpaceBorders.Add(border);
    }

    borderResource = HelpFunctions.LoadPrefabFromFile(Constants.barrelBorderPath);
    foreach (BarrelSpace barrelSpace in currentMerchantBarrelsForSell)
    {
      //Debug.Log("Creating barrel border");
      GameObject border = Instantiate(borderResource) as GameObject;
      border.transform.SetLocalPositionAndRotation(barrelSpace.transform.position, barrelSpace.transform.rotation);
      allSpaceBorders.Add(border);
    }
  }

  static public void FillCurrentSpacesWithBarrelsForNetwork(NetworkSpace space)
  {
    List<IndustrySpace> spacesWithBarrels = new();
    foreach (LocationScript loc in space.connectsLocations)
    {
      foreach (IndustrySpace indSpace in GetAllSpacesWithAvailableBarrels(loc))
        if (!spacesWithBarrels.Contains(indSpace)) spacesWithBarrels.Add(indSpace);
    }

    currentSpacesWithBarrelsForNetwork = spacesWithBarrels;
  }
  static public List<IndustrySpace> GetAllSpacesWithAvailableBarrels(NetworkSpace space)
  {
    FillCurrentSpacesWithBarrelsForNetwork(space);
    return currentSpacesWithBarrelsForNetwork;
  }
  static public void ClearCurrentSpacesWithBarrelsForNetwork()
  {
    currentSpacesWithBarrelsForNetwork.Clear();
  }

  static public void HighlightCurrentSpacesWithBarrelsForNetwork()
  {
    Object borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);
    foreach (IndustrySpace barrelSpace in currentSpacesWithBarrelsForNetwork)
    {
      GameObject border = Instantiate(borderResource) as GameObject;
      border.transform.SetLocalPositionAndRotation(barrelSpace.transform.position, barrelSpace.transform.rotation);
      allSpaceBorders.Add(border);
    }
  }
  static public List<LocationScript> GetAllConnectedLocations(LocationScript location)
  {
    Queue<LocationScript> locationsQueue = new();
    locationsQueue.Enqueue(location);
    List<LocationScript> locationsVisited = new();

    while (locationsQueue.Count > 0)
    {
      List<LocationScript> neighbors;

      LocationScript curLocation = locationsQueue.Dequeue();

      if (GameManager.currentEra == ERA.BOAT) neighbors = curLocation.neighborsBoats;
      else neighbors = curLocation.neighborsTrains;
      foreach (LocationScript neighbor in neighbors)
      {
        if (!AreLocationsConnectedDirectly(curLocation, neighbor)) continue;

        if (locationsQueue.Contains(neighbor) || locationsVisited.Contains(neighbor)) continue;
        //Debug.Log("Neighbor is " + neighbor);
        locationsQueue.Enqueue(neighbor);
      }

      locationsVisited.Add(curLocation);
    }

    return locationsVisited;
  }

  static public void HighlightAllConnectedLocations(LocationScript location)
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.tileBorderPath);

    //Debug.Log("Highlight connected locations called!");
    //Debug.Log(location);

    List<LocationScript> connectedLocations = GetAllConnectedLocations(location);
    foreach (LocationScript loc in connectedLocations)
      foreach (Transform space in loc.transform)
      {
        var newBorder = Instantiate(borderResource) as GameObject;
        newBorder.transform.position = space.transform.position;
      }
  }

  static void DisableCollider(TileScript tile)
  {
    tile.GetComponent<BoxCollider2D>().enabled = false;
    tilesWithDisabledColliders.Add(tile);
  }

  static public void RestoreColliders()
  {
    foreach (TileScript tile in tilesWithDisabledColliders)
      tile.GetComponent<BoxCollider2D>().enabled = true;
    tilesWithDisabledColliders.Clear();
  }

  static public void RemoveAllBuiltBoatTiles()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      foreach (TileScript tile in GetAllMyBuiltTiles(i))
        if (tile.level <= 1)
        {
          if (tile.builtOnSpace is not null)
            tile.builtOnSpace.RemoveBuiltIndustry();
          tile.Remove();
        }
    }
  }
  static public void DestroyAllBorders()
  {
    foreach (List<GameObject> borders in allBorders)
    {
      foreach (GameObject border in borders)
        border.SetActive(false);


      //Debug.Log("Destroying border");
      borders.Clear();
    }
  }
  static public void DestroyAllTileBorders()
  {
    foreach (GameObject border in allTileBorders)
    {
      border.SetActive(false);
      //Debug.Log("Destroying border");
    }

    allTileBorders.Clear();
  }
  static public void DestroyAllSpaceBorders()
  {
    foreach (GameObject border in allSpaceBorders)
    {
      border.SetActive(false);
      //Debug.Log("Destroying border");
    }

    allSpaceBorders.Clear();
  }
  static public void DestroyAllNetworkBorders()
  {
    foreach (GameObject border in allNetworkBorders)
    {
      border.SetActive(false);
      //Debug.Log("Destroying border");
    }

    allNetworkBorders.Clear();
  }


  public void PopulateSaveData(SaveData sd)
  {
    PopulateSaveDataStatic(sd);
  }
  public static void PopulateSaveDataStatic(SaveData sd)
  {
    foreach(List<TileScript>[] tilesPerPlayer in allTilesPerPlayer)
      foreach(List<TileScript> tiles in tilesPerPlayer)
        foreach(TileScript tile in tiles)
          tile.PopulateSaveData(sd);

    foreach (NetworkSpace space in allNetworkSpacesBoat)
      space.PopulateSaveData(sd);

    foreach (NetworkSpace space in allNetworkSpacesTrain)
      space.PopulateSaveData(sd);

    foreach (LocationScript merchant in allMerchants)
      foreach (MerchantTileSpace space in merchant.myMerchants)
        space.PopulateSaveData(sd);

    coalStorage.PopulateSaveData(sd);
    ironStorage.PopulateSaveData(sd);

    SaveData.ObjectManagerData myData = new();

    myData.firstUnbuiltBreweryIndeces = new();
    foreach (int index in firstUnbuiltBreweryIndeces)
      myData.firstUnbuiltBreweryIndeces.Add(index);
   
    myData.firstUnbuiltCoalMineIndeces = new();
    foreach (int index in firstUnbuiltCoalMineIndeces)
      myData.firstUnbuiltCoalMineIndeces.Add(index);
   
    myData.firstUnbuiltCottonMillIndeces = new();
    foreach (int index in firstUnbuiltCottonMillIndeces)
      myData.firstUnbuiltCottonMillIndeces.Add(index);
  
    myData.firstUnbuiltIronWorksIndeces = new();
    foreach (int index in firstUnbuiltIronWorksIndeces)
      myData.firstUnbuiltIronWorksIndeces.Add(index);
  
    myData.firstUnbuiltManufacturerIndeces = new();
    foreach (int index in firstUnbuiltManufacturerIndeces)
      myData.firstUnbuiltManufacturerIndeces.Add(index);
   
    myData.firstUnbuiltPotteryIndeces = new();
    foreach (int index in firstUnbuiltPotteryIndeces)
      myData.firstUnbuiltPotteryIndeces.Add(index);

    sd.objectManagerData = myData;
  }

  public void LoadFromSaveData(SaveData sd)
  {
    LoadFromSaveDataStatic(sd);
  }

  public static void LoadFromSaveDataStatic(SaveData sd)
  {
    DestroyAllObjects();
    ClearAllLocalMemory();
    LoadAllStaticObjects();
    InitializeObjects();

    

    foreach (List<TileScript>[] tilesPerPlayer in allTilesPerPlayer)
      foreach (List<TileScript> tiles in tilesPerPlayer)
        foreach (TileScript tile in tiles)
          tile.LoadFromSaveData(sd);

    foreach (NetworkSpace space in allNetworkSpacesBoat)
      space.LoadFromSaveData(sd);

    foreach (NetworkSpace space in allNetworkSpacesTrain)
      space.LoadFromSaveData(sd);

    foreach (LocationScript merchant in allMerchants)
      foreach (MerchantTileSpace space in merchant.myMerchants)
        space.LoadFromSaveData(sd);

    coalStorage.LoadFromSaveData(sd);
    ironStorage.LoadFromSaveData(sd);

    SaveData.ObjectManagerData myData = sd.objectManagerData;

    firstUnbuiltBreweryIndeces.Clear();
    foreach (int index in myData.firstUnbuiltBreweryIndeces)
      firstUnbuiltBreweryIndeces.Add(index);

    firstUnbuiltCoalMineIndeces.Clear();
    foreach (int index in myData.firstUnbuiltCoalMineIndeces)
      firstUnbuiltCoalMineIndeces.Add(index);

    firstUnbuiltCottonMillIndeces.Clear();
    foreach (int index in myData.firstUnbuiltCottonMillIndeces)
      firstUnbuiltCottonMillIndeces.Add(index);

    firstUnbuiltIronWorksIndeces.Clear();
    foreach (int index in myData.firstUnbuiltIronWorksIndeces)
      firstUnbuiltIronWorksIndeces.Add(index);

    firstUnbuiltManufacturerIndeces.Clear();
    foreach (int index in myData.firstUnbuiltManufacturerIndeces)
      firstUnbuiltManufacturerIndeces.Add(index);

    firstUnbuiltPotteryIndeces.Clear();
    foreach (int index in myData.firstUnbuiltPotteryIndeces)
      firstUnbuiltPotteryIndeces.Add(index);

  }
}
