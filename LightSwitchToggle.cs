using UnityEngine;

public class LightSwitchToggle : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject hiddenDoor;
    
    [Header("Light Settings")]
    public Light roomLight;
    
    [Header("State")]
    public bool isOn = true;
    
    private Renderer rend;
    private int toggleCount = 0;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        
        // Make sure door starts hidden
        if (hiddenDoor != null)
        {
            hiddenDoor.SetActive(false);
            Debug.Log("Hidden door disabled at start.");
        }
        else
        {
            Debug.LogWarning("Hidden door not assigned to LightSwitch!");
        }
    }
    
    public void Toggle()
    {
        toggleCount++;
        isOn = !isOn;
        
        Debug.Log("Light switch toggled! State: " + (isOn ? "ON" : "OFF") + " | Toggle count: " + toggleCount);
        
        // Change switch color
        if (rend != null)
        {
            rend.material.color = isOn ? Color.white : Color.red;
        }
        
        // Toggle room light
        if (roomLight != null)
        {
            roomLight.enabled = isOn;
            Debug.Log("Room light: " + (isOn ? "ON" : "OFF"));
        }
        
        // Show hidden door when turned OFF
        if (!isOn && hiddenDoor != null)
        {
            hiddenDoor.SetActive(true);
            Debug.Log("Hidden door revealed!");
            
            if (AdminDialogue.Instance != null)
            {
                AdminDialogue.Instance.AdminWarning("Correction accepted.");
                AdminDialogue.Instance.AdminWarning("Structural deviation increased.");
            }
            
            // Increase instability
            if (InstabilityManager.Instance != null)
            {
                InstabilityManager.Instance.IncreaseInstability(5f);
            }
        }
    }
}