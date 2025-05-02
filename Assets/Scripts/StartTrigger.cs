using UnityEngine;
using UnityEngine.Events;

public class StartTrigger : MonoBehaviour
{
    public UnityEvent onStartTriggerEnter = new UnityEvent();
    public UnityEvent onStartTriggerExit = new UnityEvent();
    public GameObjectEvent onTreasureTriggerEnter = new GameObjectEvent();
    public GameObjectEvent onTreasureTriggerExit = new GameObjectEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player at spawn");
            onStartTriggerEnter.Invoke();
        }

        if (other.CompareTag("Treasure"))
        {
            Debug.Log("treasure entered trigger");
            onTreasureTriggerEnter.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player NOT at spawn");
            onStartTriggerExit.Invoke();
        }

        if (other.CompareTag("Treasure"))
        {
            Debug.Log("treasure exited trigger");
            onTreasureTriggerExit.Invoke(other.gameObject);
        }
    }
}


[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

