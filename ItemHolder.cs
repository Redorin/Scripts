using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ItemHolder : MonoBehaviour
{
    [Header("Hold Point")]
    public Transform holdPoint;

    [Header("Hold Point Offset (from camera)")]
    public Vector3 holdPointOffset = new Vector3(0.35f, -0.3f, 0.6f);

    [Header("Settings")]
    public float pickupDistance = 3f;
    public float throwForce = 10f;
    public int maxItems = 5;

    private List<MonoBehaviour> inventory = new List<MonoBehaviour>();
    private int currentItemIndex = -1;
    private Camera playerCamera;
    private GameObject holdPointObj;

    void Start()
    {
        playerCamera = Camera.main;

        // Always create a fresh HoldPoint at scene root
        // so it NEVER inherits camera scale
        if (holdPoint == null)
        {
            holdPointObj = new GameObject("HoldPoint_Dynamic");
            // ── KEY: parent to scene root, NOT camera ──
            holdPointObj.transform.SetParent(null);
            holdPoint = holdPointObj.transform;
        }
    }

    void Update()
    {
        // ── Manually sync HoldPoint to camera each frame ──
        // This avoids ANY scale inheritance from camera or parents
        if (holdPoint != null && playerCamera != null)
{
    // Force camera scale before calculating position
    playerCamera.transform.localScale = Vector3.one;

    holdPoint.position = playerCamera.transform.TransformPoint(
        holdPointOffset);
    holdPoint.rotation = playerCamera.transform.rotation;
    holdPoint.localScale = Vector3.one;
}
        // Force camera scale to always be 1,1,1
// Prevents stretching of held items
if (playerCamera != null &&
    playerCamera.transform.localScale != Vector3.one)
{
    playerCamera.transform.localScale = Vector3.one;
}

        // Pick up with F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (inventory.Count < maxItems)
                TryPickupItem();
            else
                Debug.Log("Inventory full! (" + maxItems + " items max)");
        }

        // Drop with G key
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
                DropCurrentItem();
        }

        HandleItemSwitching();
    }

    void HandleItemSwitching()
    {
        if (inventory.Count == 0) return;

        int previousIndex = currentItemIndex;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectItem(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectItem(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectItem(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectItem(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectItem(4);
        }

        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll > 0)
            {
                currentItemIndex++;
                if (currentItemIndex >= inventory.Count)
                    currentItemIndex = 0;
            }
            else if (scroll < 0)
            {
                currentItemIndex--;
                if (currentItemIndex < 0)
                    currentItemIndex = inventory.Count - 1;
            }
        }

        if (previousIndex != currentItemIndex)
            UpdateActiveItem();
    }

    void SelectItem(int index)
    {
        if (index < inventory.Count)
        {
            currentItemIndex = index;
            UpdateActiveItem();
        }
    }

    void UpdateActiveItem()
    {
        foreach (var item in inventory)
        {
            if (item is HoldableItem holdable)
                holdable.SetActive(false);
            else if (item is ResetItem reset)
                reset.SetActive(false);
        }

        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            var currentItem = inventory[currentItemIndex];
            string itemName = "";

            if (currentItem is HoldableItem holdable)
            {
                holdable.SetActive(true);
                itemName = holdable.itemName;
            }
            else if (currentItem is ResetItem reset)
            {
                reset.SetActive(true);
                itemName = reset.itemName;
            }

            Debug.Log("Switched to: " + itemName +
                " (Slot " + (currentItemIndex + 1) + ")");
        }
    }

    void TryPickupItem()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            HoldableItem holdable = hit.collider.GetComponent<HoldableItem>();
            if (holdable != null && !holdable.IsBeingHeld())
            {
                AddToInventory(holdable);
                return;
            }

            ResetItem reset = hit.collider.GetComponent<ResetItem>();
            if (reset != null && !reset.IsBeingHeld())
            {
                inventory.Add(reset);
                reset.PickUp(holdPoint);

                if (AdminDialogue.Instance != null &&
                    reset.itemName.Contains("Reset"))
                {
                    AdminDialogue.Instance.AdminInfo(
                        "Rollback device granted.");
                    AdminDialogue.Instance.AdminInfo(
                        "Limited use. Use responsibly.");
                }

                if (inventory.Count == 1)
                {
                    currentItemIndex = 0;
                    reset.SetActive(true);
                }
                else
                {
                    reset.SetActive(false);
                }

                Debug.Log("Added to inventory: " + reset.itemName +
                    " (Slot " + inventory.Count + ")");
            }
        }
    }

    public void AddToInventory(HoldableItem holdable)
    {
        if (inventory.Count >= maxItems)
        {
            Debug.Log("Inventory full! (" + maxItems + " items max)");
            return;
        }

        if (holdable.IsBeingHeld()) return;

        inventory.Add(holdable);
        holdable.PickUp(holdPoint);

        if (inventory.Count == 1)
        {
            currentItemIndex = 0;
            holdable.SetActive(true);
        }
        else
        {
            holdable.SetActive(false);
        }

        Debug.Log("Added to inventory: " + holdable.itemName +
            " (Slot " + inventory.Count + ")");
    }

    void DropCurrentItem()
    {
        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            var item = inventory[currentItemIndex];

            if (item is HoldableItem holdable)
                holdable.Drop();
            else if (item is ResetItem reset)
                reset.Drop();

            inventory.RemoveAt(currentItemIndex);

            if (inventory.Count == 0)
            {
                currentItemIndex = -1;
            }
            else
            {
                currentItemIndex--;
                if (currentItemIndex < 0)
                    currentItemIndex = inventory.Count - 1;
                UpdateActiveItem();
            }
        }
    }

    public bool IsHoldingItem() => inventory.Count > 0;
    public int GetInventoryCount() => inventory.Count;
    public int GetCurrentIndex() => currentItemIndex;

    public string GetCurrentItemName()
    {
        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            var item = inventory[currentItemIndex];
            if (item is HoldableItem holdable) return holdable.itemName;
            else if (item is ResetItem reset) return reset.itemName;
        }
        return "None";
    }

    public string GetItemNameAtIndex(int index)
    {
        if (index < 0 || index >= inventory.Count) return "";
        var item = inventory[index];
        if (item is HoldableItem holdable) return holdable.itemName;
        else if (item is ResetItem reset) return reset.itemName;
        return "";
    }

    void OnDestroy()
    {
        // Clean up dynamic holdpoint on destroy
        if (holdPointObj != null)
            Destroy(holdPointObj);
    }

    public HoldableItem GetHoldableItemByName(string itemName)
{
    foreach (var item in inventory)
    {
        if (item is HoldableItem holdable && holdable.itemName == itemName)
            return holdable;
    }
    return null;
}

public bool RemoveItemByName(string itemName)
{
    for (int i = 0; i < inventory.Count; i++)
    {
        var item = inventory[i];
        string name = "";

        if (item is HoldableItem holdable)
            name = holdable.itemName;
        else if (item is ResetItem reset)
            name = reset.itemName;

        if (name == itemName)
        {
            inventory.RemoveAt(i);

            if (inventory.Count == 0)
                currentItemIndex = -1;
            else
            {
                currentItemIndex = Mathf.Clamp(
                    currentItemIndex, 0, inventory.Count - 1);
                UpdateActiveItem();
            }

            return true;
        }
    }
    return false;
}
}