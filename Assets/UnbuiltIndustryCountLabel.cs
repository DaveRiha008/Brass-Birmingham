using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnbuiltIndustryCountLabel : MonoBehaviour
{
  TextMeshPro myTMPro;
  public int playerIndex;
  public INDUSTRY_TYPE myType;
  public int level;
  // Start is called before the first frame update
  void Start()
  {
    myTMPro = GetComponent<TextMeshPro>();
  }

  // Update is called once per frame
  void Update()
  {
    myTMPro.text = ObjectManager.GetAllMyUnbuiltTiles(playerIndex, level, myType).Count.ToString();
  }
}
