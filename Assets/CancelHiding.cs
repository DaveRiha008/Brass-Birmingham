using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script should be attached to HUD object

public class CancelHiding : MonoBehaviour
{

  // Update is called once per frame
  void Update()
  {
    bool isShown = ActionManager.currentAction != ACTION.NONE;
    transform.Find(Constants.cancelButtonName).gameObject.SetActive(isShown); //Fixed object name!!
  }
}
