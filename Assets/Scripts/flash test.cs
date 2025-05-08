using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    public Color flashColor = Color.red; // Choose the flash color in the Inspector
    public float flashDuration = 0.2f; // Duration of the flash
    public float fadeDuration = 0.2f; // How long it takes to fade back to the original color
    private Color originalColor;
    private Renderer renderer;

    void Start()
    {
        // Get the renderer component of the object
        renderer = GetComponent<Renderer>();

        // Store the original color of the material
        if (renderer != null && renderer.material != null)
        {
            originalColor = renderer.material.color;
        }
        else
        {
            Debug.LogError("Renderer or material not found on this GameObject.");
        }

        Flash();
    }

    public void Flash()
    {
        // Start the flash effect
        if (renderer != null && renderer.material != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        // Flash to the chosen color
        renderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        // Fade back to the original color
        Color currentColor = renderer.material.color;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            renderer.material.color = Color.Lerp(flashColor, originalColor, elapsedTime / fadeDuration);
            yield return null;
        }

        // Ensure the color is fully reset
        renderer.material.color = originalColor;
    }
}