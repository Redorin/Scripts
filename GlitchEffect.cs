using UnityEngine;
using TMPro;

public class GlitchEffect : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public float glitchInterval = 3f;
    public float glitchDuration = 0.2f;

    private string originalText;
    private float timer;
    private string glitchChars = "!@#$%^&*<>?/|\\[]{}";

    void Start()
    {
        originalText = titleText.text;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= glitchInterval)
        {
            timer = 0f;
            StartCoroutine(DoGlitch());
        }
    }

    System.Collections.IEnumerator DoGlitch()
    {
        float elapsed = 0f;
        while (elapsed < glitchDuration)
        {
            char[] glitched = originalText.ToCharArray();
            for (int i = 0; i < glitched.Length; i++)
            {
                if (Random.value > 0.7f)
                    glitched[i] = glitchChars[Random.Range(0, glitchChars.Length)];
            }
            titleText.text = new string(glitched);
            elapsed += Time.deltaTime;
            yield return null;
        }
        titleText.text = originalText;
    }
}