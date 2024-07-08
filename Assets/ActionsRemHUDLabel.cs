using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionsRemHUDLabel : MonoBehaviour
{
  TextMeshProUGUI myTM;
    // Start is called before the first frame update
  void Start()
  {
    myTM = GetComponent<TextMeshProUGUI>();
  }

  // Update is called once per frame
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
