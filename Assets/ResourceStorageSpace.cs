using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorageSpace : MonoBehaviour
{
  GameObject myResource = null;
  public int cost = 0;


  public bool HasResource() => myResource is not null;
  public GameObject GetResource() => myResource;
  public void SetResource(GameObject resource)
  {
    myResource = resource;
    myResource.transform.position = transform.position;
  }
  public GameObject RemoveResource()
  {
    GameObject returnResource = myResource;
    myResource = null;
    return returnResource;
  }
  public void DestroyResource()
  {
    myResource.SetActive(false);
    myResource = null;
  }

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
        
  }
}
