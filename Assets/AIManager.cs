using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager:MonoBehaviour
{
  public static AIManager instance;

  static bool isPlaying = false;
  public static bool playFreely = false;

  static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.CHOOSE_FIRST, AI_STRATEGY.CHOOSE_FIRST, AI_STRATEGY.CHOOSE_FIRST, AI_STRATEGY.CHOOSE_FIRST };

  static AIBehaviour[] strategiesPerPlayer = { 
    new AIBehaviour(GameManager.GetPlayer(0)),
    new AIBehaviour(GameManager.GetPlayer(1)),
    new AIBehaviour(GameManager.GetPlayer(2)),
    new AIBehaviour(GameManager.GetPlayer(3)) };


  void Start()
  {
    LoadPlayerStrategies();
  }

  private void Awake()
  {
  }
  private void Update()
  {
    //Debug.Log("AIManager update");
    if (!isPlaying) return;
    if (playFreely) AIDoNextPart();

  }

  private void OnDestroy()
  {
    strategiesPerPlayer[0] = new AIBehaviour(GameManager.GetPlayer(0));
    strategiesPerPlayer[1] = new AIBehaviour(GameManager.GetPlayer(1));
    strategiesPerPlayer[2] = new AIBehaviour(GameManager.GetPlayer(2));
    strategiesPerPlayer[3] = new AIBehaviour(GameManager.GetPlayer(3));
  }

  static void LoadPlayerStrategies()
  {
    Debug.Log("LoadPlayerStrategies called");
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      Player player = GameManager.GetPlayer(i);
      if (!player.AIReplaced) continue;
      switch (playerStrategies[i])
      {
        case AI_STRATEGY.CHOOSE_FIRST:
          strategiesPerPlayer[i] = new AIBehaviour(player);
          break;
        case AI_STRATEGY.NONE:
          break;
        default:
          break;
      }
    }
  }

  public static AI_STRATEGY GetPlayerStrategy(int playerIndex) => playerStrategies[playerIndex];

  //TODO: Implement correct AI turn by a certaion strategy
  //TODO: Make the game locked for real players while AI plays
  //TODO: For better player experience -> make pauses between each AI action to better show what is going on
  public static void PlayTurn()
  {
    //Debug.Log("AI Playing turn");
    isPlaying = true;

    if (Camera.main.TryGetComponent<CameraScript>(out CameraScript camera))
    {
      camera.MoveToMainBoard();
      //camera.lockMainBoard = true;
    }

    strategiesPerPlayer[GameManager.activePlayerIndex].StartTurn();
  }

  public static void ActionWasDone()
  {
    if (!isPlaying) return;
    if(!ActionManager.PlayerDoneAllActions())
      strategiesPerPlayer[GameManager.activePlayerIndex].StartTurn();
    else
    {

      //if(Camera.main.TryGetComponent<CameraScript>(out CameraScript camera))
      //  camera.lockMainBoard = false;
      isPlaying = false;
      //Debug.Log("Calling EndTurn from AI -> Done all actions");
      GameManager.EndTurn();
    }
  }

  public static void AIDoNextPart()
  {
    if (!isPlaying) return;
    AIBehaviour curStrat = strategiesPerPlayer[GameManager.activePlayerIndex];
    //Debug.Log($"AI doing next part base on current aciton: {ActionManager.currentState}");

    switch (ActionManager.currentState)
    {
      case ACTION_STATE.CHOOSING_CARD:
        //Debug.Log($"Telling AI to choose card when current action state is {ActionManager.currentState}");
        curStrat.ChooseCard();
        break;
      case ACTION_STATE.CHOOSING_DECK:
        //Debug.Log("Telling AI to choose deck"); 
        curStrat.ChooseWildCard();
        break;
      case ACTION_STATE.CHOOSING_TILE:
        //Debug.Log("Telling AI to choose tile");
        curStrat.ChooseTile();
        break;
      case ACTION_STATE.CHOOSING_SPACE:
        //Debug.Log("Telling AI to choose space");
        curStrat.ChooseIndustrySpace();
        break;
      case ACTION_STATE.CHOOSING_IRON:
        //Debug.Log("Telling AI to choose iron");
        curStrat.ChooseIron();
        break;
      case ACTION_STATE.CHOOSING_COAL:
        //Debug.Log("Telling AI to choose coal");
        curStrat.ChooseCoal();
        break;
      case ACTION_STATE.CHOOSING_BARREL:
        //Debug.Log("Telling AI to choose barrel");
        curStrat.ChooseBarrel();
        break;
      case ACTION_STATE.CHOOSING_NETWORK_SPACE:
        //Debug.Log("Telling AI to choose network");
        curStrat.ChooseNetwork();
        break;
      case ACTION_STATE.NONE:
        //Debug.Log("Telling AI to start new Action");
        curStrat.StartAction();
        break;
      default:
        break;
    }
  }
}
public enum AI_STRATEGY { CHOOSE_FIRST, NONE }