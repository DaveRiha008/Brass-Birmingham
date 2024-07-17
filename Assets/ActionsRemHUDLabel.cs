using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionsRemHUDLabel : MonoBehaviour
{
  TextMeshProUGUI myTM;
  void Start()
  {
    myTM = GetComponent<TextMeshProUGUI>();
  }

  void Update()
  {
    int remActions;
    if (GameManager.firstEverRound)
      remActions = 1 - ActionManager.actionsPlayed;
    else
      remActions = 2 - ActionManager.actionsPlayed;
    myTM.text = "Zbývá \n akcí: \n " + remActions;
  }
}
