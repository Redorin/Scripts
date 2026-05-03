using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    public Transform teleportDestination; // Where the player will be teleported
    public bool useTeleportPoint = true; // If true, use teleportDestination. If false, use destinationCoordinates
    
    [Header("Manual Coordinates (if not using teleport point)")]
    public Vector3 destinationCoordinates = Vector3.zero;
    
    [Header("Effects (Optional)")]
    public bool fadeEffect = false;
    public float teleportDelay = 0f; // Delay before teleporting (in seconds)
    
    [Header("One-Way Teleport")]
    public bool oneTimeUse = false; // Can only be used once
    private bool hasBeenUsed = false;
    
    public void Interact()
    {
        if (oneTimeUse && hasBeenUsed)
        {
            Debug.Log("This teleporter has already been used!");
            return;
        }
        
        if (teleportDelay > 0)
        {
            Invoke("TeleportPlayer", teleportDelay);
            Debug.Log("Teleporting in " + teleportDelay + " seconds...");
        }
        else
        {
            TeleportPlayer();
        }
        
        if (oneTimeUse)
        {
            hasBeenUsed = true;
        }
    }
    
    void TeleportPlayer()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            return;
        }
        
        // Get destination position
        Vector3 destination;
        
        if (useTeleportPoint && teleportDestination != null)
        {
            destination = teleportDestination.position;
        }
        else
        {
            destination = destinationCoordinates;
        }
        
        // Teleport the player
        CharacterController controller = player.GetComponent<CharacterController>();
        
        if (controller != null)
        {
            // Disable the controller temporarily to move the player
            controller.enabled = false;
            player.transform.position = destination;
            controller.enabled = true;
        }
        else
        {
            // If no CharacterController, just move directly
            player.transform.position = destination;
        }
        
        Debug.Log("Player teleported to: " + destination);
    }
}