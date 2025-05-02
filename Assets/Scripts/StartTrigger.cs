using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartTrigger : MonoBehaviour
{
    public UnityEvent onStartTriggerEnter = new UnityEvent();
    public UnityEvent onStartTriggerExit = new UnityEvent();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player at spawn");
            onStartTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player NOT at spawn");
            onStartTriggerExit.Invoke();
        }
    }
}
