using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
   public float floatSpeed = 0.25f;
   public float duration = 1f;

    // Update is called once per frame
    void Update()
    {
        Float();
        Follow();
    }

    void Float()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Follow()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}
