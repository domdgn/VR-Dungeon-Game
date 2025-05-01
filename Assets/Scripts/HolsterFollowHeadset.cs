using UnityEngine;

public class HolsterFollowHeadset : MonoBehaviour
{
    public Transform headset;     // Drag your Main Camera here
    public Transform rigRoot;     // Drag your XR Origin or Player here
    public Vector3 offset = new Vector3(0.3f, -0.5f, 0.0f); // Position offset (hip level, right side)

    void Update()
    {
        // Get the Y rotation of the headset
        Quaternion headsetYRotation = Quaternion.Euler(0f, headset.eulerAngles.y, 0f);

        // Apply that rotation to the offset to get correct rotated position
        Vector3 rotatedOffset = headsetYRotation * offset;

        // Set position relative to rig root
        transform.position = rigRoot.position + rotatedOffset;

        // Rotate holster to face same Y as headset
        transform.rotation = headsetYRotation;
    }
}