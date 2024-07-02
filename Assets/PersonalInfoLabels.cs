using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersonalInfoLabels : MonoBehaviour
{
  public int playerIndex = 0;
  TextMeshPro playerName;
  TextMeshPro playerMoney;
  TextMeshPro playerIncome;
  TextMeshPro playerIncomePts;
  TextMeshPro playerVicPts;

  // Start is called before the first frame update
  void Start()
  {
    playerName = transform.Find("PlayerName").gameObject.GetComponent<TextMeshPro>();
    playerMoney = transform.Find("Money").gameObject.GetComponent<TextMeshPro>();
    playerIncome = transform.Find("Income").gameObject.GetComponent<TextMeshPro>();
    playerIncomePts = transform.Find("IncomePoints").gameObject.GetComponent<TextMeshPro>();
    playerVicPts = transform.Find("VictoryPoints").gameObject.GetComponent<TextMeshPro>();
  }

  // Update is called once per frame
  void Update()
  {
    Player player = GameManager.GetPlayer(playerIndex);
    //playerName.text = "Name: " + player.name;
    //playerMoney.text = "Money: " + player.money.ToString();
    //playerIncome.text = "Income: " + GameManager.GetPlayerIncomeLvl(playerIndex).ToString();
    //playerIncomePts.text = "Income points: " + player.income.ToString();
    //playerVicPts.text = "Victory points: " + player.victoryPoints.ToString();

    playerName.text = "Jm�no: " + player.name;
    playerMoney.text = "Pen�ze: " + player.money.ToString();
    playerIncome.text = "P��jem: " + GameManager.GetPlayerIncomeLvl(playerIndex).ToString();
    playerIncomePts.text = "P��jmov� body: " + player.income.ToString();
    playerVicPts.text = "V�t�zn� body: " + player.victoryPoints.ToString();
    
  }
}
