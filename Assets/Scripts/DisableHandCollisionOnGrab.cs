using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Collections;

public class DisableHandCollisionOnGrab : MonoBehaviour
{
    public string heldLayerName = "GrabbedObjects";
    public string defaultLayerName = "Default";
    public float collisionDelayAfterRelease = 0.2f;

    private XRGrabInteractable grabInteractable;
    private int heldLayer;
    private int defaultLayer;
    private int originalLayer;
    private Coroutine resetLayerCoroutine;
    private Rigidbody rb;

    private void Awake()
    {
        heldLayer = LayerMask.NameToLayer(heldLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);

        originalLayer = gameObject.layer;
        rb = GetComponent<Rigidbody>();

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (resetLayerCoroutine != null)
        {
            StopCoroutine(resetLayerCoroutine);
            resetLayerCoroutine = null;
        }

        if (heldLayer != -1)
        {
            gameObject.layer = heldLayer;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Vector3 releaseVelocity = Vector3.zero;
        if (rb != null)
        {
            releaseVelocity = rb.velocity;
        }

        resetLayerCoroutine = StartCoroutine(ResetLayerAfterDelay(releaseVelocity));
    }

    private IEnumerator ResetLayerAfterDelay(Vector3 releaseVelocity)
    {
        yield return new WaitForSeconds(collisionDelayAfterRelease);

        int targetLayer = defaultLayer != -1 ? defaultLayer : originalLayer;

        gameObject.layer = targetLayer;


        resetLayerCoroutine = null;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }

        if (resetLayerCoroutine != null)
        {
            StopCoroutine(resetLayerCoroutine);
        }
    }
}