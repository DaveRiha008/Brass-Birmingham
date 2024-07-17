using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : Clickable, ISaveable
{
  public int id = 0;
  public CARD_STATE myState = CARD_STATE.NONE;

  //SET MANUALLY IN INSPECTOR
  public CARD_TYPE myType = CARD_TYPE.LOCATION;

  /// <summary>
  /// If this is industry card -> all present industries
  /// </summary>
  public List<INDUSTRY_TYPE> myIndustries = new();
  /// <summary>
  /// If this is location card -> name of location on card
  /// </summary>
  public string myLocation = "";
  public override void OnClick()
  {
    //Debug.Log("Card Clicked!");
    if (myState == CARD_STATE.IN_HAND)
    {
      CardManager.ChooseCardFromHand(this);
    }
  }

  public void ShuffleInDeck()
  {
    BecomeUnclickable();
    myState = CARD_STATE.IN_DRAW_DECK;
  }

  public void Draw()
  {
    BecomeClickable();
    myState = CARD_STATE.IN_HAND;
  }

  public void Discard()
  {
    BecomeUnclickable();
    myState = CARD_STATE.DISCARDED;
  }

  public CARD_STATE GetState()
  {
    return myState;
  }

  public void PopulateSaveData(SaveData sd)
  {
    SaveData.CardData cardData = new();
    cardData.id = id;
    cardData.type = myType;
    cardData.state = myState;
    cardData.myIndustries = myIndustries;
    cardData.myLocation = myLocation;

    sd.cardData.Add(cardData);
  }

  public void LoadFromSaveData(SaveData sd)
  {
    foreach (SaveData.CardData cardData in sd.cardData)
      if (cardData.id == id)
      {
        myType = cardData.type;
        myState = cardData.state;
        myIndustries = cardData.myIndustries;
        myLocation = cardData.myLocation;
      }
    
  }

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
        
  }
}

public enum CARD_TYPE { LOCATION, INDUSTRY, WILD_LOCATION, WILD_INDUSTRY }
public enum CARD_STATE { IN_DRAW_DECK, IN_HAND, DISCARDED, NONE }
