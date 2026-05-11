using UnityEngine;

[RequireComponent(typeof(ResettableObject))]
public class BurntCableResettable : MonoBehaviour
{
    public CableConnectionPuzzle cable;

    void Awake()
    {
        if (cable == null)
            cable = GetComponent<CableConnectionPuzzle>();
        if (cable == null)
            cable = GetComponentInChildren<CableConnectionPuzzle>();
    }

    public void OnCableRestored()
    {
        if (cable != null)
            cable.Restore();
    }
}