using UnityEngine;
using TMPro;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Object Info")]
    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Subtitle Settings")]
    public float typingSpeed = 0.03f;
    public float lineDuration = 3f;

    private static SubtitleUI subtitleUI;
    private bool isReading = false;

    void Start()
    {
        if (subtitleUI == null)
            subtitleUI = FindObjectOfType<SubtitleUI>();
    }

    public void Interact()
    {
        if (isReading) return;
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        isReading = true;

        foreach (string line in dialogueLines)
        {
            if (subtitleUI != null)
                yield return StartCoroutine(
                    subtitleUI.ShowLine(line, typingSpeed, lineDuration)
                );
        }

        isReading = false;
    }
}