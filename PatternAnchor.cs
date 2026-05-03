using UnityEngine;

public class PatternAnchor : MonoBehaviour
{
    public EndlessHallway hallwayManager;
    private bool hasBeenActivated = false;
    
    public void Interact()
    {
        if (hasBeenActivated) return;
        
        hasBeenActivated = true;
        
        if (hallwayManager != null)
        {
            hallwayManager.AnchorHallway();
        }
        
        // Visual feedback - change color
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }
        
        Debug.Log("Pattern anchored!");
    }
}