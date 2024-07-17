using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{

  public static InputHandler instance;


  private void Awake()
  {
    CreateSingleton();
  }
  void CreateSingleton()
  {
    if (instance == null)
      instance = this;
    else
      Destroy(gameObject);


    DontDestroyOnLoad(gameObject);
  }


  // Update is called once per frame
  void Update()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    if (Input.GetKey(KeyCode.W))
    {
      camera.MoveUp();
    }
    if (Input.GetKey(KeyCode.A))
    {
      camera.MoveLeft();
    }
    if (Input.GetKey(KeyCode.S))
    {
      camera.MoveDown();
    }
    if (Input.GetKey(KeyCode.D))
    {
      camera.MoveRight();
    }
    if (Input.GetKey(KeyCode.Z))
    {
      camera.ZoomIn();
    }
    if (Input.GetKey(KeyCode.H))
    {
      camera.ZoomOut();
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
    {
      camera.ChangeZoom(-1);
    }
    if (Input.GetAxis("Mouse ScrollWheel") < 0f)
    {
      camera.ChangeZoom(1);
    }
    if (Input.GetKeyDown(KeyCode.K))
    {
      camera.MoveToMainBoard();
    }
    if (Input.GetKeyDown(KeyCode.P))
    {
      camera.MoveToPersonalBoard();
    }
    if (Input.GetKeyDown(KeyCode.V))
    {
      camera.MoveToCardPreviewHand();
      //CardManager.PrintAllHands();
    }
    if (Input.GetKeyDown(KeyCode.C))
    {
      camera.MoveToCardPreviewDiscard();
      //CardManager.PrintAllDiscards();
    }
    if (Input.GetKeyDown(KeyCode.N))
    {
      Debug.Log("¨Calling End turn from inputManager!");
      GameManager.EndTurn();
    }
    if(Input.GetKey(KeyCode.Mouse1))
    {
      if (Input.GetAxis("Mouse X") < 0)
      {
        camera.MoveRight(-Input.GetAxis("Mouse X"));
      }
      if (Input.GetAxis("Mouse X") > 0)
      {
        camera.MoveLeft(Input.GetAxis("Mouse X"));
      }
      if (Input.GetAxis("Mouse Y") < 0)
      {
        camera.MoveUp(-Input.GetAxis("Mouse Y"));
      }
      if (Input.GetAxis("Mouse Y") > 0)
      {
        camera.MoveDown(Input.GetAxis("Mouse Y"));
      }
    }
    //Game testing -> comment everything below for user gaming version


    //if (Input.GetKey(KeyCode.X) || Input.GetKeyDown(KeyCode.Y))
    //{
    //  if (SceneManager.GetActiveScene().name == "Game")
    //    AIManager.AIDoNextPart();
    //  else
    //  {
    //    SceneManager.LoadScene("Game");
    //    GameManager.StartGame();
    //    Debug.Log("Game start!");
    //  }

    //}
    //if (Input.GetKeyDown(KeyCode.Tab))
    //  AIManager.playFreely = !AIManager.playFreely;

    //if (Input.GetKeyDown(KeyCode.T))
    //{
    //  camera.lockMainBoard = !camera.lockMainBoard;
    //}

    ////if (Input.GetKeyDown(KeyCode.E))
    ////{
    ////  GameManager.ChangeEra();
    ////}
    //if (Input.GetKeyDown(KeyCode.L)) //Not in final game version - just for testing
    //{
    //  //ObjectManager.CreateAllObjects();
    //  //CardManager.CreateAllCards();
    //  ObjectManager.InitializeObjects();
    //  CardManager.InitializeCards();
    //}
    //if (Input.GetKeyDown(KeyCode.J)) //Not in final game version - just for testing
    //{
    //  Debug.Log("Destroying everything!");
    //  ObjectManager.DestroyAllObjects();
    //  CardManager.DestroyAllCards();
    //}
    //if (Input.GetKeyDown(KeyCode.O)) //Not in final game version - just for testing
    //{
    //  ObjectManager.HighlightCorrectTiles();
    //  ObjectManager.HighlightCorrectNetworkSpaces();
    //  CardManager.HighlightAllCards();
    //}
    //if (Input.GetKeyDown(KeyCode.KeypadPlus))
    //{
    //  //ObjectManager.AddCoalToStorage();
    //  //ObjectManager.AddIronToStorage();

    //  GameManager.ActivePlayerGainMoney(10);
    //  //GameManager.PlayerGainIncome(GameManager.activePlayerIndex);
    //}
    //if (Input.GetKeyDown(KeyCode.KeypadMinus))
    //{
    //  ObjectManager.GetCoalFromStorage(out _).SetActive(false);
    //  ObjectManager.GetIronFromStorage(out _).SetActive(false);

    //  GameManager.ActivePlayerSpendMoney(10, out _);
    //  GameManager.PlayerLoseIncome(GameManager.activePlayerIndex);
    //}
  }
}
