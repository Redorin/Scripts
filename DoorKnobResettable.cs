using UnityEngine;

public class DoorKnobResettable : MonoBehaviour
{
    [Header("References")]
    public BrokenDoorknob doorknobManager;

    [Header("Visuals")]
    public GameObject brokenKnobVisual;
    public GameObject fixedKnobVisual;

    private ResettableObject resettable;
    private int lastUseCount = 0;

    void Start()
    {
        resettable = GetComponent<ResettableObject>();
        if (brokenKnobVisual != null) brokenKnobVisual.SetActive(true);
        if (fixedKnobVisual != null) fixedKnobVisual.SetActive(false);
    }

    void Update()
    {
        if (resettable == null) return;

        int currentUses = resettable.maxResetUses - resettable.GetRemainingUses();
        if (currentUses > lastUseCount)
        {
            lastUseCount = currentUses;
            if (brokenKnobVisual != null) brokenKnobVisual.SetActive(false);
            if (fixedKnobVisual != null) fixedKnobVisual.SetActive(true);
            if (doorknobManager != null) doorknobManager.OnKnobReset();
        }
    }
}