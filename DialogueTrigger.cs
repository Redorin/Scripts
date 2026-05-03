using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string[] dialogueLines; // Multiple lines that will play in sequence
    
    [Header("Trigger Settings")]
    public bool triggerOnce = true; // Only trigger once, then disable
    public float delayBeforeDialogue = 0f; // Delay before showing dialogue
    
    [Header("Trigger Type")]
    public bool requireInteraction = false; // If true, player must press E instead of just walking in
    
    [Header("Visual (Optional)")]
    public bool showDebugBox = true; // Show trigger zone in Scene view
    
    private bool hasTriggered = false;
    private bool playerInZone = false;
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (other.CompareTag("Player"))
        {
            if (requireInteraction)
            {
                // Player entered zone, but needs to press E
                playerInZone = true;
                Debug.Log("Press E to trigger dialogue");
            }
            else
            {
                // Auto-trigger dialogue
                TriggerDialogue();
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
    
    void Update()
    {
        // If requires interaction and player is in zone
        if (requireInteraction && playerInZone && !hasTriggered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TriggerDialogue();
            }
        }
    }
    
    void TriggerDialogue()
    {
        // Check if already triggered
        if (hasTriggered && triggerOnce)
        {
            return;
        }
        
        // Mark as triggered
        hasTriggered = true;
        
        // Show dialogue after delay
        if (delayBeforeDialogue > 0)
        {
            Invoke("ShowDialogue", delayBeforeDialogue);
        }
        else
        {
            ShowDialogue();
        }
        
        Debug.Log("Dialogue triggered!");
    }
    
    void ShowDialogue()
    {
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in dialogueLines)
            {
                AdminDialogue.Instance.AdminWarning(line);
            }
        }
        else
        {
            Debug.LogWarning("AdminDialogue.Instance is null! Make sure AdminDialogue exists in scene.");
        }
        
        // Disable trigger if it's one-time use
        if (triggerOnce)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
    
    // Draw debug box in Scene view
    void OnDrawGizmos()
    {
        if (showDebugBox)
        {
            Gizmos.color = hasTriggered ? Color.gray : Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }
}