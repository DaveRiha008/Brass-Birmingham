using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCharHighlight : MonoBehaviour
{
  //All these values must be set manually in the inspector
  public Vector3 player1Pos;
  public Vector3 player2Pos;
  public Vector3 player3Pos;
  public Vector3 player4Pos;

  List<Vector3> playerPositions = new List<Vector3>();
  // Start is called before the first frame update
  void Start()
  {
    playerPositions.Add(player1Pos);
    playerPositions.Add(player2Pos);
    playerPositions.Add(player3Pos);
    playerPositions.Add(player4Pos);
  }

  // Update is called once per frame
  void Update()
  {
    transform.SetLocalPositionAndRotation(playerPositions[GameManager.activePlayerTurnIndex], transform.rotation);
  }
}
