using UnityEngine;

public class PlayerInventoryChecker : MonoBehaviour
{
    private ItemHolder itemHolder;

    void Start()
    {
        itemHolder = GetComponent<ItemHolder>();
    }

    public bool HasItem(string itemName)
    {
        if (itemHolder == null) return false;

        for (int i = 0; i < itemHolder.GetInventoryCount(); i++)
        {
            if (itemHolder.GetItemNameAtIndex(i) == itemName)
                return true;
        }
        return false;
    }
}