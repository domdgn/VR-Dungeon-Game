using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectScanner : MonoBehaviour
{
  private ObjectInformation raycastObj;
  RaycastHit hit;
  [SerializeField] TextMeshProUGUI itemValue;

  void FixedUpdate()
  {
    Scanner();
  }

  void Scanner()
    {
      if(Input.GetKeyDown(KeyCode.Backspace))
      {
        Debug.DrawLine(transform.position, transform.forward * 100f, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
          raycastObj = hit.collider.gameObject.GetComponent<ObjectInformation>();

          if (raycastObj != null)
          {
          var value = raycastObj.GetValue();
          Debug.Log("Value is " + value );
          itemValue.text = value.ToString("F0");
          }
          else
          {
            Debug.Log("I only gamble with your life, never my money");
            itemValue.text = "No...";
          }
        }
      } 
    }
}

