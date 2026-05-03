using UnityEngine;

public class EndlessHallway : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float resetTriggerDistance = 300f;

    [Header("Loop Direction")]
    public bool loopForward = true;

    [Header("Pattern Detection")]
    public bool isAnchored = false;

    [Header("Activation")]
    public bool playerInHallway = false;

    private bool hasShownMessage = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!playerInHallway || isAnchored || player == null) return;

        float currentZ = player.position.z;
        float hallwayStartZ = transform.position.z;
        float distanceFromStart = currentZ - hallwayStartZ;

        if (loopForward && distanceFromStart > resetTriggerDistance)
            LoopPlayer();
        else if (!loopForward && distanceFromStart < -resetTriggerDistance)
            LoopPlayer();
    }

    void LoopPlayer()
    {
        Vector3 newPos = player.position;

        if (loopForward)
            newPos.z -= resetTriggerDistance;
        else
            newPos.z += resetTriggerDistance;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.position = newPos;
            controller.enabled = true;
        }
        else
        {
            player.position = newPos;
        }

        Debug.Log("Hallway looped!");

        if (!hasShownMessage && AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning("Forward movement detected.");
            AdminDialogue.Instance.AdminWarning("Progress unchanged.");
            hasShownMessage = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHallway = true;
            Debug.Log("Player entered endless hallway zone.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHallway = false;
            Debug.Log("Player exited endless hallway zone.");
        }
    }

    public void AnchorHallway()
    {
        isAnchored = true;
        Debug.Log("Hallway anchored!");

        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning("Pattern recognition confirmed.");
            AdminDialogue.Instance.AdminWarning("Instability rising.");
        }

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(10f);
    }
}