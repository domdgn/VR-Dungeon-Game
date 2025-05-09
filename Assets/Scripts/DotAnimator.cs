using UnityEngine;
using TMPro;
using System.Collections;

public class DotAnimator : MonoBehaviour
{
    private TextMeshProUGUI thisText;
    private string baseText;
    public float interval = 0.25f;

    private void OnEnable()
    {
        thisText = GetComponent<TextMeshProUGUI>();
        baseText = thisText.text;
        StartCoroutine(AnimateDots());
    }

    IEnumerator AnimateDots()
    {
        int dotCount = 0;
        while (true)
        {
            thisText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // cycles 0–3 dots
            yield return new WaitForSeconds(interval);
        }
    }
}
