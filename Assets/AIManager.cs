using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager:MonoBehaviour
{
  public static AIManager instance;

  /// <summary>
  /// Decides whether now is the AI's turn
  /// </summary>
  static bool isPlaying = false;

  /// <summary>
  /// Decides whether AI is playing without any input
  /// </summary>
  public static bool playFreely = false;

  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.CHOOSE_BETTER, AI_STRATEGY.CHOOSE_BETTER, AI_STRATEGY.CHOOSE_FIRST, AI_STRATEGY.CHOOSE_FIRST };
  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.CHOOSE_REPEATEDLY, AI_STRATEGY.CHOOSE_REPEATEDLY, AI_STRATEGY.CHOOSE_BETTER, AI_STRATEGY.CHOOSE_FIRST };
  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.DIFF_ORDER5, AI_STRATEGY.DIFF_ORDER6, AI_STRATEGY.DIFF_ORDER7, AI_STRATEGY.DIFF_ORDER8 };
  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.CHOOSE_REPEATEDLY, AI_STRATEGY.DIFF_ORDER2, AI_STRATEGY.DIFF_ORDER3, AI_STRATEGY.DIFF_ORDER4 };

  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.CHOOSE_REPEATEDLY, AI_STRATEGY.DIFF_ORDER2, AI_STRATEGY.DIFF_ORDER5, AI_STRATEGY.DIFF_ORDER6 };

  static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.GAME_EVAL, AI_STRATEGY.CHOOSE_REPEATEDLY, AI_STRATEGY.DIFF_ORDER2, AI_STRATEGY.DIFF_ORDER6 };

  //static AI_STRATEGY[] playerStrategies = { AI_STRATEGY.QUICK_SELL, AI_STRATEGY.BUILD_MASTER, AI_STRATEGY.NETWORK_MASTER, AI_STRATEGY.QUICK_DEV };





  /// <summary>
  /// Each players strategy script
  /// </summary>
  static AIBehaviour[] strategiesPerPlayer = new AIBehaviour[4];

  /// <summary>
  /// Possible fixed actionOrders
  /// </summary>
  static List<ACTION[]> actionOrders = new()
  {
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.NETWORK, ACTION.DEVELOP, ACTION.SCOUT, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.NETWORK, ACTION.SCOUT, ACTION.DEVELOP, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.DEVELOP, ACTION.NETWORK, ACTION.SCOUT, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.DEVELOP, ACTION.SCOUT, ACTION.NETWORK, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.SCOUT, ACTION.DEVELOP, ACTION.NETWORK, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.BUILD, ACTION.SCOUT, ACTION.NETWORK, ACTION.DEVELOP, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.NETWORK, ACTION.BUILD, ACTION.DEVELOP, ACTION.SCOUT, ACTION.LOAN },
    new ACTION[] { ACTION.SELL, ACTION.NETWORK, ACTION.BUILD, ACTION.SCOUT, ACTION.DEVELOP, ACTION.LOAN }
  };


  void Start()                                
  {                                           
    playFreely = Constants.initAIFreePlay;    
    LoadPlayerStrategies();                   
  }                                           

  private void Awake()
  {
  }
  private void Update()
  {
    //Debug.Log("AIManager update");
    if (!isPlaying || GameManager.waitingForNextEra || !GameManager.GetActivePlayer().AIReplaced) return;
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
    //Debug.Log("LoadPlayerStrategies called");

    //Give each player a script based on their given strategy
    for (int i = 0; i < GameManager.numOfPlayers; i++)
    {
      Player player = GameManager.GetPlayer(i);
      if (!player.AIReplaced) continue;
      switch (playerStrategies[i])
      {
        case AI_STRATEGY.CHOOSE_FIRST:
          strategiesPerPlayer[i] = new AIBehaviour(GameManager.GetPlayer(i));
          break;
        case AI_STRATEGY.CHOOSE_BETTER:
          strategiesPerPlayer[i] = new AIChooseBetter(GameManager.GetPlayer(i));
          break;
        case AI_STRATEGY.CHOOSE_REPEATEDLY:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          break;
        case AI_STRATEGY.DIFF_ORDER2:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[1];
          break;
        case AI_STRATEGY.DIFF_ORDER3:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[2];
          break;
        case AI_STRATEGY.DIFF_ORDER4:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[3];
          break;
        case AI_STRATEGY.DIFF_ORDER5:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[4];
          break;
        case AI_STRATEGY.DIFF_ORDER6:
           strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[5];
          break;
        case AI_STRATEGY.DIFF_ORDER7:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[6];
          break;
        case AI_STRATEGY.DIFF_ORDER8:
          strategiesPerPlayer[i] = new AIChooseRepeatedly(GameManager.GetPlayer(i));
          strategiesPerPlayer[i].actionsOrder = actionOrders[7];
          break;
        case AI_STRATEGY.GAME_EVAL:
          AIGameStateEvaluation basic = new AIGameStateEvaluation(GameManager.GetPlayer(i));
          basic.SetValuesForStrategy(STATE_EVAL_SETTING.BASIC);

          strategiesPerPlayer[i] = basic;
          break;
        case AI_STRATEGY.QUICK_SELL:
          AIGameStateEvaluation quickSell = new AIGameStateEvaluation(GameManager.GetPlayer(i));
          quickSell.SetValuesForStrategy(STATE_EVAL_SETTING.QUICK_SELL);
          strategiesPerPlayer[i] = quickSell;
          break;
        case AI_STRATEGY.BUILD_MASTER:
          AIGameStateEvaluation buildMaster = new AIGameStateEvaluation(GameManager.GetPlayer(i));
          buildMaster.SetValuesForStrategy(STATE_EVAL_SETTING.BUILD_MASTER);
          strategiesPerPlayer[i] = buildMaster;
          break;
        case AI_STRATEGY.NETWORK_MASTER:
          AIGameStateEvaluation netMaster = new AIGameStateEvaluation(GameManager.GetPlayer(i));
          netMaster.SetValuesForStrategy(STATE_EVAL_SETTING.NETWORK_MASTER);
          strategiesPerPlayer[i] = netMaster;
          break;
        case AI_STRATEGY.QUICK_DEV:
          AIGameStateEvaluation quickDev = new AIGameStateEvaluation(GameManager.GetPlayer(i));
          quickDev.SetValuesForStrategy(STATE_EVAL_SETTING.QUCIK_DEV);
          strategiesPerPlayer[i] = quickDev;
          break;
        default:
          break;
      }
    }
  }


  public static void StopPlaying()
  {
    isPlaying = false;
    //if (Camera.main.TryGetComponent(out CameraScript camera))
    //  camera.lockMainBoard = false;
  }
  public static AI_STRATEGY GetPlayerStrategy(int playerIndex) => playerStrategies[playerIndex];

  /// <summary>
  /// Make AI start turn
  /// </summary>
  public static void PlayTurn()
  {
    //Debug.Log("AI Playing turn");
    isPlaying = true;

    //if (Camera.main.TryGetComponent<CameraScript>(out CameraScript camera))
    //{
    //  camera.MoveToMainBoard();
    //  camera.lockMainBoard = true;
    //}
    AIBehaviour strategy = strategiesPerPlayer[GameManager.activePlayerIndex];
    if(strategy is null)
    {
      //Debug.Log("Strategies not yet initialized");
      return;
    }
    strategy.StartTurn();
  }

  /// <summary>
  /// Informs AI about an action being done successfully
  /// </summary>
  public static void ActionWasDone()
  {
    if (!isPlaying) return;
    if(!ActionManager.PlayerDoneAllActions())
      strategiesPerPlayer[GameManager.activePlayerIndex].StartTurn();
    else
    {

      if (Camera.main.TryGetComponent(out CameraScript camera) && !Constants.initMainBoardLock)
        camera.lockMainBoard = false;
      StopPlaying();      
      //Debug.Log("Calling EndTurn from AI -> Done all actions");
      GameManager.EndTurn();
    }
  }

  /// <summary>
  /// Informs AI that action is coming to a successful end
  /// </summary>
  /// <param name="actionCanceled"></param>
  public static void ActionAboutToEndSuccessfuly(out bool actionCanceled)
  {
    if (!isPlaying) { actionCanceled = false; return; }
    strategiesPerPlayer[GameManager.activePlayerIndex].ActionAboutToBeDone(out bool canceled);
    actionCanceled = canceled;
  }

  /// <summary>
  /// Make AI do anything that is now desired (Choose action, card, tile ...)
  /// </summary>
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
public enum AI_STRATEGY { CHOOSE_FIRST, CHOOSE_BETTER, CHOOSE_REPEATEDLY,
  DIFF_ORDER2, DIFF_ORDER3, DIFF_ORDER4, DIFF_ORDER5, DIFF_ORDER6, DIFF_ORDER7,
  DIFF_ORDER8, GAME_EVAL, QUICK_SELL, BUILD_MASTER, NETWORK_MASTER, QUICK_DEV}