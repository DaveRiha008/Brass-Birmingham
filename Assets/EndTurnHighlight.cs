using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnHighlight : MonoBehaviour
{
  public GameObject border = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (ActionManager.PlayerDoneAllActions()) Highlight();
    else UnHighlight();
    }

  public void Highlight()
  {
    border.GetComponent<SpriteRenderer>().enabled = true;
  }

  public void UnHighlight()
  {
    border.GetComponent<SpriteRenderer>().enabled = false;

  }
}
