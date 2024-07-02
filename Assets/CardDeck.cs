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
  // Start is called before the first frame update
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
    card.transform.SetPositionAndRotation(transform.position + new Vector3(0, 0, 5), Quaternion.Euler(new Vector3(0, 0, 90)));//Set position and rotation acording to deck but behind its collider
    myCards.Add(card);
    card.ShuffleInDeck();
    isEmpty = false;
  }

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

    //Debug.Log("Drawing card from Normal drawDeck");

    return returnCard;
  }

  public void RemoveAllCards()
  {
    myCards = new();
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
