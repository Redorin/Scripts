// File: FuseBoxPuzzle.cs
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
    public string validFuseName = "NewFuse";
    public string oldFuseName   = "Fuse";

    [Header("State")]
    public bool hasFuse    = false;
    public bool isPowered  = false;
    public bool isSolved  => isPowered;

    [Header("Dialogue")]
    public string noFuseMessage       = "The fuse slot is empty. A replacement fuse is needed.";
    public string oldFuseMessage      = "This fuse is burnt out. It won't restore power.";
    public string fuseInsertedMessage = "New fuse installed. Panel ready for activation.";

    [Header("Fuse Visual")]
    public Transform[] fuseInsertSlots;
    public Vector3 fuseInsertLocalPos = Vector3.zero;
    public Vector3 fuseInsertLocalRot = Vector3.zero;

    [Header("Multi-Fuse Settings")]
    public int requiredFuses = 2;

    private ItemHolder itemHolder;
    private int fusesInserted = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            itemHolder = playerObj.GetComponent<ItemHolder>();

        if (itemHolder == null)
        {
            PlayerInventoryChecker checker = FindFirstObjectByType<PlayerInventoryChecker>();
            if (checker != null)
                itemHolder = checker.GetComponent<ItemHolder>();
        }

        if (fuseSlotFilled != null) fuseSlotFilled.SetActive(false);
        if (fuseSlotEmpty  != null) fuseSlotEmpty.SetActive(true);
    }

    public void Interact()
    {
        if (fusesInserted >= requiredFuses)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("All fuses installed.");
            return;
        }

        if (itemHolder == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                itemHolder = playerObj.GetComponent<ItemHolder>();
        }

        if (itemHolder == null)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(noFuseMessage);
            return;
        }

        bool hasValid = false;
        bool hasOld   = false;

        int count = itemHolder.GetInventoryCount();
        for (int i = 0; i < count; i++)
        {
            string itemName = itemHolder.GetItemNameAtIndex(i);
            if (itemName == validFuseName) hasValid = true;
            if (itemName == oldFuseName)   hasOld   = true;
        }

        if (hasValid) { InsertFuse(); return; }
        if (hasOld)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(oldFuseMessage);
            return;
        }

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(noFuseMessage);
    }

    void InsertFuse()
    {
        HoldableItem fuseToPlace = itemHolder.GetHoldableItemByName(validFuseName);
        itemHolder.RemoveItemByName(validFuseName);

        if (fuseToPlace != null)
        {
            Rigidbody rb = fuseToPlace.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic     = true;
                rb.useGravity      = false;
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

        ObjectiveTrigger[] triggers = GetComponents<ObjectiveTrigger>();

        if (fusesInserted >= requiredFuses)
        {
            hasFuse = true;

            if (fuseSlotEmpty  != null) fuseSlotEmpty.SetActive(false);
            if (fuseSlotFilled != null) fuseSlotFilled.SetActive(true);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(fuseInsertedMessage);

            if (breakerPanel != null)
                breakerPanel.SetFuseReady(true);

            // Fire second ObjectiveTrigger — completes insert_fuses, adds reset_conduit
            if (triggers.Length > 1) triggers[1].TriggerObjective();
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Fuse inserted. " + remaining + " more required.");

            // Fire first ObjectiveTrigger — completes find_fuse, adds insert_fuses
            if (triggers.Length > 0) triggers[0].TriggerObjective();
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