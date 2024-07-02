using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentActionStateLabelScript : MonoBehaviour
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
    string text = "";
    switch (ActionManager.currentState)
    {
      case ACTION_STATE.CHOOSING_CARD:
        if (ActionManager.currentAction == ACTION.NONE)
          text = Constants.actionStateTextChCardNoAction;
        else
          text = Constants.actionStateTextChCard;
        break;
      case ACTION_STATE.CHOOSING_DECK:
        text = Constants.actionStateTextChDeck;
        break;
      case ACTION_STATE.CHOOSING_TILE:
        text = Constants.actionStateTextChTile;
        break;
      case ACTION_STATE.CHOOSING_SPACE:
        text = Constants.actionStateTextChSpace;
        break;
      case ACTION_STATE.CHOOSING_IRON:
        text = Constants.actionStateTextChIron;
        break;
      case ACTION_STATE.CHOOSING_COAL:
        text = Constants.actionStateTextChCoal;
        break;
      case ACTION_STATE.CHOOSING_BARREL:
        text = Constants.actionStateTextChBarrel;
        break;
      case ACTION_STATE.CHOOSING_NETWORK_SPACE:
        text = Constants.actionStateTextChSpace;
        break;
      case ACTION_STATE.NONE:
        text = "";
        break;
      default:
        break;
    }
    myTM.text = text;
  }
}
