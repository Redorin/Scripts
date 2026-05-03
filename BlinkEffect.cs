using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkEffect : MonoBehaviour
{
    [Header("Eyelid References")]
    public RectTransform topEyelid;
    public RectTransform bottomEyelid;

    [Header("Screen Settings")]
    public float screenHeight = 1080f;

    [Header("Blink Timing")]
    public float closeSpeed = 0.06f;
    public float holdDuration = 0.05f;
    public float openSpeed = 0.1f;

    [Header("Debug")]
    public bool testBlink = false;

    private bool isBlinking = false;
    private float halfScreen;

    void Start()
    {
        halfScreen = screenHeight / 2f;
        SetEyelidsOpen();
    }

    void Update()
    {
        if (testBlink)
        {
            testBlink = false;
            TriggerBlink();
        }
    }

    public void TriggerBlink()
    {
        if (!isBlinking)
            StartCoroutine(DoBlink(closeSpeed, holdDuration, openSpeed));
    }

    public void TriggerSlowBlink()
    {
        if (!isBlinking)
            StartCoroutine(DoBlink(closeSpeed * 2f, holdDuration * 3f, openSpeed * 2f));
    }

    public void TriggerHeavyBlink()
    {
        if (!isBlinking)
            StartCoroutine(DoBlink(closeSpeed * 1.5f, holdDuration * 5f, openSpeed * 3f));
    }

    IEnumerator DoBlink(float close, float hold, float open)
    {
        isBlinking = true;
        yield return StartCoroutine(MoveEyelids(0f, close));
        yield return new WaitForSeconds(hold);
        yield return StartCoroutine(MoveEyelids(halfScreen, open));
        isBlinking = false;
    }

    IEnumerator MoveEyelids(float targetOffset, float duration)
    {
        if (topEyelid == null || bottomEyelid == null) yield break;

        float startTop = topEyelid.anchoredPosition.y;
        float startBottom = bottomEyelid.anchoredPosition.y;

        // Top eyelid moves DOWN (negative Y) to close
        // Bottom eyelid moves UP (positive Y) to close
        float endTop = targetOffset == 0f ? 0f : halfScreen;
        float endBottom = targetOffset == 0f ? 0f : -halfScreen;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            topEyelid.anchoredPosition = new Vector2(0f,
                Mathf.Lerp(startTop, endTop, t));
            bottomEyelid.anchoredPosition = new Vector2(0f,
                Mathf.Lerp(startBottom, endBottom, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        topEyelid.anchoredPosition = new Vector2(0f, endTop);
        bottomEyelid.anchoredPosition = new Vector2(0f, endBottom);
    }

    void SetEyelidsOpen()
    {
        if (topEyelid != null)
            topEyelid.anchoredPosition = new Vector2(0f, halfScreen);
        if (bottomEyelid != null)
            bottomEyelid.anchoredPosition = new Vector2(0f, -halfScreen);
    }
}