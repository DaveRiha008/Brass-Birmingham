using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : Clickable
{
  Sprite myOriginalSprite;
  SpriteRenderer mySpriteRenderer;

  public bool isEmpty = true;
  List<CardScript> myCards = new();

  public CardDeckType myType = CardDeckType.NORMAL;


  void Start()
  {
    mySpriteRenderer = GetComponent<SpriteRenderer>();
    myOriginalSprite = mySpriteRenderer.sprite;
  }

  // Update is called once per frame
  void Update()
  {
    if (isEmpty) mySpriteRenderer.sprite = null;
    else mySpriteRenderer.sprite = myOriginalSprite;
  }

  public void AddCard(CardScript card)
  {
    if (myCards.Contains(card)) return;

    card.transform.SetPositionAndRotation(transform.position + new Vector3(0, 0, 5), Quaternion.Euler(new Vector3(0, 0, 90)));//Set position and rotation acording to deck but behind its collider
    myCards.Add(card);
    card.ShuffleInDeck();


    isEmpty = false;
  }

  /// <summary>
  /// Removes card from this deck
  /// </summary>
  /// <returns>Drawn card</returns>
  public CardScript DrawCard()
  {
    if (myCards.Count <= 0)
    {
      isEmpty = true;
      return null;
    }
    isEmpty = false;
    CardScript returnCard = myCards[0];
    myCards.RemoveAt(0);
    returnCard.Draw();

    if (myCards.Count <= 0)
      isEmpty = true;
    //Debug.Log("Drawing card from Normal drawDeck");

    return returnCard;
  }

  /// <summary>
  /// Deck forgets all card - but cards remain active objects
  /// </summary>
  public void RemoveAllCards()
  {
    myCards = new();
    isEmpty = true;
  }

  /// <summary>
  /// Deck forgets the given card and sets its state to -> IN_HAND
  /// </summary>
  /// <param name="card">Card to be removed</param>
  public void RemoveCard(CardScript card)
  {
    if(!myCards.Contains(card) )
    {
      Debug.LogError("Can't remove card, which is not in this deck");
      return;
    }

    myCards.Remove(card);
    card.Draw();
    if (myCards.Count <= 0)
      isEmpty = true;

  }

  public int GetNumberOfCardsInDeck() => myCards.Count;

  public void RandomlyShuffle()
  {
    //List<CardScript> tempList = new();
    //for (int i = 0; i < myCards.Count; i++)
    //{
    //  int randInt = Random.Range(0, myCards.Count);
    //  tempList.Add(myCards[randInt]);
    //  myCards.RemoveAt(randInt);
    //}
    //myCards = tempList;
    myCards = HelpFunctions.GetShuffledList(myCards);
  }
  public override void OnClick()
  {
    CardManager.ActivePlayerDrawCard(myType);
    //Debug.Log("CardDeck clicked!");
  }
}

public enum CardDeckType { NORMAL, WILD_INDUSTRY, WILD_LOCATION }
