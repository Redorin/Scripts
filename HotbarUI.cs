using UnityEngine;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    [Header("References")]
    public ItemHolder itemHolder;
    public TextMeshProUGUI hotbarText;

    void Start()
    {
        // Hide the hotbar text completely
        if (hotbarText != null)
            hotbarText.gameObject.SetActive(false);
    }

    void Update()
    {
        // UI display disabled
    }
}