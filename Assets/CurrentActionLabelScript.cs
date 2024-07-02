using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentActionLabelScript : MonoBehaviour
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
    myTM.text = ActionManager.currentAction.ToString();
  }
}
