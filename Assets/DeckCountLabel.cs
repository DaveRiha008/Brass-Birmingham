using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckCountLabel : MonoBehaviour
{
  TextMeshPro myTMPro;
  // Start is called before the first frame update
  void Start()
  {
    myTMPro = GetComponent<TextMeshPro>();
  }

  // Update is called once per frame
  void Update()
  {
    myTMPro.text = transform.parent.gameObject.GetComponent<CardDeck>().GetNumberOfCardsInDeck().ToString();
  }
}
