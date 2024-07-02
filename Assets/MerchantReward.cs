using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantReward : MonoBehaviour
{
  public MERCHANT_REW_TYPES type = MERCHANT_REW_TYPES.NONE;
  public int amount = 0;
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
        
  }
}

public enum MERCHANT_REW_TYPES { MONEY, VIC_POINTS, INCOME, DEVELOP, NONE }
