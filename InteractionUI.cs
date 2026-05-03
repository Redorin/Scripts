using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;
    
    [Header("Settings")]
    public float displayDuration = 3f; // How long popup stays visible
    
    private float displayTimer = 0f;
    private bool isShowingPopup = false;

    void Start()
    {
        // Hide popup at start
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Auto-hide popup after duration
        if (isShowingPopup)
        {
            displayTimer += Time.deltaTime;
            
            if (displayTimer >= displayDuration)
            {
                HidePopup();
            }
        }
    }

    public void ShowPopup(string message)
    {
        if (popupPanel != null && popupText != null)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
            isShowingPopup = true;
            displayTimer = 0f;
        }
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            isShowingPopup = false;
            displayTimer = 0f;
        }
    }
}