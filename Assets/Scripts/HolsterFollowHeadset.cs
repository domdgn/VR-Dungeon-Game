using UnityEngine;
public class HolsterFollowHeadset : MonoBehaviour
{
    public Transform headset;
    public Transform xrOrigin;
    private float initialYOffsetFromOrigin;
    private Vector3 initialRelativePosition;

    void Start()
    {
        initialYOffsetFromOrigin = transform.position.y - xrOrigin.position.y;

        initialRelativePosition = Quaternion.Inverse(
            Quaternion.Euler(0f, headset.eulerAngles.y, 0f)) * (transform.position - headset.position);

        initialRelativePosition.y = 0;
    }

    void Update()
    {
        Quaternion headsetYRotation = Quaternion.Euler(0f, headset.eulerAngles.y, 0f);
        Vector3 rotatedPosition = headsetYRotation * initialRelativePosition;

        Vector3 newPosition = headset.position + rotatedPosition;
        newPosition.y = xrOrigin.position.y + initialYOffsetFromOrigin;

        transform.position = newPosition;
        transform.rotation = headsetYRotation;
    }
}