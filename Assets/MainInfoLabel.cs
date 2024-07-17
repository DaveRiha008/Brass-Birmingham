using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainInfoLabel : MonoBehaviour
{
  TextMeshProUGUI myTM;
  const float maxShowTime = 5;
  

  // Start is called before the first frame update
  void Start()
  {
    myTM = GetComponent<TextMeshProUGUI>();
    myTM.alpha = 0;
  }

  // Update is called once per frame
  void Update()
  {
    
  }


  void Show()
  {
    myTM.DOKill();
    myTM.alpha = 1;
    myTM.DOFade(0, maxShowTime);


  }
  public void SuccesfulBuild()
  {
    myTM.text = Constants.succesfulBuildText;
    Show();
  }
  public void SuccesfulSell()
  {
    myTM.text = Constants.succesfulSellText;
    Show();
  }
  public void SuccesfulLoan()
  {
    myTM.text = Constants.succesfulLoanText;
    Show();
  }
  public void SuccesfulScout()
  {
    myTM.text = Constants.succesfulScoutText;
    Show();
  }
  public void SuccesfulDevelop()
  {
    myTM.text = Constants.succesfulDevelopText;
    Show();
  }
  public void SuccesfulNetwork()
  {
    myTM.text = Constants.succesfulNetworkText;
    Show();
  }

  public void PlayerChanged()
  {
    myTM.text = Constants.playerChangedText + GameManager.GetActivePlayer().name;
    Show();
  }

  public void ActionCanceled()
  {
    myTM.text = Constants.actionCanceledText;
    Show();
  }

}
