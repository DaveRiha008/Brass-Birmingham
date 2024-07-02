using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryDataManager
{
  public static void UpdateVictoryData()
  {
    VictoryData newVicData = new VictoryData();
    List<int> playerVictories = new();
    List<int> playerVicPts = new();
    List<string> playerStrats = new();

    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      Player player = GameManager.GetPlayer(i);
      playerVicPts.Add(player.victoryPoints);
      playerStrats.Add(AIManager.GetPlayerStrategy(i).ToString());
      if (GameManager.playerWinningOrder[0] == player) playerVictories.Add(1);
      else playerVictories.Add(0);
    }

    newVicData.player1Strategy = playerStrats[0];
    newVicData.player1VicPoints = playerVicPts[0];
    newVicData.player1Victories = playerVictories[0];

    newVicData.player2Strategy = playerStrats[1];
    newVicData.player2VicPoints = playerVicPts[1];
    newVicData.player2Victories = playerVictories[1];

    newVicData.player3Strategy = playerStrats[2];
    newVicData.player3VicPoints = playerVicPts[2];
    newVicData.player3Victories = playerVictories[2];

    newVicData.player4Strategy = playerStrats[3];
    newVicData.player4VicPoints = playerVicPts[3];
    newVicData.player4Victories = playerVictories[3];

    VictoryData loadedVicData = new();

    if (FileManager.LoadFromFile(Constants.vicFileName, out var json))
    {
      Debug.Log($"Loading from JSON: {json}");

      loadedVicData.LoadFromJson(json);

      loadedVicData.player1VicPoints += newVicData.player1VicPoints;
      loadedVicData.player1Victories += newVicData.player1Victories;

      loadedVicData.player2VicPoints += newVicData.player2VicPoints;
      loadedVicData.player2Victories += newVicData.player2Victories;
      
      loadedVicData.player3VicPoints += newVicData.player3VicPoints;
      loadedVicData.player3Victories += newVicData.player3Victories;
      
      loadedVicData.player4VicPoints += newVicData.player4VicPoints;
      loadedVicData.player4Victories += newVicData.player4Victories;

      Debug.Log("Load complete");

    }
    else
    {
      Debug.LogError("Loading from file failed! \n Creating new victory data");
      loadedVicData.player1VicPoints = newVicData.player1VicPoints;
      loadedVicData.player1Victories = newVicData.player1Victories;
      loadedVicData.player1Strategy = newVicData.player1Strategy;

      loadedVicData.player2VicPoints = newVicData.player2VicPoints;
      loadedVicData.player2Victories = newVicData.player2Victories;
      loadedVicData.player2Strategy = newVicData.player2Strategy;

      loadedVicData.player3VicPoints = newVicData.player3VicPoints;
      loadedVicData.player3Victories = newVicData.player3Victories;
      loadedVicData.player3Strategy = newVicData.player3Strategy;

      loadedVicData.player4VicPoints = newVicData.player4VicPoints;
      loadedVicData.player4Victories = newVicData.player4Victories;
      loadedVicData.player4Strategy = newVicData.player4Strategy;
    }

    if (FileManager.WriteToFile(Constants.vicFileName, loadedVicData.ToJson()))
    {
      Debug.Log($"Saved {loadedVicData.ToJson()}");
      Debug.Log("Save successful");
    }
  }
}
