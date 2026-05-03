using UnityEngine;

// Attach to each Socket object in the Server Room.
// Cables will snap to this when connected.

public class CableSocket : MonoBehaviour
{
    public bool isFilled = false;
    private CableConnectionPuzzle connectedCable = null;

    public void Fill(CableConnectionPuzzle cable)
    {
        isFilled = true;
        connectedCable = cable;
        Debug.Log(gameObject.name + " socket filled.");
    }
}