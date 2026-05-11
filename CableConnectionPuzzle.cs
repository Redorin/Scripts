using UnityEngine;

// Attach to SocketA and SocketB.
// Player picks up the socket (HoldableItem) and presses E near a WallPlug to connect.
// Burnt sockets must be reset with Reset Device first.

[RequireComponent(typeof(HoldableItem))]
public class CableConnectionPuzzle : MonoBehaviour
{
    [Header("Cable Info")]
    public string cableName = "Socket A";
    public string cableID   = "CableA";     // Must match WallPlug's requiredCableID

    [Header("Burnt Settings")]
    public bool isBurnt = false;
    public string burntMessage = "Socket is damaged. Use the Reset Device to restore it.";

    [Header("State")]
    public bool isConnected = false;

    [Header("Visual")]
    public Renderer socketRenderer;
    public Color normalColor    = Color.white;
    public Color burntColor     = new Color(0.2f, 0.1f, 0f);
    public Color connectedColor = Color.green;

    private HoldableItem holdable;

    void Start()
    {
        holdable = GetComponent<HoldableItem>();

        if (socketRenderer == null)
            socketRenderer = GetComponent<Renderer>();

        UpdateVisual();

        // Burnt sockets can't be picked up until restored
        if (isBurnt && holdable != null)
            holdable.enabled = false;
    }

    // Called by BurntCableResettable when Reset Device restores it
    public void Restore()
    {
        isBurnt = false;

        if (holdable != null)
            holdable.enabled = true;

        UpdateVisual();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(
                cableName + " restored. Pick it up and plug it in.");
    }

    // Called by PlayerInteraction when player presses E near a WallPlug while holding this
    public void ConnectToPlug(CableSocket wallPlug)
    {
        if (isConnected)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(cableName + " already connected.");
            return;
        }

        if (isBurnt)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(burntMessage);
            return;
        }

        isConnected = true;
        wallPlug.Fill(this);

        // Snap socket to wall plug and detach from player
        transform.SetParent(wallPlug.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable holdable — now fixed in wall
        if (holdable != null)
        {
            holdable.Drop();
            holdable.enabled = false;
        }

        // Disable collider so player doesn't re-interact
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        UpdateVisual();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(cableName + " plugged in.");

        // Notify manager
        CableConnectionPuzzleManager manager =
            FindFirstObjectByType<CableConnectionPuzzleManager>();
        if (manager != null)
            manager.OnCableConnected();
    }

    void UpdateVisual()
    {
        if (socketRenderer == null) return;
        if (isConnected)  socketRenderer.material.color = connectedColor;
        else if (isBurnt) socketRenderer.material.color = burntColor;
        else              socketRenderer.material.color = normalColor;
    }
}