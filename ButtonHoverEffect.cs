using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public TextMeshProUGUI buttonText;

    [Header("Text Settings")]
    public string baseText = "New Session";

    [Header("Colors")]
    public Color normalColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    public Color hoverColor = new Color(0f, 1f, 1f, 1f);

    [Header("Style")]
    public bool addArrow = true;
    public bool boldOnHover = true;

    void Start()
    {
        // Auto find TMP if not assigned
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        ResetButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText == null) return;
        buttonText.text = baseText;
        buttonText.color = hoverColor;
        if (boldOnHover)
            buttonText.fontStyle = FontStyles.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonText == null) return;
        buttonText.color = new Color(0f, 0.7f, 0.7f, 1f);
    }

    void ResetButton()
    {
        if (buttonText == null) return;
        buttonText.text = baseText;
        buttonText.color = normalColor;
        if (boldOnHover)
            buttonText.fontStyle = FontStyles.Normal;
    }
}