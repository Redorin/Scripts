using UnityEngine;

public class BreakerSwitch : MonoBehaviour
{
    [Header("Settings")]
    public int breakerIndex = 0;
    public string breakerLabel = "B1";

    [Header("References")]
    public BreakerPanelPuzzle panel;

    [Header("Visual")]
    public GameObject switchUp;
    public GameObject switchDown;

    [Header("State")]
    public bool isFlipped = false;
    public bool isInteractable = false;

    void Start()
    {
        UpdateVisual();
    }

    public void Interact()
    {
        Debug.Log("[BreakerSwitch] Interact called on: " + breakerLabel + " (index " + breakerIndex + ") | isFlipped: " + isFlipped + " | isInteractable: " + isInteractable);

        if (!isInteractable)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "Panel not ready. Check power conduit.");
            return;
        }

        if (isFlipped)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    breakerLabel + " already active.");
            return;
        }

        isFlipped = true;
        UpdateVisual();

        Debug.Log("[BreakerSwitch] Notifying panel: index " + breakerIndex);
        if (panel != null)
            panel.OnBreakerFlipped(breakerIndex);
    }

    public void ResetBreaker()
    {
        isFlipped = false;
        UpdateVisual();
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    void UpdateVisual()
    {
        if (switchUp != null)
            switchUp.SetActive(!isFlipped);
        if (switchDown != null)
            switchDown.SetActive(isFlipped);
    }
}