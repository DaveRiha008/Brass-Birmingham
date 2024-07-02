using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumOfPlayersText : MonoBehaviour
{
  //public string coreText = "Number Of Players : ";
  public string coreText = "Poèet Hráèù : ";
  public TextMeshProUGUI textMesh;
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    textMesh.text = coreText + GameManager.numOfPlayers.ToString();
  }
}
