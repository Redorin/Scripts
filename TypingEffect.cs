using UnityEngine;
using TMPro;
using System.Collections;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI[] lines;
    public float typingSpeed = 0.05f;
    public float delayBetweenLines = 0.3f;

    void Start()
    {
        foreach (var line in lines)
            line.text = "";
        StartCoroutine(TypeAllLines());
    }

    IEnumerator TypeAllLines()
    {
        string[] fullTexts = {
            "> SYSTEM_SCHISM.exe",
            ">Initializing session...",
            "> WARNING: Integrity fluctuation",
            "> _"
        };

        for (int i = 0; i < lines.Length; i++)
        {
            yield return StartCoroutine(TypeLine(lines[i], fullTexts[i]));
            yield return new WaitForSeconds(delayBetweenLines);
        }
    }

    IEnumerator TypeLine(TextMeshProUGUI textObj, string fullText)
    {
        textObj.text = "";
        foreach (char c in fullText)
        {
            textObj.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}