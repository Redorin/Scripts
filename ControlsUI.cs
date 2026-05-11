using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ControlsUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject controlsPanel;
    public TextMeshProUGUI controlsText;

    [Header("Timing")]
    public float fadeInDuration = 0.5f;
    public float displayDuration = 5f;
    public float fadeOutDuration = 0.5f;
    public float delayBetweenSets = 0.3f;

    [Header("Control Sets")]
    [TextArea(3, 6)]
    public string basicControls =
        "WASD — Move\n" +
        "Mouse — Look\n" +
        "E — Interact\n" +
        "ESC — Pause";

    [TextArea(3, 6)]
    public string itemControls =
        "F — Pick Up Object\n" +
        "R — Use Object\n" +
        "G — Drop Object\n" +
        "Scroll — Switch Item";

    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private Coroutine currentCoroutine;

    // Tracked so we only show each set once
    private bool basicShown = false;
    private bool itemShown = false;

    void Start()
    {
        // Get or add CanvasGroup for fading
        canvasGroup = controlsPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = controlsPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        controlsPanel.SetActive(false);
    }

    // ── Called by CutsceneManager after opening cutscene ends ──
    public void ShowBasicControls()
    {
        if (basicShown) return;
        basicShown = true;
        ShowControls(basicControls);
    }

    // ── Called by DoorCutsceneManager after door cutscene ends ──
    public void ShowItemControls()
    {
        if (itemShown) return;
        itemShown = true;

        // If basic controls still showing, wait for them to finish
        if (isShowing)
        {
            StartCoroutine(WaitThenShowItemControls());
        }
        else
        {
            ShowControls(itemControls);
        }
    }

    IEnumerator WaitThenShowItemControls()
    {
        while (isShowing)
            yield return null;

        yield return new WaitForSeconds(delayBetweenSets);
        ShowControls(itemControls);
    }

    void ShowControls(string text)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(DisplayControls(text));
    }

    IEnumerator DisplayControls(string text)
    {
        isShowing = true;

        controlsText.text = text;
        controlsPanel.SetActive(true);

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));

        controlsPanel.SetActive(false);
        isShowing = false;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void HideImmediate()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        canvasGroup.alpha = 0f;
        controlsPanel.SetActive(false);
        isShowing = false;
    }
}