// File: CableSocket.cs
using UnityEngine;

public class CableSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    public string requiredCableID = "CableA";
    public bool isFilled = false;

    [Header("Plug Slot")]
    public Transform plugSlot;

    [Header("Visuals")]
    public GameObject emptyVisual;
    public GameObject filledVisual;

    private ItemHolder itemHolder;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            itemHolder = player.GetComponent<ItemHolder>();

        UpdateVisual();
    }

    public void Interact()
    {
        if (isFilled)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Socket already connected.");
            return;
        }

        if (itemHolder == null) return;

        CableConnectionPuzzle heldSocket = FindHeldMatchingSocket();

        if (heldSocket != null)
            heldSocket.ConnectToPlug(this);
        else
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "No compatible socket in hand. Required: " + requiredCableID);
    }

    CableConnectionPuzzle FindHeldMatchingSocket()
    {
        if (itemHolder == null) return null;
        if (itemHolder.holdPoint == null) return null;

        foreach (Transform child in itemHolder.holdPoint)
        {
            // Must be the ACTIVE item in hand
            HoldableItem holdable = child.GetComponent<HoldableItem>();
            if (holdable == null || !holdable.IsActive()) continue;

            CableConnectionPuzzle socket = child.GetComponent<CableConnectionPuzzle>();
            if (socket != null && socket.cableID == requiredCableID)
                return socket;
        }

        return null;
    }

    public void Fill(CableConnectionPuzzle socket)
    {
        isFilled = true;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (emptyVisual != null)  emptyVisual.SetActive(!isFilled);
        if (filledVisual != null) filledVisual.SetActive(isFilled);
    }
}