using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryText : MonoBehaviour
{
  TextMeshProUGUI textMesh;
  // Start is called before the first frame update
  void Start()
  {
    textMesh = GetComponent<TextMeshProUGUI>();
  }

  // Update is called once per frame
  void Update()
  {
    //textMesh.text = "Congratulations " + GameManager.playerWinningOrder[0].name +
    //  "\n\nYou have conquered the industrial revolution and can live happily ever after";
    textMesh.text = "Blahopøejeme " + GameManager.playerWinningOrder[0].name +
      "\n\nDobyl jsi prùmyslovou revoluci a nyní mùžeš žít štastnì až do smrti";
  }
}
