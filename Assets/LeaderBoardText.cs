using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardText : MonoBehaviour
{
  TextMeshProUGUI textMesh;
  // Start is called before the first frame update
  void Start()
  {
    textMesh = GetComponent<TextMeshProUGUI>();
  }

  // Update is called once per frame
  void Update()
  {

    //textMesh.text = "Final leaderboard \n";
    textMesh.text = "Koneèné poøadí\n";
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      textMesh.text += $"\n {(i + 1).ToString()} : {GameManager.playerWinningOrder[i].name} ({GameManager.playerWinningOrder[i].victoryPoints})";
    }

  }
}
