using UnityEngine;

public class CompassPoint : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform wristTransform;
    [SerializeField] private float angleOffset = 0f;

    private void Start()
    {
        if (wristTransform == null)
        {
            // Try to find the wrist transform in the hierarchy
            wristTransform = transform.parent.parent.parent;
        }
    }

    private void Update()
    {
        if (targetObject == null || wristTransform == null) return;

        // Get world positions
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 wristPosition = wristTransform.position;

        // Direction from wrist to target in world space
        Vector3 directionToTarget = targetPosition - wristPosition;

        // Convert to the UI's local space by getting its world-to-local matrix
        Vector3 localDirection = transform.parent.InverseTransformDirection(directionToTarget);

        // Get the 2D angle in the local XY plane
        float angle = Mathf.Atan2(localDirection.x, localDirection.y) * Mathf.Rad2Deg;

        // Rotate only around Z axis
        transform.localRotation = Quaternion.Euler(0, 0, -angle + angleOffset);
    }
}