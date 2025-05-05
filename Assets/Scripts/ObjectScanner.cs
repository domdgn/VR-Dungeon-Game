using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScanner : MonoBehaviour
{
  private ObjectInformation raycastObj;
  RaycastHit hit;
  void FixedUpdate()
  {
    Scanner();
  }

  void Scanner()
    {
      if(Input.GetKeyDown(KeyCode.Backspace))
      {
        Debug.DrawLine(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
          Debug.Log(hit.transform.gameObject.name);
          raycastObj = hit.collider.gameObject.GetComponent<ObjectInformation>();
          var value = raycastObj.GetValue();
          Debug.Log("Value is " + value );
        }
      } 
    }
}

