using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeldItemWindow : MonoBehaviour
{
  Image image;
  // Start is called before the first frame update
  void Start()
  {
    image = this.transform.GetComponent<Image>();
  }

  // Update is called once per frame
  void Update()
  {
    UpdateImage();
  }

  public void UpdateImage()
  {
    if (ObjectManager.itemInHand is not null) image.sprite = ObjectManager.itemInHand.GetComponent<SpriteRenderer>().sprite;
    else image.sprite = null;
  }
}
