using UnityEngine;
using System.Collections;

public class FuseBoxPuzzle : MonoBehaviour
{
    [Header("References")]
    public BreakerPanelPuzzle breakerPanel;
    public Light[] roomLights;
    public GameObject fuseSlotEmpty;
    public GameObject fuseSlotFilled;

    [Header("State")]
    public bool hasFuse = false;
    public bool isPowered = false;

    [Header("Dialogue")]
    public string noFuseMessage = "The fuse slot is empty.";
    public string fuseInsertedMessage = "Fuse inserted. Panel ready.";

    private PlayerInventoryChecker inventoryChecker;

    void Start()
    {
        inventoryChecker = FindObjectOfType<PlayerInventoryChecker>();

        if (fuseSlotFilled != null)
            fuseSlotFilled.SetActive(false);
        if (fuseSlotEmpty != null)
            fuseSlotEmpty.SetActive(true);

        SetLights(false);
    }

    public void Interact()
    {
        if (hasFuse)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Fuse already installed.");
            return;
        }

        if (inventoryChecker != null && inventoryChecker.HasItem("Fuse"))
        {
            InsertFuse();
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(noFuseMessage);
        }
    }

    void InsertFuse()
    {
        hasFuse = true;

        if (fuseSlotEmpty != null)
            fuseSlotEmpty.SetActive(false);
        if (fuseSlotFilled != null)
            fuseSlotFilled.SetActive(true);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(fuseInsertedMessage);

        if (breakerPanel != null)
            breakerPanel.SetFuseReady(true);
    }

    public void PowerOn()
    {
        isPowered = true;
        StartCoroutine(FlickerLightsOn());
    }

    IEnumerator FlickerLightsOn()
    {
        for (int i = 0; i < 4; i++)
        {
            SetLights(true);
            yield return new WaitForSeconds(0.1f);
            SetLights(false);
            yield return new WaitForSeconds(0.1f);
        }
        SetLights(true);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(
                "Power restored. Systems online.");
    }

    void SetLights(bool on)
    {
        foreach (Light l in roomLights)
            if (l != null) l.enabled = on;
    }
}