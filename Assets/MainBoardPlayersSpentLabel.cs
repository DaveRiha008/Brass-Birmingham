using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainBoardPlayersSpentLabel : MonoBehaviour
{
  public int turnIndex = 0;
  TextMeshPro myTM;
  // Start is called before the first frame update
  void Start()
  {
    myTM = GetComponent<TextMeshPro>();
  }

  // Update is called once per frame
  void Update()
  {
    myTM.text = GameManager.GetPlayer(GameManager.playerTurns[turnIndex]).moneySpentThisTurn.ToString();
  }
}
