using UnityEngine;

// Attach to an empty GameObject in Chapter 1 scene.
// Call these methods from the relevant puzzle scripts at the right moments.
// All methods are static-callable via Chapter1Objectives.Instance

public class Chapter1Objectives : MonoBehaviour
{
    public static Chapter1Objectives Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // First objective added on scene load
        AddObjective_FindWayOut();
    }

    // ── MAIN ROOM ──

    public void AddObjective_FindWayOut()
    {
        ObjectiveManager.Instance?.AddObjective("Find a way out of the lecture hall");
    }

    public void Complete_FindWayOut()
    {
        ObjectiveManager.Instance?.CompleteObjective("Find a way out of the lecture hall");
        AddObjective_PickUpResetDevice();
    }

    public void AddObjective_PickUpResetDevice()
    {
        ObjectiveManager.Instance?.AddObjective("Pick up the Reset Device");
    }

    public void Complete_PickUpResetDevice()
    {
        ObjectiveManager.Instance?.CompleteObjective("Pick up the Reset Device");
    }

    // ── HALLWAY ──

    public void AddObjective_FindAnotherWay()
    {
        ObjectiveManager.Instance?.AddObjective("Find another way — hallway is blocked");
    }

    public void Complete_FindAnotherWay()
    {
        ObjectiveManager.Instance?.CompleteObjective("Find another way — hallway is blocked");
    }

    // ── MAINTENANCE ROOM ──

    public void AddObjective_RestorePower()
    {
        ObjectiveManager.Instance?.AddObjective("Restore power to the maintenance room");
    }

    public void AddObjective_FindFuse()
    {
        ObjectiveManager.Instance?.AddObjective("Locate replacement fuses");
    }

    public void Complete_FindFuse()
    {
        ObjectiveManager.Instance?.CompleteObjective("Locate replacement fuses");
        ObjectiveManager.Instance?.AddObjective("Insert fuses into the fuse box");
    }

    public void Complete_InsertFuses()
    {
        ObjectiveManager.Instance?.CompleteObjective("Insert fuses into the fuse box");
        ObjectiveManager.Instance?.AddObjective("Reset the damaged power conduit");
    }

    public void Complete_ResetConduit()
    {
        ObjectiveManager.Instance?.CompleteObjective("Reset the damaged power conduit");
        ObjectiveManager.Instance?.AddObjective("Activate breakers in correct sequence");
    }

    public void Complete_Breakers()
    {
        ObjectiveManager.Instance?.CompleteObjective("Activate breakers in correct sequence");
        ObjectiveManager.Instance?.CompleteObjective("Restore power to the maintenance room");
        ObjectiveManager.Instance?.AddObjective("Find the Arzatech keycard");
    }

    public void Complete_FindKeycard()
    {
        ObjectiveManager.Instance?.CompleteObjective("Find the Arzatech keycard");
        ObjectiveManager.Instance?.AddObjective("Access the Arzatech server room");
    }

    // ── SERVER ROOM ──

    public void AddObjective_ServerRoom()
    {
        ObjectiveManager.Instance?.AddObjective("Restore the server room connection");
    }

    public void AddObjective_ResetBurntSocket()
    {
        ObjectiveManager.Instance?.AddObjective("Reset the damaged cable socket");
    }

    public void Complete_ResetBurntSocket()
    {
        ObjectiveManager.Instance?.CompleteObjective("Reset the damaged cable socket");
        ObjectiveManager.Instance?.AddObjective("Connect both cable sockets to wall plugs");
    }

    public void Complete_ConnectCables()
    {
        ObjectiveManager.Instance?.CompleteObjective("Connect both cable sockets to wall plugs");
        ObjectiveManager.Instance?.AddObjective("Boot servers in correct sequence");
    }

    public void Complete_BootServers()
    {
        ObjectiveManager.Instance?.CompleteObjective("Boot servers in correct sequence");
        ObjectiveManager.Instance?.CompleteObjective("Restore the server room connection");
        ObjectiveManager.Instance?.CompleteObjective("Access the Arzatech server room");
        ObjectiveManager.Instance?.AddObjective("Retrieve the archive room key");
    }

    public void Complete_GetArchiveKey()
    {
        ObjectiveManager.Instance?.CompleteObjective("Retrieve the archive room key");
        ObjectiveManager.Instance?.AddObjective("Access the archive room");
    }

    // ── ARCHIVE ROOM ──

    public void Complete_AccessArchive()
    {
        ObjectiveManager.Instance?.CompleteObjective("Access the archive room");
        ObjectiveManager.Instance?.AddObjective("Investigate the archive room");
    }

    public void Complete_InvestigateArchive()
    {
        ObjectiveManager.Instance?.CompleteObjective("Investigate the archive room");
        ObjectiveManager.Instance?.AddObjective("Return to the maintenance room");
    }

    // ── OVERRIDE PANEL ──

    public void Complete_ReturnToMaintenance()
    {
        ObjectiveManager.Instance?.CompleteObjective("Return to the maintenance room");
        ObjectiveManager.Instance?.AddObjective("Activate the maintenance override panel");
    }

    public void Complete_OverridePanel()
    {
        ObjectiveManager.Instance?.CompleteObjective("Activate the maintenance override panel");
        ObjectiveManager.Instance?.AddObjective("Reach the 4th floor");
    }

    // ── FOURTH FLOOR ──

    public void AddObjective_CrackedPanel()
    {
        ObjectiveManager.Instance?.AddObjective("Reset the cracked floor panel");
    }

    public void Complete_CrackedPanel()
    {
        ObjectiveManager.Instance?.CompleteObjective("Reset the cracked floor panel");
        ObjectiveManager.Instance?.AddObjective("Reach the sector transfer point");
    }

    public void Complete_Chapter1()
    {
        ObjectiveManager.Instance?.CompleteObjective("Reach the sector transfer point");
        ObjectiveManager.Instance?.CompleteObjective("Reach the 4th floor");
    }
}