using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelHiding : MonoBehaviour
{

  // Update is called once per frame
  void Update()
  {
    bool isShown = ActionManager.currentAction != ACTION.NONE;
    transform.Find("Cancel").gameObject.SetActive(isShown); //Fixed object name!!
  }
}
