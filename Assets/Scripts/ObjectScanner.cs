using UnityEngine;
using TMPro;

// This script is responsible for scanning objects in front of a specified origin using a raycast.
// If the object contains ObjectInformation, it displays its value using a TextMeshPro UI element.
public class ObjectScanner : MonoBehaviour
{
    private ObjectInformation raycastObj; // Stores reference to the last scanned ObjectInformation
    RaycastHit hit; // Used to store information about what the raycast hits

    [SerializeField] TextMeshProUGUI itemValue; // UI text element to display the scanned object's value
    [SerializeField] Transform raycastOrigin; // The point from which the ray is cast (usually the player's camera or a scanner)
    [SerializeField] bool showRaycastLine = true; // Whether to visually show the raycast in the Scene view (for debugging)

    private void Awake()
    {
        // Ensure this GameObject is not parented to anything at runtime (optional behavior)
        transform.SetParent(null);
    }

    // Call this method to scan forward from the raycastOrigin and detect objects with ObjectInformation
    public void Scanner()
    {
        // Check if the raycast origin has been assigned
        if (raycastOrigin == null)
        {
            Debug.LogError("Raycast Origin not assigned!");
            return;
        }

        // Perform a raycast from the raycastOrigin forward
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit))
        {
            // Optionally draw a debug line to show the raycast hit in the editor
            if (showRaycastLine)
            {
                Debug.DrawLine(raycastOrigin.position, hit.point, Color.red, 0.2f);
            }

            // Try to get the ObjectInformation component from the hit object
            raycastObj = hit.collider.gameObject.GetComponent<ObjectInformation>();
            if (raycastObj != null)
            {
                // If found, display the value in the UI
                var value = raycastObj.GetValue();
                Debug.Log("Value is " + value);
                itemValue.text = value.ToString("F0"); // Display value as an integer
            }
            else
            {
                // If no ObjectInformation was found, log and show a default message
                Debug.Log("I only gamble with your life, never my money"); // (Humorous placeholder line)
                itemValue.text = "No...";
            }
        }
    }

    // Continuously draw a ray in the Scene view for debugging if enabled
    private void Update()
    {
        if (showRaycastLine && raycastOrigin != null)
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * 10f, Color.green, Time.deltaTime);
        }
    }
}