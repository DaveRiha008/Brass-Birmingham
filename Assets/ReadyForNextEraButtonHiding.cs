using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyForNextEraButtonHiding : MonoBehaviour
{


  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    bool isShown = GameManager.waitingForNextEra;
    transform.Find("EraChangeReady").gameObject.SetActive(isShown); //Fixed object name!!
  }
}
