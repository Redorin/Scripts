// File: LightSwitch.cs
// ============================================================================
// Attach to a LightSwitch GameObject.
// Player presses E to toggle lights in the assigned room.
// Supports visual swap between on/off switch states.
// Also supports one-time switches and locked state.
// ============================================================================
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Lights")]
    [Tooltip("All lights this switch controls")]
    public Light[] lights;

    [Header("Visuals")]
    [Tooltip("GameObject shown when switch is ON")]
    public GameObject switchOnVisual;
    [Tooltip("GameObject shown when switch is OFF")]
    public GameObject switchOffVisual;

    [Header("Settings")]
    public bool startsOn = false;
    public bool isLocked = false;
    public bool oneTimeUse = false;

    [Header("Dialogue")]
    public string lockedMessage = "Switch is unresponsive.";
    public string turnOnMessage = "";
    public string turnOffMessage = "";

    [Header("State")]
    public bool isOn = false;

    private bool hasBeenUsed = false;

    void Start()
    {
        isOn = startsOn;
        ApplyLightState();
        UpdateVisual();
    }

    // Called by PlayerInteraction when player presses E
    public void Interact()
    {
        if (isLocked)
        {
            if (AdminDialogue.Instance != null && !string.IsNullOrEmpty(lockedMessage))
                AdminDialogue.Instance.AdminInfo(lockedMessage);
            return;
        }

        if (oneTimeUse && hasBeenUsed) return;

        Toggle();
        hasBeenUsed = true;
    }

    public void Toggle()
    {
        isOn = !isOn;
        ApplyLightState();
        UpdateVisual();

        if (AdminDialogue.Instance != null)
        {
            string msg = isOn ? turnOnMessage : turnOffMessage;
            if (!string.IsNullOrEmpty(msg))
                AdminDialogue.Instance.AdminInfo(msg);
        }
    }

    public void TurnOn()
    {
        if (isOn) return;
        isOn = true;
        ApplyLightState();
        UpdateVisual();
    }

    public void TurnOff()
    {
        if (!isOn) return;
        isOn = false;
        ApplyLightState();
        UpdateVisual();
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    void ApplyLightState()
    {
        foreach (Light l in lights)
            if (l != null) l.enabled = isOn;
    }

    void UpdateVisual()
    {
        if (switchOnVisual  != null) switchOnVisual.SetActive(isOn);
        if (switchOffVisual != null) switchOffVisual.SetActive(!isOn);
    }
}