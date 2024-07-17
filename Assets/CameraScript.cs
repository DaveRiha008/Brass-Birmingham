using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
  /// <summary>
  /// speed with which camera moves through the scene -> set manually in inspector
  /// </summary>
  public int speed = 1;

  /// <summary>
  /// speed with which camera zooms in and out -> set manually in inspector
  /// </summary>
  public int zoomSpeed = 1;

  /// <summary>
  /// Current camera position
  /// </summary>
  public Vector3 position;

  /// <summary>
  /// Default zoom on main board
  /// </summary>
  public const float mainBoardZoom = 5;

  /// <summary>
  /// Default zoom on personal board
  /// </summary>
  public const float personalBoardZoom = 2.8f;

  Camera myCamera;


  float currentResetSize;
  float originalCameraSize;
  float currentCameraSize;

  float originalCameraZ;

  float cameraHalfWidth;
  float cameraHalfHeight;

  float xMaxBoundary = 1000;
  float xMinBoundary = -1000;
  float yMaxBoundary = 1000;
  float yMinBoundary = -1000;

  /// <summary>
  /// Whether camera is locked on main board and can't move to any other
  /// </summary>
  public bool lockMainBoard = false;
  /// <summary>
  /// Whether camera is locken on the current screen and can't move to any other
  /// </summary>
  public bool lockScreenChange = false;



  // Start is called before the first frame update
  void Start()
  {
    myCamera = GetComponent<Camera>();
    originalCameraSize = myCamera.orthographicSize;
    currentResetSize = originalCameraSize;

    originalCameraZ = transform.position.z;

    lockMainBoard = Constants.initMainBoardLock;

    MoveToMainBoard();    
  }

  // Update is called once per frame
  void Update()
  {
    position = transform.position;
    currentCameraSize = myCamera.orthographicSize;
    cameraHalfHeight = myCamera.orthographicSize;
    cameraHalfWidth = myCamera.orthographicSize * (Screen.width/Screen.height);
  }



  // CAMERA  MOVEMENT

  void SetPosition(Vector3 newPos)
  {
    newPos = CorrectPosition(newPos);
    this.transform.position = newPos;
    //Debug.Log("Setting new camera position! Pos: " + this.transform.position.ToString());
  }

  Vector3 CorrectPosition(Vector3 pos)
  {
    Vector3 newPos = pos;
    if (pos.x > xMinBoundary + cameraHalfWidth && pos.x < xMaxBoundary - cameraHalfWidth)
    {
      //Debug.Log("Both X boundaries hit!");
      newPos.x = pos.x; //For low zoom to eliminate flicker
    }
    else newPos.x = Mathf.Clamp(pos.x, xMinBoundary + cameraHalfWidth, xMaxBoundary - cameraHalfWidth);
   
    if (pos.y > yMinBoundary + cameraHalfHeight && pos.y < yMaxBoundary - cameraHalfHeight)
    {
      //Debug.Log("Both Y boundaries hit!");
      newPos.y = pos.y; //For low zoom to eliminate flicker
    }
    else newPos.y = Mathf.Clamp(pos.y, yMinBoundary + cameraHalfHeight, yMaxBoundary - cameraHalfHeight);
    newPos.z = originalCameraZ;
    return newPos;
  }

  public void MoveUp()
  {
    position.y += speed * Time.deltaTime * (currentCameraSize/originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going up!");

  }
  public void MoveRight()
  {
    position.x += speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going right!");
  }
  public void MoveLeft()
  {
    position.x -= speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going left!");
  }
  public void MoveDown()
  {
    position.y -= speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going down!");
  }

  public void MoveUp(float val)
  {
    position.y += val* speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going up!");

  }
  public void MoveRight(float val)
  {
    position.x += val * speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going right!");
  }
  public void MoveLeft(float val)
  {
    position.x -= val * speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going left!");
  }
  public void MoveDown(float val)
  {
    position.y -= val * speed * Time.deltaTime * (currentCameraSize / originalCameraSize);
    SetPosition(position);
    //Debug.Log("Camera going down!");
  }


  // CAMERA  ZOOM


  public void ChangeZoom(float change)
  {
    //Debug.Log("Changing zoom by: " + change);
    float newOrthoSize = myCamera.orthographicSize + change;
    myCamera.orthographicSize = CorrectZoom(newOrthoSize);
  }

  float CorrectZoom(float zoom)
  {
    float newZoom = Mathf.Clamp(zoom, 1, currentResetSize);
    return newZoom;
  }

  public void ZoomIn()
  {
    ChangeZoom(-zoomSpeed * Time.deltaTime);
  }

  public void ZoomOut()
  {
    ChangeZoom(zoomSpeed * Time.deltaTime);
  }
  public void ResetZoom()
  {
    if (myCamera is null) return;
    myCamera.orthographicSize = currentResetSize;
  }

  //Camera moving to exact boards

  void SetBoundaries(Vector3 boardPos, BoxCollider2D collider)
  {
    xMaxBoundary = boardPos.x + collider.size.x / 2;
    xMinBoundary = boardPos.x - collider.size.x / 2;
    yMaxBoundary = boardPos.y + collider.size.y / 2;
    yMinBoundary = boardPos.y - collider.size.y / 2;
  }

  void MoveToBoard(string boardName)
  {
    if (lockMainBoard && boardName != Constants.mainBoardName) return;
    if (lockScreenChange) return;
    GameObject board = GameObject.Find(boardName); 
    Vector3 boardPosition = board.transform.position;
    BoxCollider2D boardCollider = board.transform.Find(Constants.boardBackgroundName).gameObject.GetComponent<BoxCollider2D>();
    SetBoundaries(boardPosition, boardCollider);
    SetPosition(boardPosition);
    ResetZoom();
    //Debug.Log("Moving Camera to main board!");
  }

  public void MoveToMainBoard()
  {
    currentResetSize = mainBoardZoom;
    MoveToBoard(Constants.mainBoardName);
    GameManager.currentlyOnBoard = BOARD.MAIN;
  }

  public void MoveToPersonalBoard()
  {
    currentResetSize = personalBoardZoom;
    string boardName = Constants.playerPersonalBoardNames[GameManager.activePlayerIndex]; 
    MoveToBoard(boardName);
    GameManager.currentlyOnBoard = BOARD.PERSONAL;
  }
  public void MoveToHelpBoard()
  {
    currentResetSize = originalCameraSize;
    MoveToBoard(Constants.helpBoardName); 
    GameManager.currentlyOnBoard = BOARD.HELP;
  }
  public void MoveToCardPreviewHand()
  {
    currentResetSize = originalCameraSize;
    MoveToBoard(Constants.playerHandBoardNames[GameManager.activePlayerIndex]);
    GameManager.currentlyOnBoard = BOARD.HAND;
  }
  public void MoveToCardPreviewDiscard()
  {
    currentResetSize = originalCameraSize;
    MoveToBoard(Constants.playerDiscardBoardNames[GameManager.activePlayerIndex]);
    GameManager.currentlyOnBoard = BOARD.DISCARD;
  }

  public void MoveToChangeEraScreen()
  {
    Debug.Log("Changing camera to EraChange");
    currentCameraSize = originalCameraSize;
    MoveToBoard(Constants.midEraScreenName);
    GameManager.currentlyOnBoard = BOARD.ERA_CHANGE;
  }
}
