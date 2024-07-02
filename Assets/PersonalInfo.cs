using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalInfo : MonoBehaviour
{
  public int playerIndex = 0;

  ClickThroughHand hand;
  ClickThroughHand discard;
  // Start is called before the first frame update
  void Start()
  {
    hand = transform.Find("Hand").gameObject.GetComponent<ClickThroughHand>();
    discard = transform.Find("Discard").gameObject.GetComponent<ClickThroughHand>();
  }

  // Update is called once per frame
  void Update()
  {

    CardScript lastCardInHand = CardManager.GetLastPlayerCard(playerIndex);
    if (lastCardInHand is null) hand.GetComponent<SpriteRenderer>().sprite = null;
    else
    {
      hand.GetComponent<SpriteRenderer>().sprite = lastCardInHand.GetComponent<SpriteRenderer>().sprite;
      hand.transform.localScale = lastCardInHand.transform.localScale * 1;
    }

    CardScript lastCardInDiscard = CardManager.GetLastPlayerDiscard(playerIndex);
    if (lastCardInDiscard is null) discard.GetComponent<SpriteRenderer>().sprite = null;
    else
    {
      discard.GetComponent<SpriteRenderer>().sprite = lastCardInDiscard.GetComponent<SpriteRenderer>().sprite;
      discard.transform.localScale = lastCardInDiscard.transform.localScale * 1;   
    }
  }
}
