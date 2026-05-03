using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ItemHolder : MonoBehaviour
{
    [Header("Hold Point")]
    public Transform holdPoint;
    
    [Header("Settings")]
    public float pickupDistance = 3f;
    public float throwForce = 10f;
    public int maxItems = 5;
    
    [Header("Hotbar")]
    private List<MonoBehaviour> inventory = new List<MonoBehaviour>(); // Can hold both HoldableItem and ResetItem
    private int currentItemIndex = -1;
    
    private Camera playerCamera;
    
    void Start()
    {
        playerCamera = Camera.main;

        if (holdPoint == null)
        {
            GameObject holdPointObj = new GameObject("HoldPoint");
            holdPointObj.transform.SetParent(playerCamera.transform);
            // Right hand position - offset right, down, and forward
            holdPointObj.transform.localPosition = new Vector3(0.4f, -0.35f, 0.6f);
            holdPoint = holdPointObj.transform;
        }
    }
    
    void Update()
    {
        // Pick up item with F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (inventory.Count < maxItems)
            {
                TryPickupItem();
            }
            else
            {
                Debug.Log("Inventory full! (" + maxItems + " items max)");
            }
        }
        
        // Drop current item with G key
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
            {
                DropCurrentItem();
            }
        }
        
        HandleItemSwitching();
    }
    
    void HandleItemSwitching()
    {
        if (inventory.Count == 0) return;
        
        int previousIndex = currentItemIndex;
        
        // Number keys
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectItem(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectItem(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectItem(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectItem(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectItem(4);
        }
        
        // Scroll wheel
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            
            if (scroll > 0)
            {
                currentItemIndex++;
                if (currentItemIndex >= inventory.Count)
                {
                    currentItemIndex = 0;
                }
            }
            else if (scroll < 0)
            {
                currentItemIndex--;
                if (currentItemIndex < 0)
                {
                    currentItemIndex = inventory.Count - 1;
                }
            }
        }
        
        if (previousIndex != currentItemIndex)
        {
            UpdateActiveItem();
        }
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
        // Hide all items
        foreach (var item in inventory)
        {
            if (item is HoldableItem holdable)
            {
                holdable.SetActive(false);
            }
            else if (item is ResetItem reset)
            {
                reset.SetActive(false);
            }
        }
        
        // Show current item
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
            
            Debug.Log("Switched to: " + itemName + " (Slot " + (currentItemIndex + 1) + ")");
        }
    }
    
    void TryPickupItem()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            // Try HoldableItem
            HoldableItem holdable = hit.collider.GetComponent<HoldableItem>();
            if (holdable != null && !holdable.IsBeingHeld())
            {
                AddToInventory(holdable);
                return;
            }
            
            // Try ResetItem
            ResetItem reset = hit.collider.GetComponent<ResetItem>();
            if (reset != null && !reset.IsBeingHeld())
            {
                inventory.Add(reset);
                reset.PickUp(holdPoint);
                
                // Admin dialogue when picking up Reset Device
                if (AdminDialogue.Instance != null && reset.itemName.Contains("Reset"))
                {
                    AdminDialogue.Instance.AdminInfo("Rollback device granted.");
                    AdminDialogue.Instance.AdminInfo("Limited use. Use responsibly.");
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
                
                Debug.Log("Added to inventory: " + reset.itemName + " (Slot " + inventory.Count + ")");
            }
        }
    }

    // Called by PlayerInteraction when player presses E near a HoldableItem
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

        Debug.Log("Added to inventory: " + holdable.itemName + " (Slot " + inventory.Count + ")");
    }

public string GetItemNameAtIndex(int index)
{
    if (index < 0 || index >= inventory.Count) return "";

    var item = inventory[index];

    if (item is HoldableItem holdable)
        return holdable.itemName;
    else if (item is ResetItem reset)
        return reset.itemName;

    return "";
}
    void DropCurrentItem()
    {
        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            var item = inventory[currentItemIndex];
            
            if (item is HoldableItem holdable)
            {
                holdable.Drop();
            }
            else if (item is ResetItem reset)
            {
                reset.Drop();
            }
            
            inventory.RemoveAt(currentItemIndex);
            
            if (inventory.Count == 0)
            {
                currentItemIndex = -1;
            }
            else
            {
                currentItemIndex--;
                if (currentItemIndex < 0)
                {
                    currentItemIndex = inventory.Count - 1;
                }
                UpdateActiveItem();
            }
        }
    }
    
    public bool IsHoldingItem()
    {
        return inventory.Count > 0;
    }
    
    public int GetInventoryCount()
    {
        return inventory.Count;
    }

    public string GetCurrentItemName()
    {
        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            var item = inventory[currentItemIndex];
            
            if (item is HoldableItem holdable)
            {
                return holdable.itemName;
            }
            else if (item is ResetItem reset)
            {
                return reset.itemName;
            }
        }
        return "None";
    }
}