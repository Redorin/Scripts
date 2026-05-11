// NPCDisabler.cs
// Attach to your CutsceneManager or any persistent GameObject in the scene.
using UnityEngine;

public class NPCDisabler : MonoBehaviour
{
    [Header("NPCs to hide during cutscene")]
    public GameObject[] npcObjects; // drag male01_3, male03_3, etc. here

    // Call this from your opening cutscene at the moment they should vanish
    public void DisableAllNPCs()
    {
        foreach (var npc in npcObjects)
        {
            if (npc != null)
                npc.SetActive(false);
        }
    }
}