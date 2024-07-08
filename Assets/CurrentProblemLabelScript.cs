using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CurrentProblemLabelScript : Clickable
{
  TextMeshProUGUI myTM;
  float fadeDuration = 3;
  // Start is called before the first frame update
  void Start()
  {
    myTM = GetComponent<TextMeshProUGUI>();
    myTM.alpha = 0;
  }

  // Update is called once per frame
  void Update()
  {
    string text = myTM.text;
    if (ActionManager.curMisRes != ACTION_MISSING_RESOURCE.NONE)
    {
      myTM.DOKill();
      myTM.alpha = 1;
    }
    switch (ActionManager.curMisRes)
    {
      case ACTION_MISSING_RESOURCE.CARD:
        text = Constants.misResTextCard;
        break;
      case ACTION_MISSING_RESOURCE.WILD_CARD_ALREADY_IN_HAND:
        text = Constants.misResTextWildCardInHand;
        break;
      case ACTION_MISSING_RESOURCE.TILE_BUILD:
        text = Constants.misResTextTileBuild;
        break;
      case ACTION_MISSING_RESOURCE.TILE_SELL:
        text = Constants.misResTextTileSell;
        break;
      case ACTION_MISSING_RESOURCE.TILE_DEVELOP:
        text = Constants.misResTextTileDevelop;
        break;
      case ACTION_MISSING_RESOURCE.SPACE_BUILD:
        text = Constants.misResTextSpaceBuild;
        break;
      case ACTION_MISSING_RESOURCE.NETWORK_SPACE:
        text = Constants.misResTextNetSpace;
        break;
      case ACTION_MISSING_RESOURCE.MONEY_COAL:
        text = Constants.misResTextMoneyCoal;
        break;
      case ACTION_MISSING_RESOURCE.MONEY_IRON:
        text = Constants.misResTextMoneyIron;
        break;
      case ACTION_MISSING_RESOURCE.INCOME_LOAN:
        text = Constants.misResTextIncomeLoan;
        break;
      case ACTION_MISSING_RESOURCE.MONEY_NETWORK:
        text = Constants.misResTextMoneyNetwork;
        break;
      case ACTION_MISSING_RESOURCE.IRON:
        text = Constants.misResTextIron;
        break;
      case ACTION_MISSING_RESOURCE.COAL:
        text = Constants.misResTextCoal;
        break;
      case ACTION_MISSING_RESOURCE.BARREL:
        text = Constants.misResTextBarrel;
        break;
      case ACTION_MISSING_RESOURCE.NONE:
        if(myTM.alpha == 1)
        {
          FadeAway();
        }
        break;
      default:
        break;
    }
    myTM.text = text;
  }

  void FadeAway()
  {
    Debug.Log("Fade called");
    myTM.DOFade(0, fadeDuration);
  }

  public override void OnClick()
  {
    myTM.text = "";
  }
}
