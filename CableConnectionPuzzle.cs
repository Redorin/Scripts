// File: CableConnectionPuzzle.cs
using UnityEngine;

[RequireComponent(typeof(HoldableItem))]
public class CableConnectionPuzzle : MonoBehaviour
{
    [Header("Cable Info")]
    public string cableName = "Socket A";
    public string cableID   = "CableA";

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
    private Collider col;

    void Start()
    {
        holdable = GetComponent<HoldableItem>();
        col      = GetComponent<Collider>();

        if (socketRenderer == null)
            socketRenderer = GetComponent<Renderer>();

        UpdateVisual();

        // Burnt sockets can't be picked up until restored
        // Disable both HoldableItem component AND collider
        if (isBurnt)
        {
            if (holdable != null) holdable.enabled = false;
            if (col != null)      col.enabled      = false;
        }
    }

    // Called by BurntCableResettable when Reset Device restores it
    public void Restore()
    {
        isBurnt = false;

        if (holdable != null) holdable.enabled = true;
        if (col != null)      col.enabled      = true;

        UpdateVisual();

        Chapter1Objectives.Instance?.Complete_ResetBurntSocket();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(cableName + " restored. Pick it up and plug it in.");
    }

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

        // Remove from player inventory
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            ItemHolder itemHolder = player.GetComponent<ItemHolder>();
            if (itemHolder != null)
                itemHolder.RemoveItemByName(holdable.itemName);
        }

        // Snap to plug slot
        Transform slot = wallPlug.plugSlot != null ? wallPlug.plugSlot : wallPlug.transform;
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (holdable != null) holdable.enabled = false;
        if (col != null)      col.enabled      = false;

        UpdateVisual();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(cableName + " plugged in.");

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