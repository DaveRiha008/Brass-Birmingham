using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
  public static GameManager instance;

  public static BOARD currentlyOnBoard = BOARD.NONE;

  public static ERA currentEra = ERA.BOAT;

  static List<Player> allPlayersList = new List<Player> { new Player(0, "Player 1"), new Player(1, "Player 2"),
    new Player(2, "Player 3"), new Player(3, "Player 4") };
  static List<Player> playerList = new List<Player>() { new Player(0, "Player 1"), new Player(1, "Player 2"),
    new Player(2, "Player 3"), new Player(3, "Player 4") };

  public static List<int> playerTurns = new List<int> { 0, 1, 2, 3 };

  public static readonly List<Player> playerWinningOrder = new() { new Player(0, "Player 1"), new Player(1, "Player 2"),
    new Player(2, "Player 3"), new Player(3, "Player 4") };

  static List<int> incomeValues = new();

  public static int activePlayerIndex = 0;
  public static int activePlayerTurnIndex = 0;
  static public int numOfPlayers = 4;
  static public int numOfAI = 2;

  static int minPlayers = 2;
  static int maxPlayers = 4;

  static int randomSeed = 100;

  static public bool firstEverRound = true;
  static bool gameRunning = false;
  static public bool waitingForNextEra = false;

  void Start()
  {
    LoadIncomeLevels();
    StartGame();
  }

  private void Awake()
  {
    CreateSingleton();
  }

  private void Update()
  {
    UpdateWinningOrderPlayers();
  }
  void CreateSingleton()
  {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);


    DontDestroyOnLoad(gameObject);
  }

  public static void StartGame()
  {
    //Debug.Log($"StartGame called - current era: {currentEra}");

    //to ensure always the same order of players in the case of repeated games
    activePlayerIndex = 0;
    activePlayerTurnIndex = 0;


    gameRunning = true;

    firstEverRound = true;

    //SetSeedOfGame(randomSeed); //Uncomment for deterministic always identical game
    
    UpdateAIReplacement();

    InitPlayerInfo();

    SortPlayerTurnsByMoneySpent();

    UpdateActivePlayerPropertiesBeforeAction();
    
    //ObjectManager.InitializeObjects();
    //CardManager.InitializeCards();
    if (playerList[activePlayerIndex].AIReplaced) AIManager.PlayTurn();
  }
  public static void EndGame()
  {
    gameRunning = false;

    ActionManager.CancelAction();

    // For AI analysis
    VictoryDataManager.UpdateVictoryData();

    AIManager.StopPlaying();

    //Debug.Log($"End Game called - setting currentEra from {currentEra} to BOAT");
    currentEra = ERA.BOAT;
    ObjectManager.DestroyAllObjects();
    CardManager.DestroyAllCards();
    SceneManager.LoadScene("VictoryScreen");
    if (Constants.instantGameRestart)
    {
      SceneManager.LoadScene("Game");
      StartGame();
    }
  }

  public static bool GameRunning() => gameRunning;

  static void InitPlayerInfo()
  {
    for (int i = 0; i < numOfPlayers; i++)
    {
      Player player = GetPlayer(i);
      player.money = Constants.initPlayerMoney;
      player.income = Constants.initPlayerIncome;
      player.victoryPoints = Constants.initPlayerVicPts;
    }
  }
  static void LoadIncomeLevels()
  {
    int[] reqIncreaseThresholds = { 10, 30, 60, int.MaxValue };
    int reqIncrIndex = 0;
    int curIncomeVal = -10;
    int curReq = 0;
    int reqIndex = 0;
    incomeValues = new();
    for (int i = 0; i < 100; i++)
    {
      incomeValues.Add(curIncomeVal);
      if (reqIndex >= curReq)
      {
        reqIndex = 0;
        curIncomeVal++;
      }
      else reqIndex++;

      if (i >= reqIncreaseThresholds[reqIncrIndex])
      {
        reqIncrIndex++;
        curReq++;
      }
    }

    //Debug.Log("Loaded Income values: ");
    //foreach (int val in incomeValues)
    //{
    //  Debug.Log("[" + incomeValues.IndexOf(val) + "] = " + val);
    //}

  }

  public static void SetSeedOfGame(int newSeed)
  {
    randomSeed = newSeed;
    Random.InitState(randomSeed);
  }

  public static int GetGameSeed() => randomSeed;

  static void UpdateNumOfPlayersNumbers()
  {
    numOfPlayers = Mathf.Clamp(numOfPlayers, minPlayers, maxPlayers);
    numOfAI = Mathf.Clamp(numOfAI, 0, numOfPlayers - 1);

    playerList.Clear();
    playerTurns.Clear();
    for (int i = 0; i < numOfPlayers; i++)
    {
      playerList.Add(allPlayersList[i]);
      playerTurns.Add(i);
    }
  }

  static public void AddPlayer()
  {
    numOfPlayers++;
    UpdateNumOfPlayersNumbers();
  }
  static public void RemovePlayer()
  {
    numOfPlayers--;
    UpdateNumOfPlayersNumbers();
  }

  static public void AddAI()
  {
    numOfAI++;
    UpdateNumOfPlayersNumbers();
    UpdateAIReplacement();
  }
  static public void RemoveAI()
  {
    numOfAI--;
    UpdateNumOfPlayersNumbers();
    UpdateAIReplacement();
  }

  static void UpdateAIReplacement()
  {
    for (int i = 0; i < numOfPlayers; i++)
    {
      int oppositeIndex = (numOfPlayers - 1) - i; //For the humans to play first
      if (i < numOfAI) playerList[oppositeIndex].AIReplaced = true;
      else playerList[oppositeIndex].AIReplaced = false;
    }
  }
  static public void EndTurn()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToMainBoard();  //Demo of end turn function
    NextPlayer();
  }

  static public void NextPlayer()
  {
    if(ActionManager.currentAction != ACTION.NONE)
      ActionManager.CancelAction();
    if (CardManager.AllPlayersHaveEmptyHands() && CardManager.drawDeck.isEmpty)
      ChangeEra();


    else if (((ActionManager.actionsPlayed < 1 && firstEverRound) || (!firstEverRound && ActionManager.actionsPlayed < Constants.maxActionsPerRound))
      && CardManager.GetPlayerCards(activePlayerIndex).Count > 0 && gameRunning) //Don't discard if no card in hand or game ended
    {
      
      //Debug.Log($"Cant end turn until card discard for all actions - current actions {ActionManager.actionsPlayed}");
      ActionManager.ChooseCardForAction(); //Remove card when there were remaining actions to do
      return;
    }

    CardManager.FillPlayerHand(activePlayerIndex);

    //Switch player
    activePlayerTurnIndex++;
    ActionManager.actionsPlayed = 0;
    if (activePlayerTurnIndex >= numOfPlayers)
    {
      LastPlayerEndTurn();
      activePlayerTurnIndex = 0;
    }
    activePlayerIndex = playerTurns[activePlayerTurnIndex];

    HelpFunctions.HUDInfoShowMessage(INFO_MESSAGE.PLAYER_CHANGED);


    UpdateActivePlayerPropertiesBeforeAction();
    //Debug.Log("Active player changed to: " + activePlayerIndex.ToString());

    if (playerList[activePlayerIndex].AIReplaced) AIManager.PlayTurn();
  }

  static public void LastPlayerEndTurn()
  {
    firstEverRound = false;
    SortPlayerTurnsByMoneySpent();
    AllPlayersGainIncomeMoney();
    foreach (Player player in playerList)
    {
      player.moneySpentThisTurn = 0;
    }
  }

  static public Player GetPlayer(int playerIndex) => playerList[playerIndex];
  static public Player GetActivePlayer() => GetPlayer(activePlayerIndex);
  static public int GetPlayerIncomeLvl(int playerIndex)
  {
    return incomeValues[playerList[playerIndex].income];
  }

  static public void PlayerGainIncome(int playerIndex, int amount = 1)
  {
    if (playerList[playerIndex].income + amount > Constants.maxIncome)
      playerList[playerIndex].income = Constants.maxIncome;
    else
      playerList[playerIndex].income += amount;
  }

  static public void ActivePlayerGainIncome(int amount = 1)
  {
    PlayerGainIncome(activePlayerIndex, amount);
  }

  static public void PlayerGainIncomeLevel(int playerIndex, int amount = 1)
  {
    int originIncomeLvl;
    int newIncome;
    int newIncomeLvl;
    for (int i = 0; i < amount; i++)
    {
      originIncomeLvl = incomeValues[playerList[playerIndex].income];
      newIncome = playerList[playerIndex].income;
      newIncomeLvl = originIncomeLvl;
      while (newIncomeLvl <= originIncomeLvl)
      {
        newIncome++;
        newIncomeLvl = incomeValues[newIncome];
      }
      if(newIncome >= Constants.maxIncome)
      {
        Debug.Log("Income can't be more than maximum");
        playerList[playerIndex].income = Constants.maxIncome;
        return;
      }
      playerList[playerIndex].income = newIncome;
    }
  }
  static public void PlayerLoseIncome(int playerIndex, int amount = 1)
  {
    if (playerList[playerIndex].income < amount)
    {
      playerList[playerIndex].income = 0;
      Debug.LogError("Income can't be less than 0!");
      return;
    }
    playerList[playerIndex].income -= amount;
  }
  static public void PlayerLoseIncomeLevel(int playerIndex, int amount = 1)
  {

    int originIncomeLvl;
    int newIncome;
    int newIncomeLvl;
    for (int i = 0; i < amount; i++)
    {
      originIncomeLvl = incomeValues[playerList[playerIndex].income];
      newIncome = playerList[playerIndex].income;
      newIncomeLvl = originIncomeLvl;
      while (newIncomeLvl >= originIncomeLvl)
      {
        newIncome--;
        newIncomeLvl = incomeValues[newIncome];
      }
      if(newIncome < 0)
      {
        Debug.Log("Cant lose income level when income is already minimum");
        playerList[playerIndex].income = 0;
        return;
      }
      playerList[playerIndex].income = newIncome;
    }
  }

  static public void PlayerGainMoney(int playerIndex, int amount = 1)
  {
    int money = playerList[playerIndex].money;

    if (money + amount < 0)
    {
      //Debug.LogError("Money can't be less than 0!");
      money = 0;
      return;
    }

    playerList[playerIndex].money = money + amount;

  }
  static public void PlayerGainVictoryPoints(int playerIndex, int amount = 1)
  {
    int vicPts = playerList[playerIndex].victoryPoints;

    if (vicPts + amount < 0)
    {
      Debug.LogError("Victory points can't be less than 0!");
      playerList[playerIndex].victoryPoints = 0;
    }
    else if (vicPts + amount > Constants.maxVP)
    {
      Debug.LogError("Victory points can't be more than max!");
      playerList[playerIndex].victoryPoints = Constants.maxVP;
    }
    else
      playerList[playerIndex].victoryPoints = vicPts + amount;
    UpdateWinningOrderPlayers();

  }

  static public void ActivePlayerGainVictoryPoints(int amount)
  {
    PlayerGainVictoryPoints(activePlayerIndex, amount);
  }

  static void UpdateWinningOrderPlayers()
  {
    playerWinningOrder.Clear();
    List<Player> tempPlayers = new();

    foreach (Player player in playerList)
    {
      tempPlayers.Add(player);
    }

    Player curBestPlayer = null;
    int curMax = 0;

    for (int i = 0; i < numOfPlayers; i++)
    {
      foreach (Player player in tempPlayers) 
      {
        if (player.victoryPoints == curMax && curBestPlayer is not null) //If VP is the same go for income difference
        {
          if (player.income > curBestPlayer.income)
            curBestPlayer = player;
          else if (player.income == curBestPlayer.income) //If Income is the same go for money difference
            if (player.money > curBestPlayer.money)
              curBestPlayer = player;
          
        }
        else if (player.victoryPoints >= curMax)
        {

          curMax = player.victoryPoints;
          curBestPlayer = player;
        } 
      }
      playerWinningOrder.Add(curBestPlayer);
      tempPlayers.Remove(curBestPlayer);
      curMax = 0;
      curBestPlayer = null;
    }


  }

  static public void AllPlayersGainIncomeMoney()
  {
    for (int i = 0; i < numOfPlayers; i++)
    {
      Player player = GetPlayer(i);
      int incomeVal = incomeValues[player.income];
      if(incomeVal < 0 && player.money < -incomeVal) PlayerGainVictoryPoints(i, incomeVal+player.money); //Lose VP if not enough money
      PlayerGainMoney(i, incomeVal);
    }
  }

  static public void ChangeEra()
  {
    ActionManager.CancelAction();
    AwardVictoryPoints();

    ObjectManager.DestroyAllNetwork();
    switch (currentEra)
    {
      case ERA.BOAT:
        Debug.Log($"Changing era from BOAT to TRAIN");
        currentEra = ERA.TRAIN;
        ObjectManager.RemoveAllBuiltBoatTiles();
        ObjectManager.FillMerchantBarrels();

        CardManager.PutAllCardsInDrawDecks();
        CardManager.ShuffleDrawDeck();

        for (int i = 0; i < numOfPlayers; i++)
          CardManager.FillPlayerHand(i);


        if(!Constants.instantGameRestart)
        {
          CameraScript camera = Camera.main.GetComponent<CameraScript>();
          camera.MoveToChangeEraScreen();
          camera.lockScreenChange = true;

          waitingForNextEra = true;
        }
        break;
      case ERA.TRAIN:
        Debug.Log($"Changing era from train -> END GAME");
        EndGame();
        break;
      default:
        break;
    }
  }

  static public void NextEraReady()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.lockScreenChange = false;
    camera.MoveToMainBoard();

    waitingForNextEra = false;
  }

  static void AwardVictoryPoints()
  {
    foreach (Player player in playerList)
    {
      AwardVictoryPoints(player.index);
    }
  }

  static void AwardVictoryPoints(int playerIndex)
  {
    //Points for network
    List<NetworkSpace> myNetworkSpaces = ObjectManager.GetAllMyNetworkSpaces(playerIndex);
    foreach (NetworkSpace networkSpace in myNetworkSpaces)
      foreach (LocationScript location in networkSpace.connectsLocations)
        if (location.myType != LocationType.MERCHANT)
          foreach (IndustrySpace industrySpace in ObjectManager.GetAllIndustrySpacesInLocation(location))
          {
            //Debug.Log("IndustrySpace being awarded " + industrySpace.ToString());
            if (industrySpace.myTile is not null && industrySpace.myTile.isUpgraded) PlayerGainVictoryPoints(playerIndex, industrySpace.myTile.upgradeNetworkVicPtsReward);
          }

    //Points for upgraded tiles
    List<TileScript> myTiles = ObjectManager.GetAllMyBuiltTiles(playerIndex);
    foreach (TileScript tile in myTiles)
      if (tile.isUpgraded) PlayerGainVictoryPoints(playerIndex, tile.upgradeVicPtsReward);
  }

  static public void ActivePlayerGainMoney(int amount)
  {
    PlayerGainMoney(activePlayerIndex, amount);
  }

  static public void ActivePlayerSpendMoney(int amount, out bool enoughMoney)
  {
    if (playerList[activePlayerIndex].money < amount)
    {
      //Debug.Log("Not enough money - try to loan some");
      enoughMoney = false;
      return;
    }
    playerList[activePlayerIndex].money -= amount;
    playerList[activePlayerIndex].moneySpentThisTurn += amount;
    enoughMoney = true;
  }

  static public bool ActivePlayerHasEnoughMoney(int wantedAmount) => playerList[activePlayerIndex].money >= wantedAmount;


 

  static public void GainMerchantBarrelReward(MerchantReward reward)
  {
    switch (reward.type)
    {
      case MERCHANT_REW_TYPES.MONEY:
        //Debug.Log("Giving Barrel reward money");
        ActivePlayerGainMoney(reward.amount);
        break;
      case MERCHANT_REW_TYPES.VIC_POINTS:
        ActivePlayerGainVictoryPoints(reward.amount);
        break;
      case MERCHANT_REW_TYPES.INCOME:
        ActivePlayerGainIncome(reward.amount);
        break;
      case MERCHANT_REW_TYPES.DEVELOP:
        ActionManager.developActionsToDo++;
        break;
      case MERCHANT_REW_TYPES.NONE:
        break;
      default:
        break;
    }
  }

  
  static public void SortPlayerTurnsByMoneySpent()
  {
    List<int> indexList = new();
    List<int> moneyList = new();
    for (int i = 0; i < numOfPlayers; i++)
    {
      indexList.Add(playerTurns[i]);
      moneyList.Add(playerList[playerTurns[i]].moneySpentThisTurn);
    }

    List<int> sortedIndexList = new();
    List<int> sortedMoneyList = new();
    for (int i = 0; i < numOfPlayers; i++)
    {
      int moneyMin = int.MaxValue;
      int maxMoneyIndex = 0;
      for (int j = 0; j < moneyList.Count; j++)
      {
        if (moneyList[j] < moneyMin)
        {
          moneyMin = moneyList[j];
          maxMoneyIndex = indexList[j];
        }
      }
      sortedIndexList.Add(maxMoneyIndex);
      sortedMoneyList.Add(moneyMin);
      moneyList.Remove(moneyMin);
      indexList.Remove(maxMoneyIndex);
    }
    playerTurns = sortedIndexList;


  }

  public void PopulateSaveData(SaveData sd)
  {
    PopulateSaveDataStatic(sd);
  }

  static public void PopulateSaveDataStatic(SaveData saveData)
  {
    foreach(Player player in playerList)
    {
      player.PopulateSaveData(saveData);
    }

    SaveData.GameData gameData = new();

    gameData.playerTurns = playerTurns;
    Debug.Log($"GameManager populating playerTurn with {playerTurns}");

    gameData.currentEra = currentEra;
    gameData.activePlayerIndex = activePlayerIndex;
    gameData.activePlayerTurnIndex = activePlayerTurnIndex;
    gameData.numOfPlayers = numOfPlayers;
    gameData.numOfAI = numOfAI;
    gameData.randomSeed = randomSeed;
    gameData.actionPlayed = ActionManager.actionsPlayed;
    gameData.firstEverRound = firstEverRound;

    saveData.gameData = gameData;
  }

  public void LoadFromSaveData(SaveData sd)
  {
    LoadGameDataStatic(sd);
  }
  static public void LoadGameDataStatic(SaveData saveData)
  {
    ActionManager.CancelAction();

    foreach (Player player in playerList)
    {
      player.LoadFromSaveData(saveData);
    }

    playerTurns = saveData.gameData.playerTurns;
    currentEra = saveData.gameData.currentEra;
    activePlayerIndex = saveData.gameData.activePlayerIndex;
    activePlayerTurnIndex = saveData.gameData.activePlayerTurnIndex;
    numOfPlayers = saveData.gameData.numOfPlayers;
    numOfAI = saveData.gameData.numOfAI;
    randomSeed = saveData.gameData.randomSeed;
    ActionManager.actionsPlayed = saveData.gameData.actionPlayed;
    firstEverRound = saveData.gameData.firstEverRound;

    UpdateActivePlayerPropertiesBeforeAction();
    UpdateWinningOrderPlayers();
  }
  static public void ResetActivePlayerToStateBeforeAction()
  {
    GetActivePlayer().victoryPoints = ActionManager.vicPtsBeforeAction;
    GetActivePlayer().income = ActionManager.incomeBeforeAction;
    GetActivePlayer().money = ActionManager.moneyBeforeAction;
    GetActivePlayer().moneySpentThisTurn = ActionManager.moneySpentThisTurnBeforeAction;
  }

  static public void UpdateActivePlayerPropertiesBeforeAction()
  {
    ActionManager.vicPtsBeforeAction = GetActivePlayer().victoryPoints;
    ActionManager.incomeBeforeAction = GetActivePlayer().income;
    ActionManager.moneyBeforeAction = GetActivePlayer().money;
    ActionManager.moneySpentThisTurnBeforeAction = GetActivePlayer().moneySpentThisTurn;
  }
}

public class Player : ISaveable
{
  public int index;

  public bool AIReplaced = false;

  public int victoryPoints = 0;
  public int income = 10;
  public int money = 17;
  public int moneySpentThisTurn = 0;
  public string name = "Player";
  public Player(int index, string name)
  {
    this.index = index;
    this.name = name;
  }

  public void PopulateSaveData(SaveData saveData)
  {
    SaveData.PlayerData playerData = new();
    playerData.index = index;
    //Debug.Log($"Player populating index with {index}");

    playerData.AIReplaced = AIReplaced;

    playerData.victoryPoints = victoryPoints;
    //Debug.Log($"Player populating victoryPoints with {victoryPoints}");
    playerData.income = income;
    //Debug.Log($"Player populating income with {income}");
    playerData.money = money;
    //Debug.Log($"Player populating money with {money}");
    playerData.moneySpentThisTurn = moneySpentThisTurn;
    //Debug.Log($"Player populating moneySpentThisTurn with {moneySpentThisTurn}");
    playerData.name = name;
    //Debug.Log($"Player populating name with {name}");

    saveData.playerData.Add(playerData);
  }

  public void LoadFromSaveData(SaveData saveData)
  {
    foreach (SaveData.PlayerData data in saveData.playerData)
      if (data.index == index)
      {
        //Debug.Log($"Loading player {saveData.playerData[index].name} info");

        AIReplaced = data.AIReplaced;

        victoryPoints = saveData.playerData[index].victoryPoints;
        income = saveData.playerData[index].income;
        money = saveData.playerData[index].money;
        moneySpentThisTurn = saveData.playerData[index].moneySpentThisTurn;
        name = saveData.playerData[index].name;
      }
  }
}

public enum BOARD { MAIN, PERSONAL, HELP, HAND, DISCARD, ERA_CHANGE, NONE};
public enum ERA { BOAT, TRAIN }