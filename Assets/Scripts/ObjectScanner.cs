using UnityEngine;
using TMPro;

public class ObjectScanner : MonoBehaviour
{
    private ObjectInformation raycastObj;
    RaycastHit hit;
    [SerializeField] TextMeshProUGUI itemValue;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] bool showRaycastLine = true;

    private void Awake()
    {
        transform.SetParent(null);
    }
    public void Scanner()
    {
        if (raycastOrigin == null)
        {
            Debug.LogError("Raycast Origin not assigned!");
            return;
        }

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit))
        {
            if (showRaycastLine)
            {
                Debug.DrawLine(raycastOrigin.position, hit.point, Color.red, 0.2f);
            }

            raycastObj = hit.collider.gameObject.GetComponent<ObjectInformation>();
            if (raycastObj != null)
            {
                var value = raycastObj.GetValue();
                Debug.Log("Value is " + value);
                itemValue.text = value.ToString("F0");
            }
            else
            {
                Debug.Log("I only gamble with your life, never my money");
                itemValue.text = "No...";
            }
        }
    }
    private void Update()
    {
        if (showRaycastLine && raycastOrigin != null)
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * 10f, Color.green, Time.deltaTime);
        }
    }
}