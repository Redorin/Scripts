using UnityEngine;

// Attach to each Cable object in the scene.
// Player presses E to connect cable to nearest open socket.

public class CableConnectionPuzzle : MonoBehaviour
{
    [Header("Cable Info")]
    public string cableName = "Cable";
    public bool isConnected = false;

    [Header("Visual")]
    public Renderer cableRenderer;
    public Color disconnectedColor = Color.red;
    public Color connectedColor = Color.green;

    void Start()
    {
        if (cableRenderer == null)
            cableRenderer = GetComponent<Renderer>();

        if (cableRenderer != null)
            cableRenderer.material.color = disconnectedColor;
    }

    public void Interact()
    {
        if (isConnected)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Cable already connected.");
            return;
        }

        // Find nearest available socket
        CableSocket[] sockets = FindObjectsOfType<CableSocket>();
        CableSocket nearest = null;
        float nearestDist = float.MaxValue;

        foreach (CableSocket socket in sockets)
        {
            if (socket.isFilled) continue;
            float dist = Vector3.Distance(transform.position, socket.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = socket;
            }
        }

        if (nearest != null)
        {
            ConnectToSocket(nearest);
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning("No available socket detected.");
        }
    }

    void ConnectToSocket(CableSocket socket)
    {
        isConnected = true;
        socket.Fill(this);

        // Snap cable visually to socket position
        transform.position = socket.transform.position;
        transform.SetParent(socket.transform);

        if (cableRenderer != null)
            cableRenderer.material.color = connectedColor;

        Debug.Log(cableName + " connected to " + socket.name);

        // Notify manager
        CableConnectionPuzzleManager manager = FindObjectOfType<CableConnectionPuzzleManager>();
        if (manager != null)
            manager.CheckAllConnected();
    }
}