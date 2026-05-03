using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup subtitleGroup;

    void Start()
    {
        if (subtitleGroup != null)
            subtitleGroup.alpha = 0f;
    }

    public IEnumerator ShowLine(string line, float typingSpeed, float duration)
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, 0.3f));

        // Type out line
        if (dialogueText != null)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Hold on screen
        yield return new WaitForSeconds(duration);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, 0.3f));
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (subtitleGroup == null) yield break;

        float elapsed = 0f;
        subtitleGroup.alpha = from;

        while (elapsed < duration)
        {
            subtitleGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        subtitleGroup.alpha = to;
    }

    public void HideImmediate()
    {
        if (subtitleGroup != null)
            subtitleGroup.alpha = 0f;
        if (dialogueText != null)
            dialogueText.text = "";
    }
}