using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardText : MonoBehaviour
{
  TextMeshProUGUI textMeshUI;
  TextMeshPro textMesh;

  public bool UI = false;
  // Start is called before the first frame update
  void Start()
  {
    if (UI)
      textMeshUI = GetComponent<TextMeshProUGUI>();
    else
      textMesh = GetComponent<TextMeshPro>();
  }

  // Update is called once per frame
  void Update()
  {
    // For final victory screen
    if (UI)
    {
      //textMesh.text = "Final leaderboard \n";
      textMeshUI.text = "Koneèné poøadí";
      for (int i = 0; i < GameManager.numOfPlayers; i++)
      {
        Player curPlayer = GameManager.playerWinningOrder[i];
        textMeshUI.text += $"\n {(i + 1).ToString()} : " + $"{curPlayer.name} \t Vítìzných bodù { curPlayer.victoryPoints} \t Peníze { curPlayer.money} \t Pøíjmových bodù { curPlayer.income}";
;
      }
    }

    // For mid era screen
    else
    {
      //textMesh.text = "Final leaderboard \n";
      textMesh.text = "Aktuální poøadí\n";
      for (int i = 0; i < GameManager.numOfPlayers; i++)
      {
        Player curPlayer = GameManager.playerWinningOrder[i];
        textMesh.text += $"\n {curPlayer.name}: Vítìzných bodù {curPlayer.victoryPoints} \t Peníze {curPlayer.money} \t Pøíjmových bodù {curPlayer.income}";
      }
    }
  }
}
