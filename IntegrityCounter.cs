using UnityEngine;
using TMPro;

public class IntegrityCounter : MonoBehaviour
{
    public TextMeshProUGUI integrityText;
    public float decreaseInterval = 2f;
    public int minValue = 85;
    public int maxValue = 97;

    private int currentValue = 97;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= decreaseInterval)
        {
            timer = 0f;
            currentValue--;
            if (currentValue < minValue)
                currentValue = maxValue;
            integrityText.text = "Integrity: " + currentValue + "%";
        }
    }
}