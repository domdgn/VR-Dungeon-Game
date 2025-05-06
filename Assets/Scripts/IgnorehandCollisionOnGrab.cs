using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IgnoreHandCollisionOnGrab : MonoBehaviour
{
    private MeshCollider col;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        col = GetComponent<MeshCollider>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        col.enabled = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        col.enabled = true;
    }
}
