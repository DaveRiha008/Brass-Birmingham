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
      textMeshUI.text = "Kone�n� po�ad�";
      for (int i = 0; i < GameManager.numOfPlayers; i++)
      {
        Player curPlayer = GameManager.playerWinningOrder[i];
        textMeshUI.text += $"\n {(i + 1).ToString()} : " + $"{curPlayer.name} \t V�t�zn�ch bod� { curPlayer.victoryPoints} \t Pen�ze { curPlayer.money} \t P��jmov�ch bod� { curPlayer.income}";
;
      }
    }

    // For mid era screen
    else
    {
      //textMesh.text = "Final leaderboard \n";
      textMesh.text = "Aktu�ln� po�ad�\n";
      for (int i = 0; i < GameManager.numOfPlayers; i++)
      {
        Player curPlayer = GameManager.playerWinningOrder[i];
        textMesh.text += $"\n {curPlayer.name}: V�t�zn�ch bod� {curPlayer.victoryPoints} \t Pen�ze {curPlayer.money} \t P��jmov�ch bod� {curPlayer.income}";
      }
    }
  }
}
