using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Clickable : MonoBehaviour
{
  protected bool canBeClicked = true;
  public void BecomeUnclickable() { canBeClicked = false; }
  public void BecomeClickable() { canBeClicked = true; }

  public bool CanBeClicked() => canBeClicked;

  public abstract void OnClick();
}
public class ClickObjectDetectionScript : MonoBehaviour
{
  Camera myCamera;
  // Start is called before the first frame update
  void Start()
  {
    myCamera = Camera.main;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 mousePos = Input.mousePosition;
      Ray myRay = myCamera.ScreenPointToRay(mousePos);
      RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(myRay);

      List<Clickable> toBeClicked = new();

      foreach(RaycastHit2D hit in hits)
      {
        if (hit)
        {

          Clickable clickable;
          bool isClickable = hit.transform.TryGetComponent(out clickable);
          if (isClickable && clickable.CanBeClicked())
          {
            //Debug.Log(hit.transform + "Clicked!");
            toBeClicked.Add(clickable);

            // If we cicked on a Tile -> it should hide everything behind it anyway - no more clicks required
            // this is a bit of a cheat - careful - could be solved better
            if (clickable is TileScript) break;
            //return;
          }
        }
        //else
        //{
        //  Debug.Log("No HIT!");
        //}
      }

      foreach (Clickable obj in toBeClicked)
      {
        obj.OnClick();
        //Debug.Log(obj.transform + "Clicked!");
      }
    }
  }
}
