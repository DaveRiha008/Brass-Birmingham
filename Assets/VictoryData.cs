using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VictoryData
{
  public int player1Victories = 0;
  public int player1Seconds= 0;
  public int player1Thirds = 0;
  public int player1Fourths = 0;
  public int player1VicPoints = 0;

  public float player1VictoriesPer = 0;
  public float player1SecondsPer = 0;
  public float player1ThirdsPer = 0;
  public float player1FourthsPer = 0;
  public float player1VicPointsPer = 0;

  public string player1Strategy = "";


  public int player2Victories = 0;
  public int player2Seconds = 0;
  public int player2Thirds = 0;
  public int player2Fourths = 0;
  public int player2VicPoints = 0;

  public float player2VictoriesPer = 0;
  public float player2SecondsPer = 0;
  public float player2ThirdsPer = 0;
  public float player2FourthsPer = 0;
  public float player2VicPointsPer = 0;

  public string player2Strategy = "";


  public int player3Victories = 0;
  public int player3Seconds = 0;
  public int player3Thirds = 0;
  public int player3Fourths = 0;
  public int player3VicPoints = 0;

  public float player3VictoriesPer = 0;
  public float player3SecondsPer = 0;
  public float player3ThirdsPer = 0;
  public float player3FourthsPer = 0;
  public float player3VicPointsPer = 0;

  public string player3Strategy = "";


  public int player4Victories = 0;
  public int player4Seconds = 0;
  public int player4Thirds = 0;
  public int player4Fourths = 0;
  public int player4VicPoints = 0;

  public float player4VictoriesPer = 0;
  public float player4SecondsPer = 0;
  public float player4ThirdsPer = 0;
  public float player4FourthsPer = 0;
  public float player4VicPointsPer = 0;

  public string player4Strategy = "";


  public int gamesCounter = 0;

  public string ToJson()
  {
    return JsonUtility.ToJson(this);
  }

  public void LoadFromJson(string a_Json)
  {
    JsonUtility.FromJsonOverwrite(a_Json, this);
  }
}
