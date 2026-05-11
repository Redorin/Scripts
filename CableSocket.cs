using UnityEngine;

// Attach to WallPlugA and WallPlugB.
// When player presses E while holding the matching socket, it connects.

public class CableSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    public string requiredCableID = "CableA";
    public bool isFilled = false;

    [Header("Visual")]
    public Renderer plugRenderer;
    public Color emptyColor  = Color.red;
    public Color filledColor = Color.green;

    private ItemHolder itemHolder;

    void Start()
    {
        if (plugRenderer == null)
            plugRenderer = GetComponent<Renderer>();

        UpdateVisual();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            itemHolder = player.GetComponent<ItemHolder>();
    }

    // Called by PlayerInteraction when player presses E on this wall plug
    public void Interact()
    {
        if (isFilled)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Socket already connected.");
            return;
        }

        if (itemHolder == null) return;

        // Check if player is holding the matching socket
        CableConnectionPuzzle heldSocket = FindHeldMatchingSocket();

        if (heldSocket != null)
        {
            heldSocket.ConnectToPlug(this);
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "No compatible socket in hand. Required: " + requiredCableID);
        }
    }

    CableConnectionPuzzle FindHeldMatchingSocket()
    {
        if (itemHolder == null) return null;

        for (int i = 0; i < itemHolder.GetInventoryCount(); i++)
        {
            string name = itemHolder.GetItemNameAtIndex(i);

            // Find the actual held object — check holdpoint children
            if (itemHolder.holdPoint != null)
            {
                foreach (Transform child in itemHolder.holdPoint)
                {
                    CableConnectionPuzzle socket =
                        child.GetComponent<CableConnectionPuzzle>();
                    if (socket != null && socket.cableID == requiredCableID)
                        return socket;
                }
            }
        }
        return null;
    }

    public void Fill(CableConnectionPuzzle socket)
    {
        isFilled = true;
        UpdateVisual();
        Debug.Log(gameObject.name + " filled by " + socket.cableID);
    }

    void UpdateVisual()
    {
        if (plugRenderer != null)
            plugRenderer.material.color = isFilled ? filledColor : emptyColor;
    }
}