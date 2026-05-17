// File: BurntCableResettable.cs
using UnityEngine;

[RequireComponent(typeof(ResettableObject))]
public class BurntCableResettable : MonoBehaviour
{
    [Header("References")]
    public CableConnectionPuzzle cable;

    [Header("Visuals")]
    public GameObject burntVisual;
    public GameObject fixedVisual;

    void Awake()
    {
        if (cable == null)
            cable = GetComponent<CableConnectionPuzzle>();
        if (cable == null)
            cable = GetComponentInChildren<CableConnectionPuzzle>();

        // Start burnt
        if (burntVisual != null) burntVisual.SetActive(true);
        if (fixedVisual  != null) fixedVisual.SetActive(false);
    }

    public void OnCableRestored()
    {
        if (cable != null)
            cable.Restore();

        // Swap visuals
        if (burntVisual != null) burntVisual.SetActive(false);
        if (fixedVisual  != null) fixedVisual.SetActive(true);

        // Fire objective trigger on this GameObject
        GetComponent<ObjectiveTrigger>()?.TriggerObjective();
    }
}