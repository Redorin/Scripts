using UnityEngine;

public class ServerTerminal : MonoBehaviour
{
    [Header("Server Info")]
    public string serverName = "Server A";

    [Header("State")]
    public bool isInteractable = false;
    public bool isBooted = false;
    public bool isOnline = false;

    [Header("Visual")]
    public Renderer statusLight;
    public Color offColor     = Color.red;
    public Color bootingColor = Color.yellow;
    public Color onlineColor  = Color.green;

    [Header("Dialogue")]
    public string notReadyMessage  = "Server offline. Connect cables first.";
    public string bootingMessage   = "Initiating boot sequence...";
    public string alreadyOnMessage = "Server already online.";

    private CableConnectionPuzzleManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<CableConnectionPuzzleManager>();
        UpdateVisual(offColor);
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    public void Interact()
    {
        if (!isInteractable)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(notReadyMessage);
            return;
        }

        if (isBooted)
        {
            // Different message if fully online vs just booted in wrong order
            string msg = isOnline
                ? serverName + " — already online."
                : alreadyOnMessage;
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(msg);
            return;
        }

        isBooted = true;
        UpdateVisual(bootingColor);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(bootingMessage);

        if (manager != null)
            manager.OnServerBooted(this);
    }

    // Called when wrong boot order resets sequence
    public void ResetBoot()
    {
        isBooted = false;
        isOnline = false;
        UpdateVisual(offColor);
    }

    // Called by manager when this server is confirmed online
    public void SetOnline()
    {
        isOnline = true;
        UpdateVisual(onlineColor);
    }

    void UpdateVisual(Color color)
    {
        if (statusLight != null)
            statusLight.material.color = color;
    }
}