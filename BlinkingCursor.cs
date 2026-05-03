using UnityEngine;
using TMPro;

public class BlinkingCursor : MonoBehaviour
{
    public TextMeshProUGUI cursorText;
    public float blinkSpeed = 0.5f;
    
    private float timer = 0f;
    private bool isVisible = true;
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= blinkSpeed)
        {
            isVisible = !isVisible;
            cursorText.enabled = isVisible;
            timer = 0f;
        }
    }
}