using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
    {
        
    }
  // Update is called once per frame
  void Update()
    {
        
    }

  public void MainMenu()
  {
    SceneManager.LoadScene("Menu");
    Debug.Log("Main menu!");
  }
  public void SetupGame()
  {
    SceneManager.LoadScene("GameSetup");
    Debug.Log("Game setup!");
  }
  public void StartGame()
  {
    SceneManager.LoadScene("Game");
    GameManager.StartGame();
    Debug.Log("Game start!");
  }
  public void QuitGame()
  {
    Application.Quit();
    Debug.Log("Program end!");
  }
  public void Rules()
  {
    //Application.OpenURL("http://files.roxley.com/Brass-Birmingham-Rulebook-2018.11.20-highlights.pdf"); // This URL is currently unavailable - use the link on the next line
    Application.OpenURL("https://www.orderofgamers.com/downloads/BrassBirmingham_v1.2.pdf");
    Debug.Log("Game rules!");
  }

  public void AddPlayer()
  {
    GameManager.AddPlayer();
    Debug.Log("Num of players changed! Current value: " + GameManager.numOfPlayers);
  }
  public void RemovePlayer()
  {
    GameManager.RemovePlayer();
    Debug.Log("Num of players changed! Current value: " + GameManager.numOfPlayers);
  }
  public void AddAI()
  {
    GameManager.AddAI();
    Debug.Log("Num of AI changed! Current value: " + GameManager.numOfAI);
  }
  public void RemoveAI()
  {
    GameManager.RemoveAI();
    Debug.Log("Num of AI changed! Current value: " + GameManager.numOfAI);
  }
  public void MainPersonal()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    if (GameManager.currentlyOnBoard == BOARD.MAIN || GameManager.currentlyOnBoard == BOARD.DISCARD || GameManager.currentlyOnBoard == BOARD.HAND)
      camera.MoveToPersonalBoard();
    else camera.MoveToMainBoard();
    Debug.Log("Moved to main/personal board");
  }
  public void Build()
  {
    ActionManager.DoAction(ACTION.BUILD);
    Debug.Log("Called Build action!");
  }
  public void Sell()
  {
    ActionManager.DoAction(ACTION.SELL);
    Debug.Log("Called Sell action!");
  }
  public void Loan()
  {
    ActionManager.DoAction(ACTION.LOAN);
    Debug.Log("Called Loan action!");
  }
  public void Scout()
  {
    ActionManager.DoAction(ACTION.SCOUT);
    Debug.Log("Called Scout action!");
  }
  public void Develop()
  {
    ActionManager.DoAction(ACTION.DEVELOP);
    Debug.Log("Called Develop action!");
  }
  public void Network()
  {
    ActionManager.DoAction(ACTION.NETWORK);
    Debug.Log("Called Network action!");
  }
  public void CancelAction()
  {
    ActionManager.CancelAction();
    Debug.Log("Called Cancel action!");
  }

  public void DoneAction()
  {
    ActionManager.CancelAction(true);
    Debug.Log("Called Done action!");
  }

  public void HelpBoard()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    camera.MoveToHelpBoard();
    Debug.Log("Camera moved to the help board!");
  }
  public void EndTurn()
  {
    Debug.Log("Calling END TURN from button!");
    GameManager.EndTurn();
  }

  public void NextEraReady()
  {
    GameManager.NextEraReady();
  }

  public void Utility()
  {
    ObjectManager.HighlightNearestFreeCoalSpaces();
  }
  
  public void SaveGame()
  {
    SaveDataManager.SaveJsonData();
  }

  public void LoadGame()
  {
    SaveDataManager.LoadJsonData();
  }
}
