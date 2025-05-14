using UnityEngine;

public class HolsterFollowHeadset : MonoBehaviour
{
    public Transform headset;

    private float initialYPosition;
    private Vector3 initialRelativePosition;
    private Quaternion initialRelativeRotation;

    void Start()
    {
        // Store the initial Y position
        initialYPosition = transform.position.y;

        // Calculate the initial relative position from headset
        initialRelativePosition = Quaternion.Inverse(
            Quaternion.Euler(0f, headset.eulerAngles.y, 0f)) * (transform.position - headset.position);

        // Zero out the Y component since we'll manage that separately
        initialRelativePosition.y = 0;
    }

    void Update()
    {
        // Get the Y rotation of the headset
        Quaternion headsetYRotation = Quaternion.Euler(0f, headset.eulerAngles.y, 0f);

        // Apply headset rotation to the initial relative position
        Vector3 rotatedPosition = headsetYRotation * initialRelativePosition;

        // Create final position by adding to headset position and preserving Y
        Vector3 newPosition = headset.position + rotatedPosition;
        newPosition.y = initialYPosition;

        // Set position
        transform.position = newPosition;

        // Rotate holster to face same Y as headset
        transform.rotation = headsetYRotation;
    }
}