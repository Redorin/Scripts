using UnityEngine;

public class DebrisBarrier : MonoBehaviour
{
    [Header("Barrier Area")]
    public Vector3 center = Vector3.zero;
    public Vector3 size = new Vector3(4f, 4f, 4f);
    public float wallThickness = 0.3f;

    [Header("References")]
    public DebrisGroup debrisGroup;

    [Header("Interaction")]
    public string resetDialogue = "Barrier rollback initiated.";

    [Header("State")]
    public bool startActive = true;

    private GameObject[] walls = new GameObject[4];
    private bool isActive = false;
    private bool isPermanent = false;

    void Start()
    {
        BuildWalls();

        if (startActive)
            Activate(allowReset: true);
        else
            Deactivate();
    }

    void BuildWalls()
    {
        float halfX = size.x / 2f;
        float halfZ = size.z / 2f;
        float t = wallThickness / 2f;

        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0,            0,  halfZ + t),
            new Vector3(0,            0, -(halfZ + t)),
            new Vector3( halfX + t,   0,  0),
            new Vector3(-(halfX + t), 0,  0)
        };

        Vector3[] wallSizes = new Vector3[]
        {
            new Vector3(size.x + wallThickness * 2, size.y, wallThickness),
            new Vector3(size.x + wallThickness * 2, size.y, wallThickness),
            new Vector3(wallThickness, size.y, size.z),
            new Vector3(wallThickness, size.y, size.z)
        };

        string[] names = { "DebrisWall_N", "DebrisWall_S", "DebrisWall_E", "DebrisWall_W" };

        for (int i = 0; i < 4; i++)
        {
            walls[i] = new GameObject(names[i]);
            walls[i].transform.position = center + offsets[i];

            BoxCollider bc = walls[i].AddComponent<BoxCollider>();
            bc.size = wallSizes[i];
            bc.center = Vector3.zero;

            BarrierWall bw = walls[i].AddComponent<BarrierWall>();
            bw.barrier = this;
        }
    }

    void Activate(bool allowReset)
    {
        isActive = true;
        foreach (var wall in walls)
        {
            if (wall == null) continue;
            wall.SetActive(true);

            ResettableObject r = wall.GetComponent<ResettableObject>();
            if (r != null)
            {
                r.canBeReset = allowReset;
                if (allowReset) r.ResetUseCount();
            }
        }
    }

    public void Deactivate()
    {
        isActive = false;
        foreach (var wall in walls)
            if (wall != null) wall.SetActive(false);
    }

    // Called by BarrierWall when player uses Reset Device on any wall
    public void TriggerDebrisReset()
    {
        if (!isActive || isPermanent) return;

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning(resetDialogue);

        if (debrisGroup != null)
            debrisGroup.ResetAll();

        Deactivate();
    }

    // Called by DebrisGroup after reset animation finishes
    public void OnDebrisReset()
    {
        Deactivate();
    }

    // Called by CollapseAgain — walls come back permanently, cannot be reset
    public void OnCollapseAgain()
    {
        isPermanent = true;
        Activate(allowReset: false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.9f);
        Gizmos.DrawWireCube(center, size);
    }
}