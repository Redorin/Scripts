using UnityEngine;
using System.Collections;

// Attach to any object that should visually glitch in Chapter 3.
// Works on Renderers (material color flicker) and Lights (intensity flicker).
// No shader required - pure script based.

public class GlitchFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minInterval = 0.05f;
    public float maxInterval = 0.3f;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;

    [Header("Color Glitch")]
    public bool glitchColor = true;
    public Color normalColor = Color.white;
    public Color glitchColor1 = Color.red;
    public Color glitchColor2 = Color.cyan;

    [Header("Light Flicker")]
    public bool glitchLight = true;
    public Light targetLight;

    [Header("Position Glitch")]
    public bool glitchPosition = false;
    public float positionGlitchAmount = 0.05f;

    [Header("State")]
    public bool isGlitching = false;

    private Renderer rend;
    private Vector3 originalPosition;
    private float originalLightIntensity;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalPosition = transform.localPosition;

        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetLight != null)
            originalLightIntensity = targetLight.intensity;
    }

    public void StartGlitch()
    {
        if (isGlitching) return;
        isGlitching = true;
        StartCoroutine(GlitchLoop());
    }

    public void StopGlitch()
    {
        isGlitching = false;

        // Restore
        if (rend != null)
            rend.material.color = normalColor;

        if (targetLight != null)
            targetLight.intensity = originalLightIntensity;

        transform.localPosition = originalPosition;
    }

    IEnumerator GlitchLoop()
    {
        while (isGlitching)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            if (!isGlitching) break;

            // Color glitch
            if (glitchColor && rend != null)
            {
                int roll = Random.Range(0, 3);
                if (roll == 0) rend.material.color = normalColor;
                else if (roll == 1) rend.material.color = glitchColor1;
                else rend.material.color = glitchColor2;
            }

            // Light flicker
            if (glitchLight && targetLight != null)
            {
                targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            }

            // Position micro-stutter
            if (glitchPosition)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-positionGlitchAmount, positionGlitchAmount),
                    Random.Range(-positionGlitchAmount, positionGlitchAmount),
                    0f
                );
                transform.localPosition = originalPosition + offset;
            }
        }
    }
}