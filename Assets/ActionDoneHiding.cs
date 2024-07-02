using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDoneHiding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

  // Update is called once per frame
  void Update()
  {
    bool isShown = ActionManager.IsActionFinishable();
    transform.Find("Done").gameObject.SetActive(isShown); //Fixed object name!!
  }
}
