using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketScaleController : MonoBehaviour
{
    public float scaleSpeed = 5f;

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnItemInserted);
        socket.selectExited.AddListener(OnItemRemoved);
    }

    void OnItemInserted(SelectEnterEventArgs args)
    {
        Transform item = args.interactableObject.transform;
        ShrinkableItem shrinkable = item.GetComponent<ShrinkableItem>();

        if (shrinkable != null)
        {
            shrinkable.StoreOriginalScale();
            Vector3 targetScale = shrinkable.GetShrunkenScale();

            StopAllCoroutines();
            StartCoroutine(ScaleItem(item, item.localScale, targetScale));
        }
    }

    void OnItemRemoved(SelectExitEventArgs args)
    {
        Transform item = args.interactableObject.transform;
        ShrinkableItem shrinkable = item.GetComponent<ShrinkableItem>();

        if (shrinkable != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleItem(item, item.localScale, shrinkable.originalScale));
        }
    }

    System.Collections.IEnumerator ScaleItem(Transform item, Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            item.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        item.localScale = to;
    }
}

