using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumOfAIText : MonoBehaviour
{
  public string coreText = "Poèet AI : ";
  //public string coreText = "Number Of AI : ";
  public TextMeshProUGUI textMesh;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    textMesh.text = coreText + GameManager.numOfAI.ToString();
  }
}
