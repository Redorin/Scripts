using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
    [Header("References")]
    public ItemHolder itemHolder;
    public Transform slotsContainer;

    [Header("Slot Settings")]
    public int maxSlots = 5;
    public Color normalSlotColor = new Color(0f, 0f, 0f, 0.6f);
    public Color selectedSlotColor = new Color(0f, 0.8f, 0.8f, 0.3f);
    public Color normalTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    public Color selectedTextColor = new Color(0f, 1f, 1f, 1f);
    public Color emptyTextColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color numberColor = new Color(0f, 0.7f, 0.7f, 1f);

    [Header("Visibility")]
    public bool hideWhenEmpty = true;
    public float fadeSpeed = 3f;

    private List<HotbarSlot> slots = new List<HotbarSlot>();
    private CanvasGroup canvasGroup;
    private int lastSelectedIndex = -1;
    private int lastInventoryCount = -1;

    [System.Serializable]
    public class HotbarSlot
    {
        public GameObject slotObject;
        public Image slotBackground;
        public TextMeshProUGUI numberText;
        public TextMeshProUGUI itemText;
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (hideWhenEmpty)
            canvasGroup.alpha = 0f;

        BuildSlots();
    }

    void BuildSlots()
    {
        // Clear existing
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);
        slots.Clear();

        for (int i = 0; i < maxSlots; i++)
        {
            // Slot container
            GameObject slotObj = new GameObject("Slot_" + (i + 1));
            slotObj.transform.SetParent(slotsContainer, false);

            // Background image
            Image bg = slotObj.AddComponent<Image>();
            bg.color = normalSlotColor;

            // Layout
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(160f, 36f);

            // Number text
            GameObject numObj = new GameObject("Number");
            numObj.transform.SetParent(slotObj.transform, false);
            TextMeshProUGUI numText = numObj.AddComponent<TextMeshProUGUI>();
            numText.text = (i + 1).ToString();
            numText.fontSize = 14;
            numText.color = numberColor;
            numText.alignment = TextAlignmentOptions.Left;
            RectTransform numRt = numObj.GetComponent<RectTransform>();
            numRt.anchorMin = new Vector2(0f, 0f);
            numRt.anchorMax = new Vector2(0f, 1f);
            numRt.offsetMin = new Vector2(8f, 0f);
            numRt.offsetMax = new Vector2(24f, 0f);

            // Item name text
            GameObject itemObj = new GameObject("ItemName");
            itemObj.transform.SetParent(slotObj.transform, false);
            TextMeshProUGUI itemText = itemObj.AddComponent<TextMeshProUGUI>();
            itemText.text = "—";
            itemText.fontSize = 15;
            itemText.color = emptyTextColor;
            itemText.alignment = TextAlignmentOptions.Left;
            RectTransform itemRt = itemObj.GetComponent<RectTransform>();
            itemRt.anchorMin = new Vector2(0f, 0f);
            itemRt.anchorMax = new Vector2(1f, 1f);
            itemRt.offsetMin = new Vector2(28f, 0f);
            itemRt.offsetMax = new Vector2(-8f, 0f);

            HotbarSlot slot = new HotbarSlot
            {
                slotObject = slotObj,
                slotBackground = bg,
                numberText = numText,
                itemText = itemText
            };

            slots.Add(slot);
        }
    }

    void Update()
    {
        if (itemHolder == null) return;

        int count = itemHolder.GetInventoryCount();
        int selectedIndex = itemHolder.GetCurrentIndex();

        // Only update if something changed
        if (count != lastInventoryCount || selectedIndex != lastSelectedIndex)
        {
            UpdateSlots(count, selectedIndex);
            lastInventoryCount = count;
            lastSelectedIndex = selectedIndex;
        }

        // Fade in/out based on inventory
        if (hideWhenEmpty)
        {
            float targetAlpha = count > 0 ? 1f : 0f;
            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }
    }

    void UpdateSlots(int count, int selectedIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            bool hasItem = i < count;
            bool isSelected = i == selectedIndex;

            // Background color
            slots[i].slotBackground.color = isSelected
                ? selectedSlotColor
                : normalSlotColor;

            // Item name
            if (hasItem)
            {
                string name = itemHolder.GetItemNameAtIndex(i);
                slots[i].itemText.text = name;
                slots[i].itemText.color = isSelected
                    ? selectedTextColor
                    : normalTextColor;
            }
            else
            {
                slots[i].itemText.text = "—";
                slots[i].itemText.color = emptyTextColor;
            }

            // Number color
            slots[i].numberText.color = isSelected
                ? selectedTextColor
                : numberColor;
        }
    }
}