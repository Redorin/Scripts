using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [Header("References")]
    public Image fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration = 2f;
    public float delayBeforeFade = 0.5f;

    void Start()
    {
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 1f;
            fadeOverlay.color = c;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
                fadeOverlay.color = c;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }
}