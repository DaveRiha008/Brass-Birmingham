using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainBoardPlayersInfo : MonoBehaviour
{
  List<TextMeshPro> moneyLabels = new();
  List<GameObject> characterImages = new();
  List<GameObject> characterImageSpaces = new();

  void Start()
  {
    LoadMoneyLabels();
    LoadCharacterImages();
    LoadImageSpaces();
    ResetMoneyLabels();
  }

  // Update is called once per frame
  void Update()
  {
    SetMoneyLabels();
    UpdateCharImagePositions();
  }

  void LoadCharacterImages()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      var resource = HelpFunctions.LoadPrefabFromFile(Constants.allCharPaths[i]);
      characterImages.Add(Instantiate(resource) as GameObject);
    }
  }

  void LoadImageSpaces()
  {
    Transform charactersParent = GameObject.Find(Constants.mainBoardName).transform.Find(Constants.charactersParentName); 
    foreach( Transform child in charactersParent)
    {
      characterImageSpaces.Add(child.gameObject);
    }
  }

  void UpdateCharImagePositions()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      characterImages[GameManager.playerTurns[i]].transform.position = characterImageSpaces[i].transform.position;
    }
  }

  void LoadMoneyLabels()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      string labelName = "Player" + (i + 1).ToString() + "Info"; //Fixed scene name
      moneyLabels.Add(transform.Find(labelName).gameObject.GetComponent<TextMeshPro>());
    }
  }

  void ResetMoneyLabels()
  {
    foreach (TextMeshPro label in moneyLabels)
    {
      label.text = "";
    }
  }

  void SetMoneyLabels()
  {
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      TextMeshPro label = moneyLabels[i];
      Player player = GameManager.GetPlayer(GameManager.playerTurns[i]);
      string text = player.name + "\n Utraceno: " + player.moneySpentThisTurn + "  Peníze: " + player.money + "  Pøíjem: " + player.income + "  Body: " + player.victoryPoints;
      //Debug.Log(labelName);
      label.text = text;
      label.color = Constants.playerColors[player.index];
    }
  }
}
