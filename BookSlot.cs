using UnityEngine;

// Attach to each book slot object on the library shelf.
// Player presses E to cycle through book titles.

public class BookSlot : MonoBehaviour
{
    [Header("Books this slot cycles through")]
    public string[] bookTitles = { "Control", "Observation", "Correction" };
    private int currentIndex = 0;

    [Header("Visual")]
    public Renderer slotRenderer;
    public Color[] bookColors;          // One color per book title

    [Header("Manager Reference")]
    public BookArrangementPuzzle puzzleManager;

    void Start()
    {
        if (slotRenderer == null)
            slotRenderer = GetComponent<Renderer>();

        UpdateVisual();
    }

    public void Interact()
    {
        currentIndex = (currentIndex + 1) % bookTitles.Length;
        UpdateVisual();
        Debug.Log(gameObject.name + " now shows: " + bookTitles[currentIndex]);

        if (puzzleManager != null)
            puzzleManager.CheckOrder();
    }

    public string GetCurrentBook()
    {
        return bookTitles[currentIndex];
    }

    void UpdateVisual()
    {
        if (slotRenderer != null && bookColors != null && currentIndex < bookColors.Length)
            slotRenderer.material.color = bookColors[currentIndex];
    }
}