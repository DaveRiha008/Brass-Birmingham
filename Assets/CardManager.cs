using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour, ISaveable
{
  public static CardManager instance;

  public static bool canChooseCard = false;
  public static bool canDrawWildCards = false;

  public static List<CardScript> chosenCards = new();

  public static CardDeck drawDeck;
  public static CardDeck wildIndustryDrawDeck;
  public static CardDeck wildLocationDrawDeck;

  private static List<CardScript> allCards = new();
  private static List<CardScript> allLocationCards = new();
  private static List<CardScript> allIndustryCards = new();
  private static List<CardScript> allWildIndustryCards = new();
  private static List<CardScript> allWildLocationCards = new();

  private static List<CardScript> player1Hand = new();
  private static List<CardScript> player2Hand = new();
  private static List<CardScript> player3Hand = new();
  private static List<CardScript> player4Hand = new();
  private static List<CardScript>[] playerHands = { player1Hand, player2Hand, player3Hand, player4Hand };

  private static List<CardScript> player1Discard = new();
  private static List<CardScript> player2Discard = new();
  private static List<CardScript> player3Discard = new();
  private static List<CardScript> player4Discard = new();
  private static List<CardScript>[] playerDiscards= { player1Discard, player2Discard, player3Discard, player4Discard };

  private static bool alreadyCreatedCards = false;

  private static GameObject player1HandBoard;
  private static GameObject player2HandBoard;
  private static GameObject player3HandBoard;
  private static GameObject player4HandBoard;

  private static GameObject player1DiscardBoard;
  private static GameObject player2DiscardBoard;
  private static GameObject player3DiscardBoard;
  private static GameObject player4DiscardBoard;


  private static GameObject[] playersHandBoards;
  private static GameObject[] playersDiscardBoards;

  private static List<GameObject> allBorders = new();


  //private void Awake()
  //{
  //  CreateSingleton();
  //}
  //void CreateSingleton()
  //{
  //  if (instance == null)
  //    instance = this;
  //  else
  //    Destroy(gameObject);


  //  //DontDestroyOnLoad(gameObject);
  //}

  private void Start()
  {

    Debug.Log("CardManager Start called!");

    try
    {
      LoadDrawDecks();
    }
    catch
    {
      Debug.LogError("Failed to find DrawDecks in main board");
    }

    drawDeck.BecomeUnclickable(); //Normal deck can't be clicked - no reason
    try
    {
      LoadHandBoards();
      LoadDiscardBoards();
    }
    catch (System.NullReferenceException e)
    {
      Debug.LogError(e.Message);
    }
    alreadyCreatedCards = false;

    //canChooseCard = false;
    canDrawWildCards = false;
    chosenCards = new();

    allCards = new();
    allLocationCards = new();
    allIndustryCards = new();
    allWildIndustryCards = new();
    allWildLocationCards = new();

    allBorders = new();

    player1Discard = new();
    player2Discard = new();
    player3Discard = new();
    player4Discard = new();

    player1Hand = new();
    player2Hand = new();
    player3Hand = new();
    player4Hand = new();


    playerHands[0] = player1Hand;
    playerHands[1] = player2Hand;
    playerHands[2] = player3Hand;
    playerHands[3] = player4Hand;

    playerDiscards[0] = player1Discard;
    playerDiscards[1] = player2Discard;
    playerDiscards[2] = player3Discard;
    playerDiscards[3] = player4Discard;

    InitializeCards();
  }

  public static void InitializeCards()
  {
    DestroyAllCards();
    DestroyAllBorders();
    CreateAllCards();
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      for (int j = 0; j < 8; j++)
      {
        PlayerDrawCard(i, CardDeckType.NORMAL);
      }
    }
  }

  private static void LoadHandBoards()
  {
    //Debug.Log("Loading HandBoards");
    player1HandBoard = GameObject.Find(Constants.player1HandBoardName);

    if (player1HandBoard is null) throw new System.NullReferenceException("Failed to find Player1HandBoard in the scene");

    player2HandBoard = GameObject.Find(Constants.player2HandBoardName);
    if (player2HandBoard is null) throw new System.NullReferenceException("Failed to find Player2HandBoard in the scene");

    player3HandBoard = GameObject.Find(Constants.player3HandBoardName);
    if (player3HandBoard is null) throw new System.NullReferenceException("Failed to find Player3HandBoard in the scene");

    player4HandBoard = GameObject.Find(Constants.player4HandBoardName);
    if (player4HandBoard is null) throw new System.NullReferenceException("Failed to find Player4HandBoard in the scene");

    playersHandBoards = new GameObject[4] { player1HandBoard, player2HandBoard, player3HandBoard, player4HandBoard };

  }

  private static void LoadDiscardBoards()
  {
    //Debug.Log("Loading DiscardBoards");
    player1DiscardBoard = GameObject.Find(Constants.player1DiscardBoardName);
    if (player1DiscardBoard is null) throw new System.NullReferenceException("Failed to find Player1DiscardBoard in the scene");

    player2DiscardBoard = GameObject.Find(Constants.player2DiscardBoardName);
    if (player2DiscardBoard is null) throw new System.NullReferenceException("Failed to find Player2DiscardBoard in the scene");

    player3DiscardBoard = GameObject.Find(Constants.player3DiscardBoardName);
    if (player3DiscardBoard is null) throw new System.NullReferenceException("Failed to find Player3DiscardBoard in the scene");

    player4DiscardBoard = GameObject.Find(Constants.player4DiscardBoardName);
    if (player4DiscardBoard is null) throw new System.NullReferenceException("Failed to find Player4DiscardBoard in the scene");

    playersDiscardBoards = new GameObject[4] { player1DiscardBoard, player2DiscardBoard, player3DiscardBoard, player4DiscardBoard };

  }

  private static void LoadDrawDecks()
  {
    drawDeck = GameObject.Find(Constants.mainBoardName).transform.Find(Constants.drawDeckInMainBoardName).GetComponent<CardDeck>();
    wildIndustryDrawDeck = GameObject.Find(Constants.mainBoardName).transform.Find(Constants.wildIndustryDrawDeckInMainBoardName).GetComponent<CardDeck>();
    wildLocationDrawDeck = GameObject.Find(Constants.mainBoardName).transform.Find(Constants.wildLocationDrawDeckInMainBoardName).GetComponent<CardDeck>();
  }

  public static void CreateAllCards()
  {
    if (alreadyCreatedCards) return;

    int cardId = 0;
    int[][] allLocationAmounts = { Constants.locationCardsAmount2Players, Constants.locationCardsAmount3Players, Constants.locationCardsAmount4Players };
    int[][] allIndustryAmounts = { Constants.industryCardsAmount2Players, Constants.industryCardsAmount3Players, Constants.industryCardsAmount4Players };

    int[] locationAmounts = allLocationAmounts[GameManager.numOfPlayers - 2];
    int[] industryAmounts = allIndustryAmounts[GameManager.numOfPlayers - 2];

    int cardIndex = 0;
    foreach (string locationPath in Constants.allLocationCards)
    {
      var newCardResource = HelpFunctions.LoadPrefabFromFile(locationPath);
      for (int i = 0; i < locationAmounts[cardIndex]; i++)
      {
        GameObject newCardObject = Instantiate(newCardResource) as GameObject;
        CardScript newCard = newCardObject.GetComponent<CardScript>();
        allLocationCards.Add(newCard);
        allCards.Add(newCard);

        newCard.id = cardId;
        cardId++;
      }
      cardIndex++;
    }
    
    
    cardIndex = 0;
    foreach (string industryPath in Constants.allIndustryCards)
    {
      var newCardResource = HelpFunctions.LoadPrefabFromFile(industryPath);
      for (int i = 0; i < industryAmounts[cardIndex]; i++)
      {
        GameObject newCardObject = Instantiate(newCardResource) as GameObject;
        CardScript newCard = newCardObject.GetComponent<CardScript>();
        allIndustryCards.Add(newCard);
        allCards.Add(newCard);

        newCard.id = cardId;
        cardId++;
      }
      cardIndex++;
    }

    var wildLocationResource = HelpFunctions.LoadPrefabFromFile(Constants.cardWildLocationPath);
    for (int i = 0; i < Constants.wildLocationCardsAmount; i++)
    {
      GameObject newCardObject = Instantiate(wildLocationResource) as GameObject;
      CardScript newCard = newCardObject.GetComponent<CardScript>();
      allWildLocationCards.Add(newCard);
      allCards.Add(newCard);

      newCard.id = cardId;
      cardId++;
    }


    var wildIndustryResource = HelpFunctions.LoadPrefabFromFile(Constants.cardWildIndustryPath);
    for (int i = 0; i < Constants.wildIndustryCardsAmount; i++)
    {
      GameObject newCardObject = Instantiate(wildIndustryResource) as GameObject;
      CardScript newCard = newCardObject.GetComponent<CardScript>();
      allWildIndustryCards.Add(newCard);
      allCards.Add(newCard);

      newCard.id = cardId;
      cardId++;
    }

    PutAllCardsInDrawDecks();
    ShuffleDrawDeck();

    alreadyCreatedCards = true;

  }

  public static void PutAllCardsInDrawDecks()
  {
    foreach (CardScript card in allLocationCards)
    {
      drawDeck.AddCard(card);
    }
    foreach (CardScript card in allIndustryCards)
    {
      drawDeck.AddCard(card);
    }
    foreach (CardScript card in allWildIndustryCards)
    {
      wildIndustryDrawDeck.AddCard(card);
    }
    foreach (CardScript card in allWildLocationCards)
    {
      wildLocationDrawDeck.AddCard(card);
    }
  }

  public static void ShuffleDrawDeck()
  {
    drawDeck.RandomlyShuffle();
  }

  public static void DestroyAllCards()
  {
    if (!alreadyCreatedCards)
      return;
    foreach(CardScript card in allCards)
    {
      card.gameObject.SetActive(false);
    }

    alreadyCreatedCards = false;

    ClearChosenCards();

    allCards.Clear();
    allLocationCards.Clear();
    allIndustryCards.Clear();
    allWildIndustryCards.Clear();
    allWildLocationCards.Clear();

    foreach (List<CardScript> hand in playerHands)
      hand.Clear();
    foreach (List<CardScript> discard in playerDiscards)
      discard.Clear();

    drawDeck.RemoveAllCards();
    wildIndustryDrawDeck.RemoveAllCards();
    wildLocationDrawDeck.RemoveAllCards();

    DestroyAllBorders();

  }

  public static void DestroyAllBorders()
  {
    foreach (GameObject border in allBorders)
    {
      border.SetActive(false);
    }

    allBorders.Clear();
  }
  public static bool AllPlayersHaveEmptyHands()
  {
    foreach (List<CardScript> hand in playerHands)
      if (hand.Count > 0) return false;
    return true;
  }
  public static void FillPlayerHand(int playerIndex)
  {
    while (!drawDeck.isEmpty && playerHands[playerIndex].Count < 8 )
      PlayerDrawCard(playerIndex, CardDeckType.NORMAL);
  }
  public static void ActivePlayerDrawCard(CardDeckType type)
  {
    PlayerDrawCard(GameManager.activePlayerIndex, type);
  }

  public static void PlayerDrawCard(int playerIndex, CardDeckType type)
  {
    CardScript drawnCard;
    bool drawWildCard = false;
    switch (type)
    {
      case CardDeckType.NORMAL:
        drawnCard = drawDeck.DrawCard();
        break;
      case CardDeckType.WILD_INDUSTRY:
        if (!canDrawWildCards) return;
        drawWildCard = true;
        drawnCard = wildIndustryDrawDeck.DrawCard();
        break;
      case CardDeckType.WILD_LOCATION:
        if (!canDrawWildCards) return;
        drawWildCard = true;
        drawnCard = wildLocationDrawDeck.DrawCard();
        break;
      default:
        drawnCard = null;
        Debug.LogError("Unknown deck type");
        break;
    }
    if (drawnCard is null)
    {
      //Debug.Log("Draw deck already empty!! Cannot draw another card");
      return;
    }
    if (drawWildCard && ActionManager.currentAction == ACTION.SCOUT)
      ActionManager.ScoutChoseWildCard();
    drawnCard.Draw();
    playerHands[playerIndex].Add(drawnCard);
    RemakeHandView(playerIndex);
  }

  private static void RemakeHandView(int playerIndex)
  {
    List<CardScript> hand = playerHands[playerIndex];
    //Debug.Log($"Remaking view of hand {hand}");
    if (hand.Count <= 0) return;


    GameObject board = playersHandBoards[playerIndex];

    RemakeView(hand, board);
  }

  private static void RemakeDiscardView(int playerIndex)
  {
    List<CardScript> discard = playerDiscards[playerIndex];
    if (discard.Count <= 0) return;


    GameObject board = playersDiscardBoards[playerIndex];

    if (board is null) Debug.Log("Board is null");

    //Debug.Log("Board: " + board.ToString());
    //Debug.Log("Discard set: " + discard.ToString());


    RemakeView(discard, board);
  }

  private static void RemakeView(List<CardScript> cardList, GameObject board)
  {
    int matrixSize = Mathf.CeilToInt(Mathf.Sqrt(cardList.Count));
    float middle = ((matrixSize - 1) / 2.0f);
    float cardSizeX = cardList[0].GetComponent<BoxCollider2D>().size.x * cardList[0].transform.localScale.x; //Expecting all cards are the same size
    float cardSizeY = cardList[0].GetComponent<BoxCollider2D>().size.y * cardList[0].transform.localScale.y;

    //Debug.Log("Card size X: " + cardSizeX.ToString());
    //Debug.Log("Card size Y: " + cardSizeY.ToString());
    //Debug.Log("Matrix size: " + matrixSize.ToString());
    //Debug.Log("Middle: " + middle.ToString());


    foreach (CardScript card in cardList)
    {
      card.transform.position = board.transform.position;
      card.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    for (int i = 0; i < matrixSize; i++)
    {
      float xChange = (i - middle) * cardSizeX;
      for (int j = 0; j < matrixSize; j++)
      {
        int cardIndex = (i * matrixSize) + j;
        if (cardIndex >= cardList.Count) continue;
        float yChange = (j - middle) * cardSizeY;
        cardList[cardIndex].transform.position = board.transform.position + new Vector3(xChange, yChange, 0);
        //Debug.Log("Card positioned!  X Change: " + xChange.ToString() + " Y Change: " + yChange.ToString() + " Final position: " + cardList[cardIndex].transform.position.ToString());
      }
    }
  }

  public static List<CardScript> GetPlayerCards(int playerIndex)
  {
    return playerHands[playerIndex];
  }

  public static void HighlightPlayerDiscards(int playerIndex)
  {
    HighlightCards(playerDiscards[playerIndex]);
  }

  public static void HighlightAllCards()
  {
    HighlightCards(allCards);
  }

  public static void HighlightWildDecks()
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.cardBorderPath);

    if (!wildLocationDrawDeck.isEmpty)
    {
      var wildLocBorder = Instantiate(borderResource) as GameObject;
      wildLocBorder.transform.SetLocalPositionAndRotation(wildLocationDrawDeck.transform.position, wildLocationDrawDeck.transform.rotation);
      allBorders.Add(wildLocBorder);
    }

    if (!wildIndustryDrawDeck.isEmpty)
    {
      var wildIndBorder = Instantiate(borderResource) as GameObject;
      wildIndBorder.transform.SetLocalPositionAndRotation(wildIndustryDrawDeck.transform.position, wildIndustryDrawDeck.transform.rotation);
      allBorders.Add(wildIndBorder);
    }

  }

  public static void HighlightCards(List<CardScript> cards)
  {
    var borderResource = HelpFunctions.LoadPrefabFromFile(Constants.cardBorderPath);

    foreach (CardScript location in cards)
    {
      var newBorder = Instantiate(borderResource) as GameObject;
      newBorder.transform.position = location.transform.position;
      newBorder.transform.rotation = location.transform.rotation;
      allBorders.Add(newBorder);
    }
  }


  public static bool PlayerHasWildCard(int playerIndex)
  {
    bool doesHaveWildCard = false;
    foreach (CardScript card in playerHands[playerIndex])
    {
      if (card.myType == CARD_TYPE.WILD_LOCATION || card.myType == CARD_TYPE.WILD_INDUSTRY)
      {
        doesHaveWildCard = true;
        break;
      }
    }
    return doesHaveWildCard;
  }

  public static void ChooseCardFromHand(CardScript card)
  {
    if (!canChooseCard) return;

    chosenCards.Add(card);
    DiscardCard(card, GameManager.activePlayerIndex);

    ActionManager.ChoseCard(card);

  }

  public static void ClearChosenCards()
  {
    chosenCards.Clear();
  }

  public static void ReturnCardFromDiscard(CardScript card, int playerIndex)
  {
    canDrawWildCards = true;
    if (card.myType == CARD_TYPE.WILD_INDUSTRY)
      PlayerDrawCard(playerIndex, CardDeckType.WILD_INDUSTRY);
    else if (card.myType == CARD_TYPE.WILD_LOCATION)
      PlayerDrawCard(playerIndex, CardDeckType.WILD_LOCATION);
    else
    {
      playerDiscards[playerIndex].Remove(card);
      playerHands[playerIndex].Add(card);
      RemakeDiscardView(playerIndex);

    }
    canDrawWildCards = false;

    card.Draw();
    RemakeHandView(playerIndex);

  }

  public static void DiscardCard(CardScript card, int playerIndex)
  {
    playerHands[playerIndex].Remove(card);
    if (card.myType == CARD_TYPE.WILD_LOCATION)
    {
      wildLocationDrawDeck.AddCard(card);
    }
    else if (card.myType == CARD_TYPE.WILD_INDUSTRY)
    {
      wildIndustryDrawDeck.AddCard(card);
    }
    else
    {
      playerDiscards[playerIndex].Add(card);
      RemakeDiscardView(playerIndex);
      card.Discard();
    }

    RemakeHandView(playerIndex);
  }

  public static CardScript GetLastPlayerCard(int playerIndex)
  {
    if (playerIndex >= GameManager.numOfPlayers || playerHands[playerIndex].Count <= 0) return null;
    else return playerHands[playerIndex][playerHands[playerIndex].Count-1];
  }

  public static CardScript GetLastPlayerDiscard(int playerIndex)
  {
    if (playerIndex >= GameManager.numOfPlayers || playerDiscards[playerIndex].Count <= 0) return null;
    else return playerDiscards[playerIndex][playerDiscards[playerIndex].Count-1];
  }
  public void PopulateSaveData(SaveData sd) => PopulateSaveDataStatic(sd);
  public static void PopulateSaveDataStatic(SaveData sd)
  {
    foreach (CardScript card in allCards)
    {
      //if(card.myState == CARD_STATE.IN_HAND)
      //{
      //  Debug.Log($"Populating card {card} with id {card.id} which should be in hand");
      //}
      card.PopulateSaveData(sd);
    }

    SaveData.CardManagerData myData = new();
    myData.player1HandIds = new();
    myData.player2HandIds = new();
    myData.player3HandIds = new();
    myData.player4HandIds = new();

    int playerIndex = 0;
    foreach(List<CardScript> playerHand in playerHands)
    {
      List<int> dataHand = new();
      if (playerIndex == 0) dataHand = myData.player1HandIds;
      else if (playerIndex == 1) dataHand = myData.player2HandIds;
      else if (playerIndex == 2) dataHand = myData.player3HandIds;
      else if (playerIndex == 3) dataHand = myData.player4HandIds;
      foreach (CardScript card in playerHand)
      {
        Debug.Log($"Adding {card} with id {card.id} to {playerIndex} hand in data");
        dataHand.Add(card.id);
      }

      playerIndex++;
    }

    myData.player1DiscardIds = new();
    myData.player2DiscardIds = new();
    myData.player3DiscardIds = new();
    myData.player4DiscardIds = new();
    playerIndex = 0;
    foreach (List<CardScript> playerDiscard in playerDiscards)
    {
      List<int> dataDiscard = new() ;
      if (playerIndex == 0) dataDiscard = myData.player1DiscardIds;
      else if (playerIndex == 1) dataDiscard = myData.player2DiscardIds;
      else if (playerIndex == 2) dataDiscard = myData.player3DiscardIds;
      else if (playerIndex == 3) dataDiscard = myData.player4DiscardIds;
      foreach (CardScript card in playerDiscard)
      {
        Debug.Log($"Adding {card} with id {card.id} to {playerIndex} discard in data");
        dataDiscard.Add(card.id);
      }

      playerIndex++;
    }

    sd.cardManagerData = myData;
  }
  public void LoadFromSaveData(SaveData sd) => LoadFromSaveDataStatic(sd);

  public static void LoadFromSaveDataStatic(SaveData sd)
  {
    PutAllCardsInDrawDecks();
    drawDeck.RemoveAllCards();
    wildIndustryDrawDeck.RemoveAllCards();
    wildLocationDrawDeck.RemoveAllCards();
    canChooseCard = false;
    canDrawWildCards = false;
    DestroyAllBorders();
    chosenCards.Clear();
    foreach (List<CardScript> hand in playerHands)
      hand.Clear();

    foreach (List<CardScript> discard in playerDiscards)
      discard.Clear();

    foreach (CardScript card in allCards)
    {
      card.LoadFromSaveData(sd);

      switch (card.myState)
      {
        case CARD_STATE.IN_DRAW_DECK:
          card.BecomeUnclickable();
          switch (card.myType)
          {
            case CARD_TYPE.LOCATION:
            case CARD_TYPE.INDUSTRY:
              //Debug.Log($"Adding {card} with id {card.id} to drawDeck");
              drawDeck.AddCard(card);
              break;
            case CARD_TYPE.WILD_LOCATION:
              //Debug.Log($"Adding {card} with id {card.id} to wildDeck");
              wildLocationDrawDeck.AddCard(card);
              break;
            case CARD_TYPE.WILD_INDUSTRY:
              //Debug.Log($"Adding {card} with id {card.id} to wildDeck");
              wildIndustryDrawDeck.AddCard(card);
              break;
            default:
              break;
          }
          break;


        case CARD_STATE.IN_HAND:
          card.BecomeClickable();
          if (sd.cardManagerData.player1HandIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 1 hand");
            player1Hand.Add(card);
          }
          else if (sd.cardManagerData.player2HandIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 2 hand");

            player2Hand.Add(card);
          }
          else if (sd.cardManagerData.player3HandIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 3 hand");

            player3Hand.Add(card);
          }
          else if (sd.cardManagerData.player4HandIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 4 hand");

            player4Hand.Add(card);
          }
          else
          {
            //Debug.Log($"Card {card} with id {card.id} should be in hand, but didnt find owner");
          }
          break;


        case CARD_STATE.DISCARDED:
          card.BecomeUnclickable();
          if (sd.cardManagerData.player1DiscardIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 1 discard");

            player1Discard.Add(card);
          }
          else if (sd.cardManagerData.player2DiscardIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 2 discard");


            player2Discard.Add(card);
          }
          else if (sd.cardManagerData.player3DiscardIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 3 discard");


            player3Discard.Add(card);
          }
          else if (sd.cardManagerData.player4DiscardIds.Contains(card.id))
          {
            //Debug.Log($"Adding card {card} with id {card.id} to player 4 discard");

            player4Discard.Add(card);
          }
          break;
        default:
          ////Debug.Log($"Card {card} with id {card.id} and state {card.myState.ToString()} didn't get anywhere!");
          break;
      }

    }

    playerHands[0] = player1Hand;
    playerHands[1] = player2Hand;
    playerHands[2] = player3Hand;
    playerHands[3] = player4Hand;

    playerDiscards[0] = player1Discard;
    playerDiscards[1] = player2Discard;
    playerDiscards[2] = player3Discard;
    playerDiscards[3] = player4Discard;


    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      RemakeDiscardView(i);
      RemakeHandView(i);
      //Debug.Log($"Loaded player {i} hand: ");
      //foreach (CardScript card in playerHands[i])
        //Debug.Log($"Card {card} with id {card.id}");
    }
  }
}
