using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("References")]
    public Image fadeOverlay;
    public CanvasGroup leftPanelGroup;

    [Header("Timing")]
    public float fadeOutDuration = 5f;

    private bool isTransitioning = false;

    void Start()
    {
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }

        if (leftPanelGroup != null)
            leftPanelGroup.alpha = 1f;
    }

    public void StartTransition(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(FadeOutThenLoad(sceneName));
    }

    IEnumerator FadeOutThenLoad(string sceneName)
    {
        isTransitioning = true;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            float t = elapsed / fadeOutDuration;

            // Fade overlay in
            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                fadeOverlay.color = c;
            }

            // Fade left panel out
            if (leftPanelGroup != null)
                leftPanelGroup.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure everything is fully done
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 1f;
            fadeOverlay.color = c;
        }

        if (leftPanelGroup != null)
            leftPanelGroup.alpha = 0f;

        SceneManager.LoadScene(sceneName);
    }
}