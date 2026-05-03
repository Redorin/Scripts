using UnityEngine;

// Attach to puzzle objects in Chapter 4 that the player interacts with.
// Each interaction registers a choice with Chapter4Manager (stability or truth).
// Examples: a terminal, a document, a switch - anything that represents a decision.

public class Chapter4PuzzleChoice : MonoBehaviour
{
    [Header("Choice")]
    public bool isStabilityChoice = true;   // True = aligns with Admin A, False = Admin B

    [Header("Dialogue shown before choice registers")]
    public string choiceDescription = "This terminal shows system logs.";

    [Header("Admin Reaction")]
    public string[] adminAReaction;         // What Admin A says if this is chosen
    public string[] adminBReaction;         // What Admin B says if this is chosen

    [Header("Visual")]
    public Renderer choiceRenderer;
    public Color chosenColor = Color.gray;

    private bool hasBeenChosen = false;

    void Start()
    {
        if (choiceRenderer == null)
            choiceRenderer = GetComponent<Renderer>();
    }

    public void Interact()
    {
        if (hasBeenChosen) return;
        hasBeenChosen = true;

        // Show description
        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(choiceDescription);

        // Gray out object
        if (choiceRenderer != null)
            choiceRenderer.material.color = chosenColor;

        // Register with manager
        if (Chapter4Manager.Instance != null)
            Chapter4Manager.Instance.RegisterChoice(isStabilityChoice);

        Debug.Log("Choice made: " + (isStabilityChoice ? "Stability" : "Truth"));
    }
}