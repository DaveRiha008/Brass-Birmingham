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
    playerName = transform.Find(Constants.personalBoardNameLabelName).gameObject.GetComponent<TextMeshPro>();
    playerMoney = transform.Find(Constants.personalBoardMoneyLabelName).gameObject.GetComponent<TextMeshPro>();
    playerIncome = transform.Find(Constants.personalBoardIncomeLabelName).gameObject.GetComponent<TextMeshPro>();
    playerIncomePts = transform.Find(Constants.personalBoardIncomePointsLabelName).gameObject.GetComponent<TextMeshPro>();
    playerVicPts = transform.Find(Constants.personalBoardVictoryPointsLabelName).gameObject.GetComponent<TextMeshPro>();

    playerName.color = Constants.playerColors[playerIndex];
    playerMoney.color = Constants.playerColors[playerIndex];
    playerIncome.color = Constants.playerColors[playerIndex];
    playerIncomePts.color = Constants.playerColors[playerIndex];
    playerVicPts.color = Constants.playerColors[playerIndex];
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

    playerName.text = "Jméno: " + player.name;
    playerMoney.text = "Peníze: " + player.money.ToString();
    playerIncome.text = "Pøíjem: " + GameManager.GetPlayerIncomeLvl(playerIndex).ToString();
    playerIncomePts.text = "Pøíjmové body: " + player.income.ToString();
    playerVicPts.text = "Vítìzné body: " + player.victoryPoints.ToString();
    
  }
}
