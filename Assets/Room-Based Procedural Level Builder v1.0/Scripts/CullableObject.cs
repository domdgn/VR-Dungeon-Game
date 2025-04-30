using UnityEngine;

public class CullableObject : MonoBehaviour
{
    public float activationDistance = 20f;

    private void Awake()
    {
        if (DistanceCullingManager.Instance != null)
        {
            DistanceCullingManager.Instance.AddToList(this);
        }
    }
}
