using UnityEngine;

public class ShrinkableItem : MonoBehaviour
{
    [Tooltip("Percentage to scale down when inside backpack (0.3 = 30%)")]
    [Range(0.1f, 1f)]
    public float shrinkPercentage = 0.3f;

    [HideInInspector]
    public Vector3 originalScale;

    private bool scaleStored = false;

    public void StoreOriginalScale()
    {
        if (!scaleStored)
        {
            originalScale = transform.localScale;
            scaleStored = true;
        }
    }

    public Vector3 GetShrunkenScale()
    {
        return originalScale * shrinkPercentage;
    }
}
