using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickThroughHand : Clickable
{
  public CARD_STATE myType = CARD_STATE.IN_HAND;

  public override void OnClick()
  {
    CameraScript camera = Camera.main.GetComponent<CameraScript>();
    switch (myType)
    {
      case CARD_STATE.IN_HAND:
        camera.MoveToCardPreviewHand();
        break;
      case CARD_STATE.DISCARDED:
        camera.MoveToCardPreviewDiscard();
        break;
      default:
        break;
    }
  }
}
