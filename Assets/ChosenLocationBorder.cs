using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenLocationBorder : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    if(ObjectManager.chosenBuildSpace is null)
      gameObject.GetComponent<SpriteRenderer>().enabled = false;
    else
    {
      gameObject.GetComponent<SpriteRenderer>().enabled = true;
      transform.position = ObjectManager.chosenBuildSpace.transform.position;
    }
  }
}
