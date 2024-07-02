using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VictoryData
{
  public int player1Victories = 0;
  public int player1VicPoints = 0;
  public string player1Strategy = "";
  public int player2Victories = 0;
  public int player2VicPoints = 0;
  public string player2Strategy = "";
  public int player3Victories = 0;
  public int player3VicPoints = 0;
  public string player3Strategy = "";
  public int player4Victories = 0;
  public int player4VicPoints = 0;
  public string player4Strategy = "";

  public string ToJson()
  {
    return JsonUtility.ToJson(this);
  }

  public void LoadFromJson(string a_Json)
  {
    JsonUtility.FromJsonOverwrite(a_Json, this);
  }
}
