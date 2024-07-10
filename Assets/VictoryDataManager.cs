using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryDataManager
{
  public static void UpdateVictoryData()
  {
    VictoryData newVicData = new VictoryData();
    List<int> playerVictories = new();
    List<int> playerSeconds = new();
    List<int> playerThirds = new();
    List<int> playerFourths = new();
    List<int> playerVicPts = new();
    List<string> playerStrats = new();

    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      Player player = GameManager.GetPlayer(i);
      playerVicPts.Add(player.victoryPoints);
      //Debug.Log($"Adding strategy of player {i} to {AIManager.GetPlayerStrategy(i).ToString()} ");
      playerStrats.Add(AIManager.GetPlayerStrategy(i).ToString());
      if (GameManager.playerWinningOrder[0] == player) playerVictories.Add(1);
      else playerVictories.Add(0);

      if (GameManager.playerWinningOrder[1] == player) playerSeconds.Add(1);
      else playerSeconds.Add(0);

      if (GameManager.playerWinningOrder[2] == player) playerThirds.Add(1);
      else playerThirds.Add(0);

      if (GameManager.playerWinningOrder[3] == player) playerFourths.Add(1);
      else playerFourths.Add(0);
    }

    newVicData.player1Strategy = playerStrats[0].ToString();
    newVicData.player1VicPoints = playerVicPts[0];
    newVicData.player1Victories = playerVictories[0];
    newVicData.player1Seconds = playerSeconds[0];
    newVicData.player1Thirds = playerThirds[0];
    newVicData.player1Fourths = playerFourths[0];

    newVicData.player2Strategy = playerStrats[1].ToString();
    newVicData.player2VicPoints = playerVicPts[1];
    newVicData.player2Victories = playerVictories[1];
    newVicData.player2Seconds = playerSeconds[1];
    newVicData.player2Thirds = playerThirds[1];
    newVicData.player2Fourths = playerFourths[1];

    newVicData.player3Strategy = playerStrats[2].ToString();
    newVicData.player3VicPoints = playerVicPts[2];
    newVicData.player3Victories = playerVictories[2];
    newVicData.player3Seconds = playerSeconds[2];
    newVicData.player3Thirds = playerThirds[2];
    newVicData.player3Fourths = playerFourths[2];

    newVicData.player4Strategy = playerStrats[3].ToString();
    newVicData.player4VicPoints = playerVicPts[3];
    newVicData.player4Victories = playerVictories[3];
    newVicData.player4Seconds = playerSeconds[3];
    newVicData.player4Thirds = playerThirds[3];
    newVicData.player4Fourths = playerFourths[3];


    VictoryData loadedVicData = new();

    if (FileManager.LoadFromFile(Constants.vicFileName, out var json))
    {
      //Debug.Log($"Loading from JSON: {json}");

      loadedVicData.LoadFromJson(json);

      loadedVicData.player1VicPoints += newVicData.player1VicPoints;
      loadedVicData.player1Victories += newVicData.player1Victories;
      loadedVicData.player1Seconds += newVicData.player1Seconds;
      loadedVicData.player1Thirds += newVicData.player1Thirds;
      loadedVicData.player1Fourths += newVicData.player1Fourths;

      loadedVicData.player2VicPoints += newVicData.player2VicPoints;
      loadedVicData.player2Victories += newVicData.player2Victories;
      loadedVicData.player2Seconds += newVicData.player2Seconds;
      loadedVicData.player2Thirds += newVicData.player2Thirds;
      loadedVicData.player2Fourths += newVicData.player2Fourths;

      loadedVicData.player3VicPoints += newVicData.player3VicPoints;
      loadedVicData.player3Victories += newVicData.player3Victories;
      loadedVicData.player3Seconds += newVicData.player3Seconds;
      loadedVicData.player3Thirds += newVicData.player3Thirds;
      loadedVicData.player3Fourths += newVicData.player3Fourths;

      loadedVicData.player4VicPoints += newVicData.player4VicPoints;
      loadedVicData.player4Victories += newVicData.player4Victories;
      loadedVicData.player4Seconds += newVicData.player4Seconds;
      loadedVicData.player4Thirds += newVicData.player4Thirds;
      loadedVicData.player4Fourths += newVicData.player4Fourths;

      loadedVicData.gamesCounter++;

      //Debug.Log("Load complete");

    }
    else
    {
      //Debug.LogError("Loading from file failed! \n Creating new victory data");
      loadedVicData.player1VicPoints = newVicData.player1VicPoints;
      loadedVicData.player1Victories = newVicData.player1Victories;
      loadedVicData.player1Seconds = newVicData.player1Seconds;
      loadedVicData.player1Thirds = newVicData.player1Thirds;
      loadedVicData.player1Fourths = newVicData.player1Fourths;

      loadedVicData.player2VicPoints = newVicData.player2VicPoints;
      loadedVicData.player2Victories = newVicData.player2Victories;
      loadedVicData.player2Seconds = newVicData.player2Seconds;
      loadedVicData.player2Thirds = newVicData.player2Thirds;
      loadedVicData.player2Fourths = newVicData.player2Fourths;

      loadedVicData.player3VicPoints = newVicData.player3VicPoints;
      loadedVicData.player3Victories = newVicData.player3Victories;
      loadedVicData.player3Seconds = newVicData.player3Seconds;
      loadedVicData.player3Thirds = newVicData.player3Thirds;
      loadedVicData.player3Fourths = newVicData.player3Fourths;

      loadedVicData.player4VicPoints = newVicData.player4VicPoints;
      loadedVicData.player4Victories = newVicData.player4Victories;
      loadedVicData.player4Seconds = newVicData.player4Seconds;
      loadedVicData.player4Thirds = newVicData.player4Thirds;
      loadedVicData.player4Fourths = newVicData.player4Fourths;

      loadedVicData.player1Strategy = newVicData.player1Strategy;
      loadedVicData.player2Strategy = newVicData.player2Strategy;
      loadedVicData.player3Strategy = newVicData.player3Strategy;
      loadedVicData.player4Strategy = newVicData.player4Strategy;


      loadedVicData.gamesCounter = 1;

    }

    int gamesCount = loadedVicData.gamesCounter;
    int sumVP = loadedVicData.player1VicPoints + loadedVicData.player2VicPoints + loadedVicData.player3VicPoints + loadedVicData.player4VicPoints;

    loadedVicData.player1VicPointsPer = loadedVicData.player1VicPoints / (float)sumVP *100;
    loadedVicData.player1VictoriesPer = loadedVicData.player1Victories / (float)gamesCount * 100;
    loadedVicData.player1SecondsPer = loadedVicData.player1Seconds / (float)gamesCount * 100;
    loadedVicData.player1ThirdsPer = loadedVicData.player1Thirds / (float)gamesCount * 100;
    loadedVicData.player1FourthsPer = loadedVicData.player1Fourths / (float)gamesCount * 100;

    loadedVicData.player2VicPointsPer = loadedVicData.player2VicPoints / (float)sumVP * 100;
    loadedVicData.player2VictoriesPer = loadedVicData.player2Victories / (float)gamesCount * 100;
    loadedVicData.player2SecondsPer = loadedVicData.player2Seconds / (float)gamesCount * 100;
    loadedVicData.player2ThirdsPer = loadedVicData.player2Thirds / (float)gamesCount * 100;
    loadedVicData.player2FourthsPer = loadedVicData.player2Fourths / (float)gamesCount * 100;

    loadedVicData.player3VicPointsPer = loadedVicData.player3VicPoints / (float)sumVP * 100;
    loadedVicData.player3VictoriesPer = loadedVicData.player3Victories / (float)gamesCount * 100;
    loadedVicData.player3SecondsPer = loadedVicData.player3Seconds / (float)gamesCount * 100;
    loadedVicData.player3ThirdsPer = loadedVicData.player3Thirds / (float)gamesCount * 100;
    loadedVicData.player3FourthsPer = loadedVicData.player3Fourths / (float)gamesCount * 100;

    loadedVicData.player4VicPointsPer = loadedVicData.player4VicPoints / (float)sumVP * 100;
    loadedVicData.player4VictoriesPer = loadedVicData.player4Victories / (float)gamesCount * 100;
    loadedVicData.player4SecondsPer = loadedVicData.player4Seconds / (float)gamesCount * 100;
    loadedVicData.player4ThirdsPer = loadedVicData.player4Thirds / (float)gamesCount * 100;
    loadedVicData.player4FourthsPer = loadedVicData.player4Fourths / (float)gamesCount * 100;


    if (FileManager.WriteToFile(Constants.vicFileName, loadedVicData.ToJson()))
    {
      //Debug.Log($"Saved {loadedVicData.ToJson()}");
      //Debug.Log("Save successful");
    }
  }
}
