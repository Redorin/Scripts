using UnityEngine;
using System.Collections;

public class FuseBoxPuzzle : MonoBehaviour
{
    [Header("References")]
    public BreakerPanelPuzzle breakerPanel;
    public Light[] roomLights;
    public GameObject fuseSlotEmpty;
    public GameObject fuseSlotFilled;

    [Header("Fuse Settings")]
    // Set this to the Item Name of the working fuse in your scene
    public string validFuseName = "NewFuse";
    // Set this to the Item Name of the old/burnt fuse
    public string oldFuseName = "Fuse";

    [Header("State")]
    public bool hasFuse = false;
    public bool isPowered = false;

    // Other scripts use this to check completion
    public bool isSolved => isPowered;

    [Header("Dialogue")]
    public string noFuseMessage       = "The fuse slot is empty. A replacement fuse is needed.";
    public string oldFuseMessage      = "This fuse is burnt out. It won't restore power.";
    public string fuseInsertedMessage = "New fuse installed. Panel ready for activation.";

    private ItemHolder itemHolder;

    void Start()
    {
        // Try to find ItemHolder directly on any GameObject tagged Player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            itemHolder = playerObj.GetComponent<ItemHolder>();

        // Fallback: find via PlayerInventoryChecker
        if (itemHolder == null)
        {
            PlayerInventoryChecker checker = FindFirstObjectByType<PlayerInventoryChecker>();
            if (checker != null)
                itemHolder = checker.GetComponent<ItemHolder>();
        }

        if (itemHolder == null)
            Debug.LogWarning("[FuseBoxPuzzle] Could not find ItemHolder on player. Make sure Player tag is set.");

        if (fuseSlotFilled != null) fuseSlotFilled.SetActive(false);
        if (fuseSlotEmpty  != null) fuseSlotEmpty.SetActive(true);

        SetLights(false);
    }

    public void Interact()
    {
        if (fusesInserted >= requiredFuses)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("All fuses installed.");
            return;
        }

        // Re-attempt finding ItemHolder in case Start ran too early
        if (itemHolder == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                itemHolder = playerObj.GetComponent<ItemHolder>();
        }

        if (itemHolder == null)
        {
            Debug.LogWarning("[FuseBoxPuzzle] ItemHolder still null on Interact.");
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(noFuseMessage);
            return;
        }

        // Check inventory by iterating — most reliable
        bool hasValid = false;
        bool hasOld   = false;

        int count = itemHolder.GetInventoryCount();
        Debug.Log("[FuseBoxPuzzle] Inventory count: " + count);

        for (int i = 0; i < count; i++)
        {
            string itemName = itemHolder.GetItemNameAtIndex(i);
            Debug.Log("[FuseBoxPuzzle] Inventory slot " + i + ": " + itemName);
            if (itemName == validFuseName) hasValid = true;
            if (itemName == oldFuseName)   hasOld   = true;
        }

        if (hasValid)
        {
            InsertFuse();
            return;
        }

        if (hasOld)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(oldFuseMessage);
            return;
        }

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(noFuseMessage);
    }

    [Header("Fuse Visual")]
    public Transform[] fuseInsertSlots;  // size 2 — one slot per fuse
    public Vector3 fuseInsertLocalPos = Vector3.zero;
    public Vector3 fuseInsertLocalRot = Vector3.zero;

    [Header("Multi-Fuse Settings")]
    public int requiredFuses = 2;

    private int fusesInserted = 0;

    void InsertFuse()
    {
        // Get direct reference BEFORE removing from inventory
        HoldableItem fuseToPlace = itemHolder.GetHoldableItemByName(validFuseName);

        // Now remove from inventory
        itemHolder.RemoveItemByName(validFuseName);

        if (fuseToPlace != null)
        {
            // Kill physics
            Rigidbody rb = fuseToPlace.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            foreach (Collider col in fuseToPlace.GetComponentsInChildren<Collider>(true))
                col.enabled = false;

            fuseToPlace.transform.SetParent(null);

            Transform slot = null;
            if (fuseInsertSlots != null && fusesInserted < fuseInsertSlots.Length)
                slot = fuseInsertSlots[fusesInserted];

            if (slot != null)
            {
                fuseToPlace.transform.SetParent(slot);
                fuseToPlace.transform.localPosition = fuseInsertLocalPos;
                fuseToPlace.transform.localRotation = Quaternion.Euler(fuseInsertLocalRot);
            }

            fuseToPlace.gameObject.SetActive(true);
        }

        fusesInserted++;
        int remaining = requiredFuses - fusesInserted;

        if (fusesInserted >= requiredFuses)
        {
            hasFuse = true;

            if (fuseSlotEmpty  != null) fuseSlotEmpty.SetActive(false);
            if (fuseSlotFilled != null) fuseSlotFilled.SetActive(true);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(fuseInsertedMessage);

            if (breakerPanel != null)
                breakerPanel.SetFuseReady(true);
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "Fuse inserted. " + remaining + " more required.");
        }
    }

    public void PowerOn()
    {
        isPowered = true;
        StartCoroutine(FlickerLightsOn());
    }

    IEnumerator FlickerLightsOn()
    {
        for (int i = 0; i < 4; i++)
        {
            SetLights(true);
            yield return new WaitForSeconds(0.1f);
            SetLights(false);
            yield return new WaitForSeconds(0.1f);
        }
        SetLights(true);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo("Power restored. Systems online.");
    }

    void SetLights(bool on)
    {
        foreach (Light l in roomLights)
            if (l != null) l.enabled = on;
    }
}