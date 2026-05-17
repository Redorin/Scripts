// File: Chapter1Objectives.cs
// ============================================================================
using UnityEngine;

public class Chapter1Objectives : MonoBehaviour
{
    public static Chapter1Objectives Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ── MAIN ROOM ──

    public void Complete_FindWayOut()
    {
        ObjectiveManager.Instance?.Complete("find_way_out");
        ObjectiveManager.Instance?.Add("pickup_reset_device");
    }

    public void Complete_PickUpResetDevice()
    {
        ObjectiveManager.Instance?.Complete("pickup_reset_device");
        ObjectiveManager.Instance?.Add("find_another_way");
    }

    // ── HALLWAY ──

    public void Complete_FindAnotherWay()
    {
        ObjectiveManager.Instance?.Complete("find_another_way");
        ObjectiveManager.Instance?.Add("restore_power");
        ObjectiveManager.Instance?.Add("find_fuse");
    }

    // ── MAINTENANCE ROOM ──

    public void Complete_FindFuse()
    {
        ObjectiveManager.Instance?.Complete("find_fuse");
        ObjectiveManager.Instance?.Add("insert_fuses");
    }

    public void Complete_InsertFuses()
    {
        ObjectiveManager.Instance?.Complete("insert_fuses");
        ObjectiveManager.Instance?.Add("reset_conduit");
    }

    public void Complete_ResetConduit()
    {
        ObjectiveManager.Instance?.Complete("reset_conduit");
        ObjectiveManager.Instance?.Add("activate_breakers");
    }

    public void Complete_Breakers()
    {
        ObjectiveManager.Instance?.Complete("activate_breakers");
        ObjectiveManager.Instance?.Complete("restore_power");
        ObjectiveManager.Instance?.Add("find_keycard");
    }

    public void Complete_FindKeycard()
    {
        ObjectiveManager.Instance?.Complete("find_keycard");
        ObjectiveManager.Instance?.Add("access_server_room");
    }

    // ── SERVER ROOM ──

    public void Add_ServerRoom()
    {
        ObjectiveManager.Instance?.Add("restore_server_connection");
        ObjectiveManager.Instance?.Add("reset_burnt_socket");
    }

    public void Complete_ResetBurntSocket()
    {
        ObjectiveManager.Instance?.Complete("reset_burnt_socket");
        ObjectiveManager.Instance?.Add("connect_cables");
    }

    public void Complete_ConnectCables()
    {
        ObjectiveManager.Instance?.Complete("connect_cables");
        ObjectiveManager.Instance?.Add("boot_servers");
    }

    public void Complete_BootServers()
    {
        ObjectiveManager.Instance?.Complete("boot_servers");
        ObjectiveManager.Instance?.Complete("restore_server_connection");
        ObjectiveManager.Instance?.Complete("access_server_room");
        ObjectiveManager.Instance?.Add("retrieve_archive_key");
    }

    public void Complete_GetArchiveKey()
    {
        ObjectiveManager.Instance?.Complete("retrieve_archive_key");
        ObjectiveManager.Instance?.Add("access_archive");
    }

    // ── ARCHIVE ROOM ──

    public void Complete_AccessArchive()
    {
        ObjectiveManager.Instance?.Complete("access_archive");
        ObjectiveManager.Instance?.Add("investigate_archive");
    }

    public void Complete_InvestigateArchive()
    {
        ObjectiveManager.Instance?.Complete("investigate_archive");
        ObjectiveManager.Instance?.Add("return_to_maintenance");
    }

    // ── OVERRIDE PANEL ──

    public void Complete_ReturnToMaintenance()
    {
        ObjectiveManager.Instance?.Complete("return_to_maintenance");
        ObjectiveManager.Instance?.Add("activate_override");
    }

    public void Complete_OverridePanel()
    {
        ObjectiveManager.Instance?.Complete("activate_override");
        ObjectiveManager.Instance?.Add("reach_4th_floor");
    }

    // ── FOURTH FLOOR ──

    public void Add_CrackedPanel()
    {
        ObjectiveManager.Instance?.Add("reset_cracked_panel");
    }

    public void Complete_CrackedPanel()
    {
        ObjectiveManager.Instance?.Complete("reset_cracked_panel");
        ObjectiveManager.Instance?.Add("reach_transfer_point");
    }

    public void Complete_Chapter1()
    {
        ObjectiveManager.Instance?.Complete("reach_transfer_point");
        ObjectiveManager.Instance?.Complete("reach_4th_floor");
    }
}